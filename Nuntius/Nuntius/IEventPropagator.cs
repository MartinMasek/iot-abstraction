using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius
{
    /// <summary>
    /// Represents an ability to both generate and consume <see cref="NuntiusMessage"/>
    /// </summary>
    public interface IEventPropagator : IEventSource, IEventTarget
    {
    }
}
