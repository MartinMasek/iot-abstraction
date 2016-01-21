using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Privacy
{
    /// <summary>
    /// Represents an interface used in k-anonymity algorithm. Instance of a class implementing
    /// this interface is offered an element and should decide whether there is enough number of elements
    /// (at least K in total).
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public interface IKAnonymitySet<TElement>
    {
        /// <summary>
        /// Id of the set.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Adds a new element to the set. Returns true if there are at least K elements
        /// in the set (including the added one). This method must be thread safe.
        /// </summary>
        /// <param name="element">Element to add.</param>
        /// <returns>True if there are at least K elements in the set (including the added one).</returns>
        bool AddToSet(TElement element);
    }
}
