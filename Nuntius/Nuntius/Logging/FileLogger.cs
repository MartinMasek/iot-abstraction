using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Logging
{
    class FileLogger : ILogger,IDisposable
    {
        private readonly string _filePath;
        private StreamWriter writer;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(filePath)) File.Create(filePath);
            writer = new StreamWriter(_filePath);
        }

        public void Log(NuntiusMessage message)
        {
            writer.WriteLine($"Message: {message} {Environment.NewLine}");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
