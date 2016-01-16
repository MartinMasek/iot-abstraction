using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Signalizes how should the framework behave when an exception is thrown during message sending.
    /// </summary>
    public enum CommunicationExceptionStrategy
    {
        /// <summary>
        /// Indicates that the source which caught the exception should continue in its flow when receiving next message.
        /// </summary>
        ContinueFlow,
        /// <summary>
        /// Indicates that the source which caught the exception should stop its flow and signal <see cref="IEventSource.End"/> 
        /// event and not send any more messages (though it can receive messages).
        /// </summary>
        StopFlow
    }
}
