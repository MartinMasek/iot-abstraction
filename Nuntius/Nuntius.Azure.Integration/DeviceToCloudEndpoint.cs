using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Nuntius.Azure.Integration
{
    /// <summary>
    /// Represents an endpoint which transforms nuntius messages to IoT Hub messages and sends them to the
    /// IoT Hub.
    /// </summary>
    public class DeviceToCloudEndpoint : IEventTarget
    {
        private readonly string _endpointId;
        private readonly string _iotHubUri;
        private readonly string _devicePrimaryKey;
        private readonly string _deviceId;
        private readonly DeviceClient _deviceClient;


        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="endpointId">Id of the endpoint is attached to the message so the cloud consumer knows what messages
        /// to propagate. <see cref="CloudToDeviceEndpoint"/> has to have the same id if it should propagate messages
        /// of this object.</param>
        /// <param name="iotHubUri">The fully-qualified DNS hostname of IoT Hub. Usually in the form of xxx.azure-devices.net.</param>
        /// <param name="devicePrimaryKey">Device primary key which was obtained when registering the device.</param>
        /// <param name="deviceId">Device id under which it was registered.</param>
        public DeviceToCloudEndpoint(string endpointId,string iotHubUri, string devicePrimaryKey, string deviceId)
        {
            if (String.IsNullOrEmpty(iotHubUri)) throw new ArgumentException($"Argument {nameof(iotHubUri)} must be non empty string.");
            if (String.IsNullOrEmpty(devicePrimaryKey)) throw new ArgumentException($"Argument {nameof(devicePrimaryKey)} must be non empty string.");
            if (String.IsNullOrEmpty(deviceId)) throw new ArgumentException($"Argument {nameof(deviceId)} must be non empty string.");
            if (String.IsNullOrEmpty(endpointId)) throw new ArgumentException($"Argument {nameof(endpointId)} must be non empty string.");
            _endpointId = endpointId;
            _iotHubUri = iotHubUri;
            _devicePrimaryKey = devicePrimaryKey;
            _deviceId = deviceId;
            _deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, devicePrimaryKey));
        }

        public Task ProcessMessage(NuntiusMessage message)
        {
            var azureMessage = new Message();
            azureMessage.MessageId = _endpointId;
            foreach (var keyValue in message.Properties)
            {
                azureMessage.Properties[keyValue.Key] = keyValue.Value;
            }
            return _deviceClient.SendEventAsync(azureMessage);
        }

        public void EndProcessing()
        {
            _deviceClient.CloseAsync().Wait();
        }
    }
}
