using Azure.Messaging.EventHubs.Primitives;
using Azure.Storage.Blobs;
using CIAM.DataProviders.EventHubs.Processors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubTester
{
    public static class EventHubManager
    {
        public static async Task RunEventHubListener(CancellationToken cancellationToken,
            string EventHubConnectionString,
            string EventHubName,
            string ConsumerGroupName,
            string CheckpointConnectionString,
            string CheckpointContainerName,
            Action<string> WriteLog)
        {
            WriteLog("Start running");
            try
            {
                // create blob connection for checkpoint management
                BlobContainerClient checkpointStorageClient = new BlobContainerClient(
                    CheckpointConnectionString,
                    CheckpointContainerName
                );
                BlobCheckpointStore checkpointStore = new BlobCheckpointStore(checkpointStorageClient);

                EventProcessorOptions options = new EventProcessorOptions
                {
                    LoadBalancingUpdateInterval = TimeSpan.FromSeconds(10),
                    MaximumWaitTime = TimeSpan.FromSeconds(10),
                };

                SimpleBatchPrecessor eventProcessorClient = new SimpleBatchPrecessor(
                    checkpointStore,
                    100,
                    ConsumerGroupName,
                    EventHubConnectionString,
                    EventHubName,
                    options,
                    WriteLog
                );

                WriteLog("Start processing");
                await eventProcessorClient.StartProcessingAsync(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    // The worker is still active
                    await Task.Delay(1000, cancellationToken);
                }

                await eventProcessorClient.StopProcessingAsync();
            }
            catch (System.Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }
    }
}
