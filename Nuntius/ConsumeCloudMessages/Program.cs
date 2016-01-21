using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuntius;
using Nuntius.Azure.Integration;

namespace ConsumeCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            var c2d = new CloudToDeviceEndpoint("myId", "HostName=iotprivacy.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=DnGzzFikzUb2oxXqzDpFkv356unp8tGTie3m5ysV59c=");
            c2d.Initialize();
            c2d.LinkTo(m =>
            {
                return Task.Factory.StartNew(() => Console.WriteLine(m));
            });
            Console.ReadLine();
        }
    }
}
