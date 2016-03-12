using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Logging
{
    /// <summary>
    /// This class provides extensions used for logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs a message using a given <see cref="ILogger"/>.
        /// </summary>
        /// <param name="source">Source generating messages.</param>
        /// <param name="logger">Logger used to log messages.</param>
        /// <returns>The original source which generates message.</returns>
        public static IEventPropagator Log(this IEventPropagator source, ILogger logger)
        {
            source.Send += message =>
            {
                logger.Log(message);
                return Task.FromResult<object>(null);
            };
            source.End += logger.EndLogging;
            return source;
        }

        /// <summary>
        /// Logs a message using <see cref="FileLogger"/>.
        /// </summary>
        /// <param name="source">Source generating messages.</param>
        /// <param name="filePath">Path to the file where the logger should output messages. File is overridden or created
        /// if it does not exist.</param>
        /// <returns>The original source which generates message.</returns>
        public static IEventPropagator LogToFile(this IEventPropagator source, string filePath)
        {
            var logger = new FileLogger(filePath);
            return Log(source, logger);
        }

        /// <summary>
        /// Logs a message using <see cref="FileLogger"/>.
        /// </summary>
        /// <param name="source">Source generating messages.</param>
        /// <param name="filePath">Path to the file where the logger should output messages. File is overridden or created
        /// if it does not exist.</param>
        /// <param name="append">True if the new content should be appended to the file.</param>
        /// <returns>The original source which generates message.</returns>
        public static IEventPropagator LogToFile(this IEventPropagator source, string filePath, bool append)
        {
            var logger = new FileLogger(filePath, append);
            return Log(source, logger);
        }

        /// <summary>
        /// Logs a message using <see cref="FileLogger"/>.
        /// </summary>
        /// <param name="source">Source generating messages.</param>
        /// <param name="filePath">Path to the file where the logger should output messages. File is overridden or created
        /// if it does not exist.</param>
        /// <param name="append">True if the new content should be appended to the file.</param>
        /// <param name="messageToString">Function converting a message to a string which is then logged.</param>
        /// <returns>The original source which generates message.</returns>
        public static IEventPropagator LogToFile(this IEventPropagator source, string filePath, bool append,
            Func<NuntiusMessage, string> messageToString)
        {
            var logger = new FileLogger(filePath, append, messageToString);
            return Log(source, logger);
        }
    }
}
