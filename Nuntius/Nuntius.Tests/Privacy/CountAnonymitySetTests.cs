using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuntius.Privacy;
using NUnit.Framework;

namespace Nuntius.Tests.Privacy
{
    class CountAnonymitySetTests
    {
        public class When_all_events_are_under_first_lifspan
        {
            [TestFixture]
            public class And_number_of_elements_is_larger_than_k
            {
                [Test]
                public void Then_message_is_returned()
                {
                    var set = new CountAnonymitySet(1, 4, 100, x => new NuntiusMessage()
                    {
                        Properties = { { "k", x.ToString() } }
                    });
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(10).Wait();
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(10).Wait();
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(10).Wait();
                    Assert.AreEqual("4", set.OfferMessage(null).Properties["k"]);
                    Task.Delay(10).Wait();
                    Assert.AreEqual("5", set.OfferMessage(null).Properties["k"]);
                }
            }
        }

        public class And_last_element_is_after_first_lifespam
        {
            [TestFixture]
            public class And_number_of_elements_is_lesser_than_k
            {
                [Test]
                public void Then_null_is_returned()
                {
                    var set = new CountAnonymitySet(1, 4, 100, x => new NuntiusMessage()
                    {
                        Properties = { { "k", x.ToString() } }
                    });
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(20).Wait();
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(20).Wait();
                    Assert.AreEqual(null, set.OfferMessage(null));
                    Task.Delay(10).Wait();
                    Assert.AreEqual("4", set.OfferMessage(null)["k"]);
                    Task.Delay(10).Wait();
                    Assert.AreEqual("5", set.OfferMessage(null)["k"]);
                    Task.Delay(45).Wait();
                    Assert.AreEqual("4", set.OfferMessage(null)["k"]);
                    Task.Delay(50).Wait();
                    Assert.AreEqual(null, set.OfferMessage(null));
                }
            }
        }
    }
}
