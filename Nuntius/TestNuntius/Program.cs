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
            var d = new TestDevice();
            d.LinkTo(hash).LinkTo(trim).LinkTo(m =>
            {
                return Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(m);
                });
            });
            d.Initialize();
            Console.ReadLine();
            d.EndDevice();
            Console.ReadLine();
        }
    }
}
