# Event Hub Listener App

Simple UWP application to test Azure Event Hub and check if you're receiving data.

## Features

- **Real-time Notifications**: This application shows a toast notification every time you receive an event. Very useful when you want to know in real-time when a message is received.
- **Event Hub Listener**: Connect to an Azure Event Hub and listen for incoming events.
- **Checkpointing**: Uses Azure Blob Storage for checkpointing to ensure reliable event processing.

## Getting Started

### Prerequisites

- Visual Studio 2019 or later
- Azure Subscription
- Azure Event Hub
- Azure Blob Storage

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/event-hub-listener-app.git
   ```
2. Open the solution file `EventHubListenerApp.sln` in Visual Studio.
3. Restore the NuGet packages.
4. Build the solution.

### Configuration

1. Open the `MainPage.xaml.cs` file.
2. Update the following fields with your Azure Event Hub and Blob Storage details:
   ```csharp
   this.eventHubConnectionString.Text = "Your Event Hub Connection String";
   this.eventHubName.Text = "Your Event Hub Name";
   this.eventHubConsumerGroup.Text = "Your Event Hub Consumer Group";
   this.blobStorageConnectionString.Text = "Your Blob Storage Connection String";
   this.blobStorageContainerName.Text = "Your Blob Storage Container Name";
   ```

### Running the Application

1. Start the application from Visual Studio.
2. Enter your Azure Event Hub and Blob Storage details in the provided fields.
3. Click the "Start listener" button to begin listening for events.
4. Click the "Stop listener" button to stop listening for events.

## License

This project is licensed under the GNU Affero General Public License - see the [LICENSE.txt](LICENSE.txt) file for details.
