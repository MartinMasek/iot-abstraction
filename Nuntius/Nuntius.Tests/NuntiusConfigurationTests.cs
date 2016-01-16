using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nuntius.Tests
{
    public class NuntiusConfigurationTests
    {
        public class When_DistributeException_is_called
        {
            [TestFixture]
            public class Then_handlers_are_called
            {
                private bool _called;
                private Action<NuntiusCommunicationException> _action;

                [SetUp]
                public void Initialize()
                {
                    _action = x => _called = true;
                    NuntiusConfiguration.Exception += _action;
                }

                [Test]
                public void Then_nothing_happens()
                {
                    NuntiusConfiguration.DistributeException(null);
                    Task.Delay(20).Wait();
                    Assert.AreEqual(true, _called);
                }

                [TearDown]
                public void TearDown()
                {
                    NuntiusConfiguration.Exception -= _action;
                }
            }
        }
    }
}
