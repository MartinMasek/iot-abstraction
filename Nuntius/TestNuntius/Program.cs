using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nuntius.Privacy;
using Nuntius;
using Nuntius.Azure.Integration;

namespace TestNuntius
{
    class Program
    {
        static void Main(string[] args)
        {
            var hash = new HashFilter(HashType.Sha256, "Data");
            var trim = new TrimMessageFilter("Trim");
            var d = new TestDeviceSourceEndpoint();
            NuntiusConfiguration.CommunicationExceptionStrategy = CommunicationExceptionStrategy.StopFlow;
            NuntiusConfiguration.Exception += m =>
            {
                Console.WriteLine(m.InnerExceptions[0]);
            };
            var d2c = new DeviceToCloudEndpoint("myId", "iotprivacy.azure-devices.net", "MSmK/ZpH6sHsLPd/sS9czefw+OQulerhBCXGN90Pw9g=", "myFirstDevice");
            d.LinkTo(hash).LinkTo(trim)
                //.LinkTo(new DelayFilter(3000))
                .LinkTo(new KAnonymityFilter<object>(new CountAnonymitySet[]
                {
                  new CountAnonymitySet(0,3,4500),
                  new CountAnonymitySet(1,3,4500),
                }, m => m["Original"], o => Int32.Parse((string)o) % 2))

                //.LinkTo(d2c);
                .LinkTo(m =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine(m);
                    });
                });
            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    d.SendMessage(new NuntiusMessage()
                    {
                        ["Data"] = i.ToString(),
                        ["Original"] = i.ToString(),
                        ["Trim"] = (i * i).ToString()
                    });
                    i++;
                    Task.Delay(1000).Wait();
                }
            });
            Console.ReadLine();
            d.EndSending();
            Console.ReadLine();
        }
    }
}
