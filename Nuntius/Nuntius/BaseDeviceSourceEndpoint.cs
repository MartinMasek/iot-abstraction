namespace Nuntius
{
    /// <summary>
    /// Represents base implementation of <see cref="IDeviceSourceEndpoint"/>
    /// </summary>
    public class BaseDeviceSourceEndpoint : EventSourceBase, IDeviceSourceEndpoint
    {
        /// <summary>
        /// Sends a new message. 
        /// </summary>
        /// <param name="message">Message to send.</param>
        public virtual void SendMessage(NuntiusMessage message)
        {
            SafelyInvokeSendEvent(message);
        }

        /// <summary>
        /// Signalizes no more messages are going to be sent.
        /// </summary>
        public virtual void EndSending()
        {
            SafelyInvokeEndEvent();
        }
        
        /// <summary>
        /// Safely invokes <see cref="EndSending"/> method.
        /// </summary>
        public void Dispose()
        {
            EndSending();
        }
    }
}
