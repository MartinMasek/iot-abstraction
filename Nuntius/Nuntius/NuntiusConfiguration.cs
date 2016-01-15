using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents configuration of Nuntius framework. 
    /// </summary>
    public static class NuntiusConfiguration
    {
        /// <summary>
        /// Asynchronously distributes <see cref="NuntiusCommunicationException"/> to registered handlers.
        /// </summary>
        /// <param name="e">Exception to be distributed.</param>
        public static void DistributeException(NuntiusCommunicationException e)
        {
            Task.Factory.StartNew(() => { Exception?.Invoke(e); });
        }

        /// <summary>
        /// Event raised when an exception is thrown during message sending in Nuntius framework. 
        /// </summary>
        public static event Action<NuntiusCommunicationException> Exception;

        /// <summary>
        /// Communication strategy when exception happens in message flow. Default value is <see cref="CommunicationExceptionStrategy.ContinueFlow"/>.
        /// </summary>
        public static CommunicationExceptionStrategy CommunicationExceptionStrategy { get; set; } = CommunicationExceptionStrategy.ContinueFlow;
    }
}
