using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents base implementation of <see cref="IDeviceSourceEndpoint"/> which periodically sends
    /// a message.
    /// </summary>
    class PeriodicEventDeviceSource : BaseDeviceSourceEndpoint
    {
        private readonly int _intervalInMiliseconds;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="intervalInMiliseconds">This is an interval in milliseconds before sending a message again. 
        /// It must be at least 10 ms.</param>
        /// <param name="getMessage">Function that retrieves a message from this object.</param>
        public PeriodicEventDeviceSource(int intervalInMiliseconds, Func<PeriodicEventDeviceSource, NuntiusMessage> getMessage)
        {
            if (intervalInMiliseconds < 10)
                throw new ArgumentException($"{nameof(intervalInMiliseconds)} must be greater or equal 10.");
            if (getMessage == null) throw new ArgumentNullException($"{nameof(intervalInMiliseconds)} must not be null.");
            _intervalInMiliseconds = intervalInMiliseconds;
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(_intervalInMiliseconds);
                SendMessage(getMessage(this));
            });
        }
    }
}
