using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Kafka;
using Wolverine.SqlServer;

namespace WolverineSagaIssue;

public static class WolverineSetup
{
    public static IServiceCollection SetUpWolverine(this IServiceCollection services, IConfiguration configuration)
    {
        string conStr = configuration.GetConnectionString(Configuration.DB.ConnectionStringName)!;
        string hosts = configuration.GetRequiredSection(Configuration.Kafka.HostsSection).Value!;
        string consumerGroupId = configuration.GetRequiredSection(Configuration.Kafka.ConsumerGroupIdSection).Value!;

        services.AddWolverine(opts =>
        {
            opts.UseKafka(hosts)
                .ConfigureClient(client =>
                {
                    client.AllowAutoCreateTopics = true;
                    client.BootstrapServers = hosts;
                })
                .ConfigureConsumers(consumerConfig =>
                {
                    consumerConfig.GroupId = consumerGroupId;
                });

            string topic = configuration.GetRequiredSection(Configuration.Kafka.TopicSection).Value!;

            opts.ListenToKafkaTopic(topic);
            opts.Publish(config =>
            {
                config.ToKafkaTopic(topic);
            });

            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.AutoApplyTransactions();
            opts.Durability.Mode = DurabilityMode.Solo;
            opts.Policies.UseDurableInboxOnAllListeners();
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
            opts.Policies.UseDurableLocalQueues();
            opts.PersistMessagesWithSqlServer(conStr!);
            //opts.AutoBuildMessageStorageOnStartup = JasperFx.AutoCreate.All;
            //opts.AutoBuildMessageStorageOnStartup = true;

        });

        return services;
    }
}
