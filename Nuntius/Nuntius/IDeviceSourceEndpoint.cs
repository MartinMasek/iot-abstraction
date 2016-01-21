namespace Nuntius
{
    /// <summary>
    /// Represents a device endpoint which can be used as a start for flow in Nuntius framework. Devices generate
    /// messages which can be transformed by other components and passed further.
    /// </summary>
    public interface IDeviceSourceEndpoint : IEventSource
    {
        /// <summary>
        /// Sends a new message. 
        /// </summary>
        /// <param name="message">Message to send.</param>
        void SendMessage(NuntiusMessage message);

        /// <summary>
        /// Signalizes no more messages
        /// </summary>
        void EndSending();
    }
}
