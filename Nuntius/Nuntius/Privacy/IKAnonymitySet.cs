using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Privacy
{
    /// <summary>
    /// Represents an interface used in k-anonymity algorithm. Instance of a class implementing
    /// this interface is offered an element and it should decide whether there is enough number of elements
    /// (at least K in total) and output a message appropriately.
    /// </summary>
    public interface IKAnonymitySet
    {
        /// <summary>
        /// Id of the set. This is used by the k-anonymity algorithm to decide what set to offer
        /// a new message.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Offers the message to the set which should behave appropriately. If the set if not in k-anonymity
        /// state with the offered message, it should return null; otherwise appropriate output message should
        /// be returned. This method is called from multiple threads.
        /// </summary>
        /// <param name="inputMessage">Message offered to the set.</param>
        /// <returns></returns>
        NuntiusMessage OfferMessage(NuntiusMessage inputMessage);
    }
}
