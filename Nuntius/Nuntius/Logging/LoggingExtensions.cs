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
        public static IEventPropagator Log(this IEventPropagator source, ILogger logger)
        {
            // TODO: add logging
            return source;
        }

        public static IEventPropagator LogToFile(this IEventPropagator source, string filePath)
        {
            var logger = new FileLogger(filePath);
            source.Send += message =>
            {
                logger.Log(message);
                return Task.FromResult<object>(null);
            };
            source.End += () => logger.Dispose();
            return source;
        }
    }
}
