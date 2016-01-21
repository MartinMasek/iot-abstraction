using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuntius.Privacy
{
    /// <summary>
    /// This set doesn't track elements. Instead it has a counter representing occurrences of elements. Each element
    /// is assigned a lifespan and one the lifespan ends it is removed from the set. So for example say lifespan is 
    /// 1000 ms and element is added at 100th, 200th and 500th. After an element is added at 1201th millisecond number
    /// of elements in the set is 2.
    /// </summary>
    public class CountAnonymitySet : IKAnonymitySet<object>
    {
        private readonly int _k;
        private readonly int _lifespanInMilliseconds;
        private int _occurences;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="setId">Set id.</param>
        /// <param name="k">A threshold. If number of elements after adding is greater or equal k true is returned.</param>
        /// <param name="lifespanInMilliseconds">Lifespan of an element. After this lifespan element is removed from the set. This should be
        /// optimally larger then time between <see cref="AddToSet"/> calls so this class doesn't throttle.</param>
        public CountAnonymitySet(int setId, int k, int lifespanInMilliseconds)
        {
            if (k < 1) throw new ArgumentException($"{nameof(k)} must be positive.");
            if (lifespanInMilliseconds < 1) throw new ArgumentException($"{nameof(lifespanInMilliseconds)} must be positive.");

            Id = setId;
            _k = k;
            _lifespanInMilliseconds = lifespanInMilliseconds;
        }

        /// <summary>
        /// Id of the set.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Adds a new element to the set. Returns true if there are at least K elements
        /// in the set (including the added one). This method must be thread safe.
        /// </summary>
        /// <param name="element">Element to add. Doesn't matter what is the element because the element is never used
        /// nor accessed. Therefore it should be called with null parameter.</param>
        /// <returns>True if there are at least K elements in the set (including the added one).</returns>
        public bool AddToSet(object element)
        {
            var occurences = Interlocked.Add(ref _occurences, 1);
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(_lifespanInMilliseconds);
                Interlocked.Add(ref _occurences, -1);
            });
            return occurences >= _k;
        }
    }
}
