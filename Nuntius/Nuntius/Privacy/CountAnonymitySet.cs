using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuntius.Privacy
{
    /// <summary>
    /// This set has a counter representing messages offered. Each time a message is offered, the counter is incremented
    /// and a task, which decrements the counter after a period, is queued. So for example say the period is 1000 ms
    /// and a message is offered at 100th, 200th and 500th ms. After a message is added at 1201th millisecond, 
    /// the counter is 2. The <see cref="OfferMessage"/> method can be overridden.
    /// </summary>
    public class CountAnonymitySet : IKAnonymitySet
    {
        private readonly int _k;
        private readonly int _lifespanInMilliseconds;
        private readonly Func<CountAnonymitySet, NuntiusMessage> _setToMessage;
        private int _occurences;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="setId">Set id.</param>
        /// <param name="k">A threshold. If the number of messages after adding is greater or equal k, 
        /// <see cref="setToMessage"/> is called.</param>
        /// <param name="lifespanInMilliseconds">Lifespan of an element. After this lifespan the counter is decreased
        /// This should be optimally larger then time between <see cref="OfferMessage"/> calls so this class
        /// doesn't throttle.</param>
        /// <param name="setToMessage">This is a mapping function called once the counter is at least k.</param>
        public CountAnonymitySet(int setId, int k, int lifespanInMilliseconds, Func<CountAnonymitySet, NuntiusMessage> setToMessage)
        {
            if (k < 1) throw new ArgumentException($"{nameof(k)} must be positive.");
            if (lifespanInMilliseconds < 1)
                throw new ArgumentException($"{nameof(lifespanInMilliseconds)} must be positive.");
            if (setToMessage == null) throw new ArgumentNullException($"{nameof(setToMessage)} must not be null.");

            Id = setId;
            _k = k;
            _lifespanInMilliseconds = lifespanInMilliseconds;
            _setToMessage = setToMessage;
        }

        public int Id { get; }

        /// <summary>
        /// Number of messages which arrived from this point to the past and are maximum
        /// <see cref="_lifespanInMilliseconds"/> old.
        /// </summary>
        public int Count => _occurences;

        /// <summary>
        /// Offers the message to the set which should behave appropriately. If the set if not in k-anonymity
        /// state with the offered message, it should return null; otherwise appropriate output message should
        /// be returned. This method is called from multiple threads.
        /// </summary>
        /// <param name="inputMessage">Message offered to the set.</param>
        /// <returns></returns>
        public virtual NuntiusMessage OfferMessage(NuntiusMessage inputMessage)
        {
            var occurences = Interlocked.Add(ref _occurences, 1);
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(_lifespanInMilliseconds);
                Interlocked.Add(ref _occurences, -1);
            });
            if (occurences >= _k) return _setToMessage(this);
            return null;
        }
    }
}