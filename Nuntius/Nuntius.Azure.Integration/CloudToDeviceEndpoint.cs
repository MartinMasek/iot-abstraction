using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Nuntius.Azure.Integration
{
    /// <summary>
    /// Represents an endpoint which transforms message from event hub back to nuntius messages.
    /// </summary>
    public class CloudToDeviceEndpoint : BaseDeviceSourceEndpoint
    {
        private readonly string _endpointId;
        private readonly string _iotHubConnectionString;
        private readonly string _iotHubDeviceToCloudEndpoint = "messages/events";
        private EventHubClient _eventHubClient;

        /// <summary>
        /// Creates a new instance which consumes messages from iot hub.
        /// </summary>
        /// <param name="endpointId">Id of the <see cref="DeviceToCloudEndpoint"/> producing the messages.</param>
        /// <param name="iotHubConnectionString">Iot hub connection string.</param>
        public CloudToDeviceEndpoint(string endpointId, string iotHubConnectionString)
        {
            if (String.IsNullOrEmpty(iotHubConnectionString)) throw new ArgumentException($"Argument {nameof(iotHubConnectionString)} must be non empty string.");
            if (String.IsNullOrEmpty(endpointId)) throw new ArgumentException($"Argument {nameof(endpointId)} must be non empty string.");
            _endpointId = endpointId;
            _iotHubConnectionString = iotHubConnectionString;
            _eventHubClient = EventHubClient.CreateFromConnectionString(iotHubConnectionString, _iotHubDeviceToCloudEndpoint);
        }

        private async void ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = _eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null || !IsThisEndpointMessage(eventData)) continue;
                var message = MapEventDataToNuntiusMessage(eventData);
                SafelyInvokeSendEvent(message);
            }
        }

        public void Initialize()
        {
            var d2cPartitions = _eventHubClient.GetRuntimeInformation().PartitionIds;
            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
        }

        private bool IsThisEndpointMessage(EventData eventData)
        {
            object messageId;
            if (eventData.SystemProperties.TryGetValue("message-id", out messageId))
            {
                return ((string)messageId) == _endpointId;
            }
            return false;
        }

        private NuntiusMessage MapEventDataToNuntiusMessage(EventData eventData)
        {
            var message = new NuntiusMessage();
            foreach (var keyPair in eventData.Properties)
            {
                message[keyPair.Key] = keyPair.Value.ToString();
            }
            return message;
        }
    }
}
