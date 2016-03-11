using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Logging
{
    /// <summary>
    /// Represents an ability to log messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Log(NuntiusMessage message);

        /// <summary>
        /// Method called when no more logging should be done.
        /// </summary>
        void EndLogging();
    }
}
