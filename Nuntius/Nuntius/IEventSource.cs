using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents a source of <see cref="NuntiusMessage"/>.
    /// </summary>
    public interface IEventSource
    {
        /// <summary>
        /// Fired when a new message is sent.
        /// </summary>
        event Func<NuntiusMessage,Task> Send;

        /// <summary>
        /// Fired when the event source signalizes it will not send any more messages. Handlers of this event should be
        /// short and simple. Any non trivial action should be executed as an asynchronous operation.
        /// </summary>
        event Action End;
    }
}
