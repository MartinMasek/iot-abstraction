namespace Nuntius
{
    /// <summary>
    /// Represents base implementation of <see cref="IDeviceSourceEndpoint"/>
    /// </summary>
    public class BaseDeviceSourceEndpoint : EventSourceBase, IDeviceSourceEndpoint
    {
        public virtual void SendMessage(NuntiusMessage message)
        {
            SafelyInvokeSendEvent(message);
        }

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
