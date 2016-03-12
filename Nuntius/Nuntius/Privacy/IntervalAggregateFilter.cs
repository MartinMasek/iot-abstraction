using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuntius.Privacy
{
    /// <summary>
    /// A filter that waits a given time period after receiving a message. Aggregates all
    /// messages in that interval and sends the result further. It basically executes the left fold
    /// function until the time is up and then sends the result.
    /// </summary>
    /// <typeparam name="TResult">Result of aggregation.</typeparam>
    public class IntervalAggregateFilter<TResult> : EventSourceBase, IEventPropagator
    {
        private readonly Func<NuntiusMessage, TResult, TResult> _aggregateFunction;
        private readonly TResult _initialValue;
        private TResult _actualAccumulatedValue;
        private readonly Func<TResult, NuntiusMessage> _resultToMessageMapping;
        private readonly int _intervalInMilliseconds;
        private readonly object _lock = new object();
        private bool _messageInIntervalReceived;
        private CancellationTokenSource _sendTaskToken = new CancellationTokenSource();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="aggregateFunction">Function used to compute the result from the new incoming message and
        /// so far accumulated result from previous messages in the given time interval.</param>
        /// <param name="initialValue">Initial value passed to the first arrived message.</param>
        /// <param name="resultToMessageMapping">Mapping function from the result to the output message.</param>
        /// <param name="intervalInSeconds">Interval in milliseconds. Must be at least 100.</param>
        public IntervalAggregateFilter(Func<NuntiusMessage, TResult, TResult> aggregateFunction,
            TResult initialValue, Func<TResult, NuntiusMessage> resultToMessageMapping, int intervalInSeconds)
            : this(aggregateFunction, initialValue, resultToMessageMapping, EventSourceCallbackMonitoringOptions.NotCheckTaskException, intervalInSeconds)
        { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="aggregateFunction">Function used to compute the result from the new incoming message and
        /// so far accumulated result from previous messages in the given time interval.</param>
        /// <param name="initialValue">Initial value passed to the first arrived message.</param>
        /// <param name="resultToMessageMapping">Mapping function from the result to the output message.</param>
        /// <param name="options">How to behave when invoking <see cref="EventSourceBase.Send"/> callbacks.</param>
        /// <param name="intervalInMilliseconds">Interval in milliseconds. Must be at least 100.</param>
        public IntervalAggregateFilter(Func<NuntiusMessage, TResult, TResult> aggregateFunction,
            TResult initialValue, Func<TResult, NuntiusMessage> resultToMessageMapping, EventSourceCallbackMonitoringOptions options,
            int intervalInMilliseconds)
            : base(options)
        {
            if (intervalInMilliseconds < 100) throw new ArgumentException($"{nameof(intervalInMilliseconds)} must be at least 100 ms.");
            _aggregateFunction = aggregateFunction;
            _initialValue = initialValue;
            _resultToMessageMapping = resultToMessageMapping;
            _intervalInMilliseconds = intervalInMilliseconds;
            _actualAccumulatedValue = _initialValue;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(_intervalInMilliseconds);
                    lock (_lock)
                    {
                        if (_messageInIntervalReceived)
                        {
                            var msg = _resultToMessageMapping(_actualAccumulatedValue);
                            if (_sendTaskToken.IsCancellationRequested) break;
                            SafelyInvokeSendEvent(msg);
                            _messageInIntervalReceived = false;
                            _actualAccumulatedValue = _initialValue;
                        }
                    }
                    if (_sendTaskToken.IsCancellationRequested) break;
                }
            }, _sendTaskToken.Token);
        }

        /// <summary>
        /// Returns a task which represents message processing by the event target.
        /// </summary>
        /// <param name="message">Message to process.</param>
        /// <returns>Task which represents message processing.</returns>
        public Task ProcessMessage(NuntiusMessage message)
        {
            lock (_lock)
            {
                if (!_messageInIntervalReceived) _messageInIntervalReceived = true;
                _actualAccumulatedValue = _aggregateFunction(message, _actualAccumulatedValue);
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Callback which is called when no more messages are generated by the event source.
        /// </summary>
        public void EndProcessing()
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    _sendTaskToken.Cancel();
                }
                SafelyInvokeEndEvent();
            }).ContinueWith(t =>
            {
                var nuntiusException = new NuntiusCommunicationException(null, this,
                    CommunicationExceptionOrigin.EndHandler,
                    new ReadOnlyCollection<Exception>(new List<Exception>() { t.Exception }));
                NuntiusConfiguration.DistributeException(nuntiusException);
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
