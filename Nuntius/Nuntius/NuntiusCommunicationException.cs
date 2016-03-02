using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents an exception which is throw during event handling between <see cref="IEventSource"/> and
    /// <see cref="IEventTarget"/>.
    /// </summary>
    public class NuntiusCommunicationException : Exception
    {
        public NuntiusCommunicationException() { }

        public NuntiusCommunicationException(string message) : base(message) { }

        public NuntiusCommunicationException(NuntiusMessage messageCausingException,
            IEventSource exceptionCatcher,
            CommunicationExceptionOrigin origin,
            ReadOnlyCollection<Exception> innerExceptions)
        {
            MessageCausingException = messageCausingException;
            ExceptionCatcher = exceptionCatcher;
            InnerExceptions = innerExceptions;
            ExceptionOrigin = origin;
        }

        /// <summary>
        /// Holds message after which the exception occured. Can be null (for example in a case when
        /// exception was caused during <see cref="IEventSource.End"/> event.
        /// </summary>
        public NuntiusMessage MessageCausingException { get; private set; }
        /// <summary>
        /// Who caught the exception.
        /// </summary>
        public IEventSource ExceptionCatcher { get; private set; }
        /// <summary>
        /// Original exceptions(s) which has been thrown.
        /// </summary>
        public ReadOnlyCollection<Exception> InnerExceptions { get; private set; }
        /// <summary>
        /// More info about exception origin.
        /// </summary>
        public CommunicationExceptionOrigin ExceptionOrigin { get; private set; }
    }

    /// <summary>
    /// Adds additional info about exception origin.
    /// </summary>
    public enum CommunicationExceptionOrigin
    {
        /// <summary>
        /// Exception has been thrown when <see cref="IEventSource"/> fired its <see cref="IEventSource.Send"/> event and
        /// the exception was thrown inside the callee handle method.
        /// </summary>
        ProcessHandler,
        /// <summary>
        /// Exception has been thrown when waiting for result of the task returned by <see cref="IEventSource.Send"/> 
        /// method.
        /// </summary>
        ProcessTask,
        /// <summary>
        /// Exception has been thrown when <see cref="IEventSource"/> fired its <see cref="IEventSource.End"/> event and the
        /// exception was thrown inside the callee handle method.
        /// method.
        /// </summary>
        EndHandler
    }
}
