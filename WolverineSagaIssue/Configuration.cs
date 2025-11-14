namespace WolverineSagaIssue;

public static class Configuration
{
    public static class DB
    {
        public const string ConnectionStringName = "OrdersDb";
    }

    public static class Kafka
    {
        public const string ConfigurationSection = "Kafka";

        public const string HostsSection = $"{ConfigurationSection}:HostsString";
        public const string TopicSection = $"{ConfigurationSection}:Topic";
        public const string ConsumerGroupIdSection = $"{ConfigurationSection}:ConsumerGroupId";
    }
}
