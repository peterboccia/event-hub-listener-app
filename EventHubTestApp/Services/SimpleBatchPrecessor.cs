using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace EventHubTestApp.Services
{
    internal class SimpleBatchPrecessor : PluggableCheckpointStoreEventProcessor<EventProcessorPartition>
    {
        private readonly Action<string> writeLog;

        public SimpleBatchPrecessor(
            CheckpointStore checkpointStore,
            int eventBatchMaximumCount,
            string consumerGroup,
            string connectionString,
            string eventHubName,
            EventProcessorOptions clientOptions,
            Action<string> WriteLog
        )
            : base(checkpointStore, eventBatchMaximumCount, consumerGroup, connectionString, eventHubName, clientOptions)
        {
            writeLog = WriteLog;
        }

        protected override Task OnProcessingErrorAsync(
            Exception exception,
            EventProcessorPartition partition,
            string operationDescription,
            CancellationToken cancellationToken
        )
        {
            writeLog($"Error occurred in the partition {partition?.PartitionId} ({operationDescription})");
            return Task.CompletedTask;
        }

        protected override async Task OnProcessingEventBatchAsync(
            IEnumerable<EventData> events,
            EventProcessorPartition partition,
            CancellationToken cancellationToken
        )
        {
            string partitionId = partition?.PartitionId ?? "NO_ID";
            writeLog($"Processing event batch started on {events.Count()} event data. Partition id: {partitionId}");

            try
            {
                // Iterates every events, keeping the last one processed
                EventData lastEvent = null;
                foreach (EventData ev in events)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // If cancellation is requested we break here
                        writeLog($"During batch processing, cancellation was requested.");
                        break;
                    }
                    try
                    {
                        string body = Encoding.UTF8.GetString(ev.Body.ToArray());
                        writeLog($"Event: {ev.SequenceNumber}");
                        writeLog(body);
                        ShowNotification($"Received Event: #{ev.SequenceNumber}", body);
                    }
                    catch (Exception)
                    {
                        writeLog(
                            $"Error client processing data with sequence number: {ev.SequenceNumber}, and partition: {partitionId}."
                        );
                    }
                    lastEvent = ev;
                }

                // if at least one event is processed, create a checkpoint base on last processed
                if (lastEvent != null)
                {
                    await UpdateCheckpointAsync(partitionId, lastEvent.Offset, lastEvent.SequenceNumber, cancellationToken);
                }
            }
            catch (Exception)
            {
                writeLog(

                    $"Error during the processing of events (num. of events in this batch {events.Count()}) occurred on partition {partition?.PartitionId}."
                );
            }
        }

        private void ShowNotification(string title, string content)
        {
            // Create the notification content
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));

            // Create the notification
            ToastNotification toast = new ToastNotification(toastXml);

            // Show the notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
