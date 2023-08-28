using EventHubTestApp.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace EventHubTestApp
{
    /// <summary>
    /// Page that handle the event hub listener
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MainPage()
        {
            this.InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();

            History history = GetLastRun();
            this.eventHubConnectionString.Text = history.EventHubConnectionString;
            this.eventHubName.Text = history.EventHubName;
            this.eventHubConsumerGroup.Text = history.EventHubConsumerGroup;
            this.blobStorageConnectionString.Text = history.CheckpointConnectionString;
            this.blobStorageContainerName.Text = history.CheckpointContainerName;
        }

        /// <summary>
        /// Start listener logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.eventHubConnectionString.IsEnabled = false;
            this.eventHubName.IsEnabled = false;
            this.eventHubConsumerGroup.IsEnabled = false;
            this.blobStorageConnectionString.IsEnabled = false;
            this.blobStorageContainerName.IsEnabled = false;
            this.btnStart.IsEnabled = false;
            this.btnStop.IsEnabled = true;

            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            History history = new History
            {
                EventHubConnectionString = eventHubConnectionString.Text,
                EventHubName = eventHubName.Text,
                EventHubConsumerGroup = eventHubConsumerGroup.Text,
                CheckpointConnectionString = blobStorageConnectionString.Text,
                CheckpointContainerName = blobStorageContainerName.Text
            };

            _ = EventHubManager.RunEventHubListener(cancellationToken,
                eventHubConnectionString.Text, eventHubName.Text, eventHubConsumerGroup.Text,
                blobStorageConnectionString.Text, blobStorageContainerName.Text,
                async (value) =>
                {
                    // Update the UI with the output (dispatched to UI thread)
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () =>
                        {
                            OutputTextBlock.Text += $"{DateTime.Now.ToLongTimeString()}: {value}{Environment.NewLine}";
                            Scroller.ChangeView(null, Scroller.ScrollableHeight, null);
                        }
                    );
                });

            _ = SaveHistory(history);
        }

        /// <summary>
        /// Stop logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();


            this.eventHubConnectionString.IsEnabled = true;
            this.eventHubName.IsEnabled = true;
            this.eventHubConsumerGroup.IsEnabled = true;
            this.blobStorageConnectionString.IsEnabled = true;
            this.blobStorageContainerName.IsEnabled = true;
            this.btnStart.IsEnabled = true;
            this.btnStop.IsEnabled = false;
        }

        /// <summary>
        /// Save last run configuration
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        private async Task SaveHistory(History history)
        {
            try
            {
                await File.WriteAllTextAsync(".history", JsonSerializer.Serialize(history));
            }
            catch
            {
                Trace.WriteLine("Cannot save the hisotry");
            }
        }

        /// <summary>
        /// retrieve last run configuration
        /// </summary>
        /// <returns></returns>
        private History GetLastRun()
        {
            try
            {
                string content = File.ReadAllText(".history");
                return JsonSerializer.Deserialize<History>(content);
            }
            catch
            {
                return new History();
            }
        }

    }
}
