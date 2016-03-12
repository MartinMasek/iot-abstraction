using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nuntius;
using Nuntius.Logging;
using Nuntius.Privacy;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = new BaseDeviceSourceEndpoint();
            d.LinkTo(new TrimMessageFilter("Trim")).LogToFile(@".\Foo\Log_2.txt", true);
            d.SendMessage(new NuntiusMessage()
            {
                Properties =
                    {
                        {"Id", "1"},
                        {"Name", "crews"},
                        {"Trim", "TT"}
                    }
            });
            Thread.Sleep(1000);
            d.SendMessage(new NuntiusMessage()
            {
                Properties =
                    {
                        {"Id", "1"},
                        {"Trim", "TT"}
                    }
            });
            Thread.Sleep(1000);
            d.SendMessage(new NuntiusMessage()
            {
                Properties =
                    {
                        {"Id", "1"},
                        {"Trim", "TT"}
                    }
            });
            d.EndSending();
        }
    }
}
