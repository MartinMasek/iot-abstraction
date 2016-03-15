using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuntius.Privacy;
using NUnit.Framework;

namespace Nuntius.Tests.Privacy
{
    class IntervalAggregateFilterTests
    {
        [TestFixture]
        public class When_messages_are_passed_to_two_intervals
        {
            [Test]
            public void Then_proper_results_are_returned()
            {
                var results = new List<string>(2);

                var filter = new IntervalAggregateFilter<int>(
                    (m, acu) => int.Parse(m["number"]) + acu,
                    0, acu => new NuntiusMessage() { Properties = { { "number", acu.ToString() } } }, 1000);
                filter.Send += m =>
                {
                    results.Add(m["number"]);
                    return Task.FromResult<object>(null);
                };

                NuntiusConfiguration.Exception += e =>
                {
                    throw e;
                };

                filter.ProcessMessage(new NuntiusMessage()
                {
                    Properties = { { "number", "10" } }
                });
                filter.ProcessMessage(new NuntiusMessage()
                {
                    Properties = { { "number", "20" } }
                });
                Task.Delay(1100).Wait();
                filter.ProcessMessage(new NuntiusMessage()
                {
                    Properties = { { "number", "5" } }
                });
                Task.Delay(1100).Wait();

                Assert.AreEqual("30", results[0]);
                Assert.AreEqual("5", results[1]);
            }
        }
    }
}
