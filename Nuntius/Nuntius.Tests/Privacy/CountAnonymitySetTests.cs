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
                public void Then_true_is_returned()
                {
                    var set = new CountAnonymitySet(1, 4, 100);
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(10).Wait();
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(10).Wait();
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(10).Wait();
                    Assert.IsTrue(set.AddToSet(null));
                    Task.Delay(10).Wait();
                    Assert.IsTrue(set.AddToSet(null));
                }
            }
        }

        public class And_last_element_is_after_first_lifespam
        {
            [TestFixture]
            public class And_number_of_elements_is_lesser_than_k
            {
                [Test]
                public void Then_false_is_returned()
                {
                    var set = new CountAnonymitySet(1, 4, 100);
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(20).Wait();
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(20).Wait();
                    Assert.IsFalse(set.AddToSet(null));
                    Task.Delay(20).Wait();
                    Assert.IsTrue(set.AddToSet(null));
                    Task.Delay(20).Wait();
                    Assert.IsTrue(set.AddToSet(null));
                    Task.Delay(20).Wait();
                    Assert.IsTrue(set.AddToSet(null));
                    Task.Delay(50).Wait();
                    Assert.IsFalse(set.AddToSet(null));
                }
            }
        }
    }
}
