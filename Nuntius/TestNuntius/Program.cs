using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nuntius.Privacy;
using Nuntius;

namespace TestNuntius
{
    class Program
    {
        static void Main(string[] args)
        {
            var hash = new HashFilter(HashType.Sha256, "Data");
            var trim = new TrimMessageFilter("Trim");
            var d = new TestDeviceSourceEndpoint();
            d.LinkTo(hash).LinkTo(trim).LinkTo(m =>
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
                        ["Trim"] = (i*i).ToString()
                    });
                    i++;
                    Task.Delay(100).Wait();
                }
            });
            Console.ReadLine();
            d.EndSending();
            Console.ReadLine();
        }
    }
}
