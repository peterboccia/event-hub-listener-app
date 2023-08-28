namespace EventHubTestApp.Services
{
    internal class History
    {
        public string EventHubConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string EventHubConsumerGroup { get; set; }
        public string CheckpointConnectionString { get; set; }
        public string CheckpointContainerName { get; set; }
    }
}
