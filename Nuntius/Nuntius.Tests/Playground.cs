using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Nuntius.Privacy;
using NUnit.Framework;

namespace Nuntius.Tests
{
    public class TestDevice : IDevice
    {
        public async void Initialize()
        {
            int i = 0;
            while (true)
            {
                Send?.Invoke(new NuntiusMessage() { ["Data"] = i.ToString() });
                i++;
                await Task.Delay(1000);
            }
        }

        public event Func<NuntiusMessage, Task> Send;
        public event Action End;
    }

    [TestFixture]
    public class TestRun
    {
        [Test]
        public void Run()
        {
            var hash = new HashFilter(HashType.Sha256, "Data");
            var d = new TestDevice();
            d.Send += hash.ProcessMessage;
            hash.Send += m => Task.Factory.StartNew(() => Debug.WriteLine(m["Data"]));
            Thread.Sleep(5000);
        }
    }
}
