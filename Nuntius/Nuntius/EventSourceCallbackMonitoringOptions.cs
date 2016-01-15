using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Tells the event source how it should behave when calling <see cref="IEventSource.Send"/> handler.
    /// </summary>
    public enum EventSourceCallbackMonitoringOptions
    {
        /// <summary>
        /// Indicates the <see cref="IEventSource"/> will not check whether task returned by the callback contains exception.
        /// </summary>
        NotCheckTaskException,
        /// <summary>
        /// Indicates the <see cref="IEventSource"/> will check whether task returned by the callback contains exception.
        /// </summary>
        CheckTaskException
    }
}
