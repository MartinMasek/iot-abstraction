using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuntius.Privacy;
using NUnit.Framework;

namespace Nuntius.Tests.Privacy
{
    class LastFilterTests
    {
        [TestFixture]
        public class When_three_messages_are_sent_in_one_interval
        {
            [Test]
            public void Then_only_last_is_sent()
            {
                var result = new List<string>();
                var filter = new LastFilter(30);
                filter.Send += m =>
                {
                    result.Add(m["x"]);
                    return Task.FromResult<object>(null);
                };
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "1" } } });
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "2" } } });
                Task.Delay(10).Wait();
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "3" } } });
                Task.Delay(30).Wait();
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("3", result[0]);
            }
        }

        [TestFixture]
        public class When_three_messages_in_two_intervals_are_sent
        {
            [Test]
            public void Then_two_proper_messages_are_sent()
            {
                var result = new List<string>();
                var filter = new LastFilter(30);
                filter.Send += m =>
                {
                    result.Add(m["x"]);
                    return Task.FromResult<object>(null);
                };
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "1" } } });
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "2" } } });
                Task.Delay(40).Wait();
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "3" } } });
                Task.Delay(30).Wait();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("2", result[0]);
                Assert.AreEqual("3", result[1]);
            }
        }

        [TestFixture]
        public class When_one_message_is_sent 
        {
            [Test]
            public void Then_exactly_one_message_is_sent()
            {
                var result = new List<string>();
                var filter = new LastFilter(10);
                filter.Send += m =>
                {
                    result.Add(m["x"]);
                    return Task.FromResult<object>(null);
                };
                filter.ProcessMessage(new NuntiusMessage() { Properties = { { "x", "1" } } });
                Task.Delay(30).Wait();
                Assert.AreEqual(1, result.Count);
            } 
        }
    }
}
