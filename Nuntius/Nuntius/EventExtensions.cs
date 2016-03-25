using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Adds extensions to <see cref="IEventPropagator"/> and <see cref="IEventSource"/>.
    /// </summary>
    public static class EventExtensions
    {

        /// <summary>
        /// Adds listener to <see cref="IEventPropagator"/> Send event and returns the listener.
        /// </summary>
        /// <param name="source">Source to which add the listener.</param>
        /// <param name="target">Target to which link the source. This is returned after linking.</param>
        /// <returns>The target of linking.</returns>
        public static IEventPropagator LinkTo(this IEventPropagator source, IEventPropagator target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        /// <summary>
        /// Adds listener to <see cref="IEventPropagator"/> Send event and returns the listener.
        /// </summary>
        /// <param name="source">Source to which add the listener.</param>
        /// <param name="target">Target to which link the source. This is returned after linking.</param>
        /// <returns>The target of linking.</returns>
        public static IEventTarget LinkTo(this IEventPropagator source, IEventTarget target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        /// <summary>
        /// Adds listener to <see cref="IEventSource"/> Send event and returns the listener.
        /// </summary>
        /// <param name="source">Source to which add the listener.</param>
        /// <param name="target">Target to which link the source. This is returned after linking.</param>
        /// <returns>The target of linking.</returns>
        public static IEventPropagator LinkTo(this IEventSource source, IEventPropagator target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        /// <summary>
        /// Adds listener to<see cref="IEventSource"/> Send event and returns the listener.
        /// </summary>
        /// <param name="source">Source to which add the listener.</param>
        /// <param name="target">Target to which link the source. This is returned after linking.</param>
        /// <returns>The target of linking.</returns>
        public static IEventTarget LinkTo(this IEventSource source, IEventTarget target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        /// <summary>
        /// Adds listener to <see cref="IEventSource"/> Send event and returns the listener.
        /// </summary>
        /// <param name="source">Source to which add the listener.</param>
        /// <param name="terminalAction">A lambda method which is linked to the source's send event.</param>
        public static void LinkTo(this IEventSource source, Func<NuntiusMessage, Task> terminalAction)
        {
            source.Send += terminalAction;
            source.End += () => source.Send -= terminalAction;
        }

        private static void AddUnsubscribeFromSourceHandler(IEventSource source, IEventTarget target)
        {
            source.End += () =>
            {
                source.Send -= target.ProcessMessage;
                target.EndProcessing();
            };
        }
    }
}
