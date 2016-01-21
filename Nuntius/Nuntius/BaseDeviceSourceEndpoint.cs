namespace Nuntius
{
    /// <summary>
    /// Represents base implementation of <see cref="IDeviceSourceEndpoint"/>
    /// </summary>
    public class BaseDeviceSourceEndpoint : EventSourceBase, IDeviceSourceEndpoint
    {
        public void SendMessage(NuntiusMessage message)
        {
            SafelyInvokeSendEvent(message);
        }

        public void EndSending()
        {
            SafelyInvokeEndEvent();
        }
    }
}
