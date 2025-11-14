using Microsoft.EntityFrameworkCore;
using WolverineSagaIssue;

var builder = Host.CreateApplicationBuilder(args);

var conStr = builder.Configuration.GetConnectionString(Configuration.DB.ConnectionStringName);

builder.Services.AddDbContextFactory<LocalOrdersEfContext>(options =>
{
    options.UseSqlServer(conStr!, config =>
    {
        config.EnableRetryOnFailure(3);
    });
});

builder.Services.AddScoped<IRepository, Repository>();

builder.Services.SetUpWolverine(builder.Configuration);

var host = builder.Build();
await host.RunAsync();

public partial class Program;