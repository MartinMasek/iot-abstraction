using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuntius;

namespace TestNuntius
{
    public class TestDevice : IDevice
    {
        public async void Initialize()
        {
            int i = 0;
            while (true)
            {
                Send?.Invoke(new NuntiusMessage() { ["Data"] = i.ToString(), ["Original"] = i.ToString(), ["Trim"] = (i * i).ToString() });
                i++;
                await Task.Delay(1000);
            }
        }

        public void EndDevice()
        {
            End?.Invoke();
        }

        public event Func<NuntiusMessage, Task> Send;
        public event Action End;
    }
}
