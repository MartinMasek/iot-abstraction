using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nuntius.Tests
{
    class EventSourceBaseTests
    {
        private class TestFilter : EventSourceBase, IEventPropagator
        {
            private readonly Action<NuntiusMessage> _filterBody;
            private readonly Action<NuntiusMessage> _preTaskBody;
            private readonly Action _endBody;

            public TestFilter(Action<NuntiusMessage> filterBody) : this(filterBody, m => { })
            {
            }

            public TestFilter(EventSourceCallbackMonitoringOptions options, Action<NuntiusMessage> filterBody) : base(options)
            {
                _filterBody = filterBody;
                _preTaskBody = m => { };
            }

            public TestFilter(EventSourceCallbackMonitoringOptions options, Action<NuntiusMessage> filterBody, Action<NuntiusMessage> preTaskBody)
                : base(options)
            {
                _filterBody = filterBody;
                _preTaskBody = preTaskBody;
            }

            public TestFilter(Action<NuntiusMessage> filterBody, Action<NuntiusMessage> preTaskBody)
            {
                _filterBody = filterBody;
                _preTaskBody = preTaskBody;
            }

            public TestFilter(Action<NuntiusMessage> filterBody, Action<NuntiusMessage> preTaskBody, Action endBody)
            {
                _filterBody = filterBody;
                _preTaskBody = preTaskBody;
                _endBody = endBody;
            }

            public Task ProcessMessage(NuntiusMessage message)
            {
                _preTaskBody?.Invoke(message);
                return Task.Factory.StartNew(() =>
                {
                    _filterBody?.Invoke(message);
                    SafelyInvokeSendEvent(message);
                });
            }

            public void EndProcessing()
            {
                _endBody?.Invoke();
                SafelyInvokeEndEvent();
            }
        }

        private static int _handlerCalledCount;
        private static Action<NuntiusCommunicationException> _exceptionHandler;

        private static void Initialize_exception_handler()
        {
            _handlerCalledCount = 0;
            _exceptionHandler = e => Interlocked.Add(ref _handlerCalledCount, 1);
            NuntiusConfiguration.Exception += _exceptionHandler;
        }

        private static void Set_configuration_to_default()
        {
            NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.ContinueFlow;
            NuntiusConfiguration.Exception -= _exceptionHandler;
        }

        public class When_exception_is_thrown_before_task_starts
        {
            [TestFixture]
            public class And_flow_should_continue
            {
                private NuntiusCommunicationException distributedException;
                private Action<NuntiusCommunicationException> exceptionHandler;

                [SetUp]
                public void Initialize()
                {
                    Initialize_exception_handler();
                    NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.ContinueFlow;
                    exceptionHandler = e => distributedException = e;
                    NuntiusConfiguration.Exception += exceptionHandler;
                }

                [Test]
                public void Then_exception_is_registered()
                {
                    List<string> flowResult = new List<string>();
                    TestFilter first = new TestFilter(m => { });
                    TestFilter second = new TestFilter(m => { flowResult.Add(m["data"]); },
                        m => { if (m["data"] == "A") throw new Exception(); });
                    first.LinkTo(second);
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                    Task.Delay(20).Wait();
                    Assert.AreEqual(1, _handlerCalledCount);
                    Task.Delay(20).Wait();
                    Assert.AreEqual(1, flowResult.Count);
                    Assert.AreEqual("B", flowResult[0]);
                }

                [Test]
                public void Then_proper_exception_message_is_returned()
                {
                    TestFilter first = new TestFilter(m => { });
                    TestFilter second = new TestFilter(m => { }, m => { if (m["data"] == "A") throw new Exception(); });
                    first.LinkTo(second);
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                    Task.Delay(50).Wait();
                    Assert.AreEqual(first, distributedException.ExceptionCatcher);
                    Assert.AreEqual(CommunicationExceptionOrigin.ProcessHandler, distributedException.ExceptionOrigin);
                    Assert.AreEqual("A", distributedException.MessageCausingException["data"]);
                }

                [TearDown]
                public void Teardown()
                {
                    Set_configuration_to_default();
                    NuntiusConfiguration.Exception -= exceptionHandler;
                }
            }

            [TestFixture]
            public class And_flow_should_not_continue
            {
                [SetUp]
                public void Initialize()
                {
                    Initialize_exception_handler();
                    NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.StopFlow;
                }

                [Test]
                public void Then_exception_is_registered()
                {
                    List<string> flowResult = new List<string>();
                    TestFilter first = new TestFilter(m => { });
                    TestFilter second = new TestFilter(m => { flowResult.Add(m["data"]); },
                        m => { if (m["data"] == "A") throw new Exception(); });
                    first.LinkTo(second);
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                    Task.Delay(20).Wait();
                    first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                    Task.Delay(20).Wait();
                    Assert.AreEqual(1, _handlerCalledCount);
                    Assert.AreEqual(0, flowResult.Count);
                }

                [TearDown]
                public void Teardown()
                {
                    Set_configuration_to_default();
                }
            }
        }

        public class When_exception_is_thrown_in_task
        {
            [TestFixture]
            public class And_tasks_are_not_checked
            {
                private TestFilter _first, _second, _third;
                private List<string> _result;

                [SetUp]
                public void Initialize()
                {
                    Initialize_exception_handler();
                    _result = new List<string>();
                    _first = new TestFilter(m => { });
                    _second = new TestFilter(m => { if (m["data"] == "B") throw new Exception(); });
                    _third = new TestFilter(m => { _result.Add(m["data"]); });
                }

                [Test]
                public void Then_exception_handler_is_not_called()
                {
                    _first.LinkTo(_second).LinkTo(_third);
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                    Task.Delay(10).Wait();
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                    Task.Delay(10).Wait();
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                    Task.Delay(10).Wait();
                    Assert.AreEqual(0, _handlerCalledCount);
                }

                [Test]
                public void Then_messages_with_exception_are_not_in_result()
                {
                    _first.LinkTo(_second).LinkTo(_third);
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                    Task.Delay(10).Wait();
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                    Task.Delay(10).Wait();
                    _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                    Task.Delay(10).Wait();
                    Assert.AreEqual(2, _result.Count);
                    Assert.AreEqual("A", _result[0]);
                    Assert.AreEqual("C", _result[1]);
                }

                [Test]
                public void TearDown()
                {
                    Set_configuration_to_default();
                }
            }

            public class And_tasks_are_checked
            {
                private static TestFilter _first, _second, _third;
                private static List<string> _result;

                [TestFixture]
                public class And_flow_should_continue
                {
                    [SetUp]
                    public void Initialize()
                    {
                        Initialize_exception_handler();
                        NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.ContinueFlow;
                        _result = new List<string>();
                        _first = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { });
                        _second = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { if (m["data"] == "B") throw new Exception(); });
                        _third = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { _result.Add(m["data"]); });
                        _first.LinkTo(_second).LinkTo(_third);
                    }

                    [Test]
                    public void Then_exception_handler_is_called_proper_times()
                    {
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                        Task.Delay(10).Wait();
                        Assert.AreEqual(2, _handlerCalledCount);
                    }

                    [Test]
                    public void Then_flow_continues()
                    {
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                        Task.Delay(10).Wait();
                        Assert.AreEqual(2, _result.Count);
                        Assert.AreEqual("A", _result[0]);
                        Assert.AreEqual("C", _result[1]);
                    }

                    [TearDown]
                    public void TearDown()
                    {
                        Set_configuration_to_default();
                        NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.ContinueFlow;
                    }
                }

                [TestFixture]
                public class And_flow_should_stop
                {
                    private Action<NuntiusCommunicationException> exceptionHandler;
                    private NuntiusCommunicationException _exceptionMessage;

                    [SetUp]
                    public void Initialize()
                    {
                        Initialize_exception_handler();
                        exceptionHandler += m => _exceptionMessage = m;
                        NuntiusConfiguration.Exception += exceptionHandler;
                        NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.StopFlow;
                        _result = new List<string>();
                        _first = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { });
                        _second = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { if (m["data"] == "B") throw new Exception(); });
                        _third = new TestFilter(EventSourceCallbackMonitoringOptions.CheckTaskException, m => { _result.Add(m["data"]); });
                        _first.LinkTo(_second).LinkTo(_third);
                    }

                    [Test]
                    public void Then_exception_handler_is_called_proper_times()
                    {
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                        Task.Delay(10).Wait();
                        Assert.AreEqual(1, _handlerCalledCount);
                    }

                    [Test]
                    public void Then_exception_handler_gets_proper_message()
                    {
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                        Task.Delay(10).Wait();
                        Assert.AreEqual(_first, _exceptionMessage.ExceptionCatcher);
                        Assert.AreEqual(CommunicationExceptionOrigin.ProcessTask, _exceptionMessage.ExceptionOrigin);
                        Assert.AreEqual("B", _exceptionMessage.MessageCausingException["data"]);
                    }

                    [Test]
                    public void Then_flow_continues()
                    {
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "A" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "B" });
                        Task.Delay(10).Wait();
                        _first.ProcessMessage(new NuntiusMessage() { ["data"] = "C" });
                        Task.Delay(10).Wait();
                        Assert.AreEqual(1, _result.Count);
                        Assert.AreEqual("A", _result[0]);
                    }

                    [TearDown]
                    public void TearDown()
                    {
                        Set_configuration_to_default();
                        NuntiusConfiguration.Exception -= exceptionHandler;
                    }
                }
            }
        }

        [TestFixture]
        public class When_exception_is_thrown_in_end_handler
        {
            TestFilter _first, _second;
            private Action<NuntiusCommunicationException> _handler;
            private NuntiusCommunicationException _distributedException;

            [SetUp]
            public void Initialize()
            {
                NuntiusConfiguration.Exception += e => { _distributedException = e; };
                _first = new TestFilter(null, null, null);
                _second = new TestFilter(null, null, () => { throw new NullReferenceException(); });
                _first.LinkTo(_second);
            }

            [Test]
            public void The_proper_exception_message_is_distributed()
            {
                _first.EndProcessing();
                Task.Delay(20).Wait();
                Assert.AreEqual(_first, _distributedException.ExceptionCatcher);
                Assert.AreEqual(CommunicationExceptionOrigin.EndHandler, _distributedException.ExceptionOrigin);
                Assert.AreEqual(typeof(NullReferenceException), _distributedException.InnerExceptions[0].GetType());
            }

            [TearDown]
            public void TearDown()
            {
                NuntiusConfiguration.Exception -= _handler;
            }
        }
    }
}
