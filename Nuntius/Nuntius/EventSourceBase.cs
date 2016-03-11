using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// This class provides helper methods for <see cref="IEventSource"/> interface. This class enables safe invocations
    /// of <see cref="IEventSource.Send"/> and <see cref="IEventSource.End"/> events with automatic exception handling.
    /// </summary>
    public class EventSourceBase : IEventSource
    {
        protected readonly EventSourceCallbackMonitoringOptions MonitoringOption;

        /// <summary>
        /// Creates a new instance with <see cref="EventSourceCallbackMonitoringOptions.NotCheckTaskException"/> as default option.
        /// </summary>
        protected EventSourceBase() : this(EventSourceCallbackMonitoringOptions.NotCheckTaskException)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="monitoringOption">Configuration regarding checking of tasks returned by <see cref="Send"/> event
        /// handlers.</param>
        protected EventSourceBase(EventSourceCallbackMonitoringOptions monitoringOption)
        {
            MonitoringOption = monitoringOption;
        }

        public event Func<NuntiusMessage, Task> Send;
        public event Action End;

        /// <summary>
        /// Safely invokes <see cref="Send"/> event and handles and properly distributes any exceptions thrown in the process.
        /// </summary>
        /// <param name="message">Message to pass to the <see cref="IEventSource.Send"/> event.</param>
        protected void SafelyInvokeSendEvent(NuntiusMessage message)
        {
            try
            {
                switch (MonitoringOption)
                {
                    case EventSourceCallbackMonitoringOptions.NotCheckTaskException:
                        Send?.Invoke(message);
                        break;
                    case EventSourceCallbackMonitoringOptions.CheckTaskException:
                        if (Send != null)
                        {
                            foreach (Func<NuntiusMessage, Task> d in Send?.GetInvocationList())
                            {
                                d(message).ContinueWith(t =>
                                {
                                    var ex = new NuntiusCommunicationException(message, this, CommunicationExceptionOrigin.ProcessTask,
                                        t.Exception?.InnerExceptions);
                                    NuntiusConfiguration.DistributeException(ex);
                                    switch (NuntiusConfiguration.CommunicationExceptionStrategy)
                                    {
                                        case CommunicationExceptionStrategy.StopFlow:
                                            SafelyInvokeEndEvent();
                                            break;
                                        case CommunicationExceptionStrategy.ContinueFlow:
                                            break;
                                        default:
                                            throw new NotImplementedException($"Strategy for {NuntiusConfiguration.CommunicationExceptionStrategy} was not implemented.");
                                    }
                                }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Strategy for {MonitoringOption} was not implemented.");
                }
            }
            catch (Exception e)
            {
                var ex = new NuntiusCommunicationException(message, this, CommunicationExceptionOrigin.ProcessHandler,
                    new ReadOnlyCollection<Exception>(new List<Exception>() { e }));
                NuntiusConfiguration.DistributeException(ex);
                switch (NuntiusConfiguration.CommunicationExceptionStrategy)
                {
                    case CommunicationExceptionStrategy.StopFlow:
                        SafelyInvokeEndEvent();
                        break;
                    case CommunicationExceptionStrategy.ContinueFlow:
                        break;
                    default:
                        throw new NotImplementedException($"Strategy for {NuntiusConfiguration.CommunicationExceptionStrategy} was not implemented.");
                }
            }
        }

        /// <summary>
        /// Safely invokes <see cref="End"/> event and handles and properly distributes any exception thrown in the process.
        /// </summary>
        protected void SafelyInvokeEndEvent()
        {
            try
            {
                End?.Invoke();
                End = null;
            }
            catch (Exception e)
            {
                var nuntiusException = new NuntiusCommunicationException(null, this,
                    CommunicationExceptionOrigin.EndHandler,
                    new ReadOnlyCollection<Exception>(new List<Exception>() { e }));
                NuntiusConfiguration.DistributeException(nuntiusException);
            }
        }
    }
}
