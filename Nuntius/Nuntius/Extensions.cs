using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    public static class Extensions
    {
        public static IEventPropagator LinkTo(this IEventPropagator source, IEventPropagator target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        public static IEventTarget LinkTo(this IEventPropagator source, IEventTarget target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        public static IEventPropagator LinkTo(this IEventSource source, IEventPropagator target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

        public static IEventTarget LinkTo(this IEventSource source, IEventTarget target)
        {
            source.Send += target.ProcessMessage;
            AddUnsubscribeFromSourceHandler(source, target);
            return target;
        }

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
