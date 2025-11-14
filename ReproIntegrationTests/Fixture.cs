using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.Kafka;
using Testcontainers.MsSql;
using Wolverine.Persistence.Durability;
using WolverineSagaIssue;

namespace ReproIntegrationTests;

public class Fixture : AppFixture<Program>
{
    public IHost Host => _host;
    public const int MessagingTimeoutInMilliseconds = 5000;

    private IHost _host = null!;
    private readonly MsSqlContainer _databaseContainer = null!;
    private readonly KafkaContainer _kafkaContainer = null!;
    private static string _connectionString = null!;
    private const string _localDevPassword = "password#123";
    private const string _databaseName = "OrdersTestDb";    

    public Fixture()
    {
        _databaseContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword(_localDevPassword)
            .WithCleanUp(false)
            .WithReuse(true)
        .Build();

        _kafkaContainer = new KafkaBuilder()
        .Build();
    }

    protected override async ValueTask PreSetupAsync()
    {
        await Task.WhenAll(
            _databaseContainer.StartAsync(),
            _kafkaContainer.StartAsync()
            );

        var tmpCnxString = _databaseContainer.GetConnectionString();

        using var cnx = new SqlConnection(tmpCnxString);
        using var cmd = cnx.CreateCommand();
        cmd.CommandText =
            $"""
                IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '{_databaseName}')
                CREATE DATABASE {_databaseName};
            """;
        await cnx.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        SqlConnectionStringBuilder connectionStringBuilder = new(tmpCnxString)
        {
            InitialCatalog = _databaseName,
            TrustServerCertificate = true
        };
        _connectionString = connectionStringBuilder.ConnectionString;
    }

    protected override IHost ConfigureAppHost(IHostBuilder a)
    {
        _host = a.ConfigureWebHost(builder =>
        {
            builder.Configure(_ => { });
        }).Build();
        _host.Start();
        return _host;
    }

    protected override async ValueTask SetupAsync()
    {
        _host ??= Services.GetRequiredService<IHost>();

        await EnsureDatabase();

        var store = Services.GetRequiredService<IMessageStore>();

        // Rebuild the database schema objects
        // and delete existing message data
        // This is good for testing
        await store.Admin.RebuildAsync();
    }

    private async ValueTask EnsureDatabase()
    {
        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LocalOrdersEfContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        a.ConfigureLogging((builder) =>
        {
            builder.ClearProviders();
        });

        a.UseContentRoot(Directory.GetCurrentDirectory());

        var configDict = new Dictionary<string, string?>()
        {
            [$"ConnectionStrings:{Configuration.DB.ConnectionStringName}"] = _connectionString,
            [Configuration.Kafka.HostsSection] = _kafkaContainer.GetBootstrapAddress(),
            [Configuration.Kafka.ConsumerGroupIdSection] = "Test",
            [Configuration.Kafka.TopicSection] = "Test"
        };

        a.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(configDict).Build());
    }
}
