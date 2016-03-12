using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents base implementation of <see cref="IDeviceSourceEndpoint"/> which periodically sends
    /// a message.
    /// </summary>
    public abstract class PeriodicEventDeviceSource : BaseDeviceSourceEndpoint
    {
        private readonly int _intervalInMiliseconds;
        /// <summary>
        /// Indicates whether the messages should be send. Once it is false the messages stop being sent.
        /// </summary>
        private bool _shouldSendMessages = true;
        private object _shouldSendMessagesLock = new object();

        /// <summary>
        /// Starts a task which periodically sends messages.
        /// </summary>
        /// <returns></returns>
        public Task StartSending()
        {
            return Task.Factory.StartNew(async () =>
            {
                while (_shouldSendMessages)
                {
                    await Task.Delay(_intervalInMiliseconds);
                    lock (_shouldSendMessagesLock)
                    {
                        SendMessage(GetNextMessage());
                    }
                }
            });
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="intervalInMiliseconds">This is an interval in milliseconds before sending a message again. 
        /// It must be at least 10 ms.</param>
        protected PeriodicEventDeviceSource(int intervalInMiliseconds)
        {
            if (intervalInMiliseconds < 10)
                throw new ArgumentException($"{nameof(intervalInMiliseconds)} must be greater or equal 10.");
            _intervalInMiliseconds = intervalInMiliseconds;
        }

        /// <summary>
        /// Function which is periodically called and its message is send.
        /// </summary>
        /// <returns></returns>
        protected abstract NuntiusMessage GetNextMessage();

        public override void EndSending()
        {
            lock (_shouldSendMessagesLock)
            {
                _shouldSendMessages = false;
            }
            base.EndSending();
        }
    }
}
