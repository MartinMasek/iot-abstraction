using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Tests.Privacy
{
    class DelayFilterTest
    {
        public class When_few_messages_are_sent
        {
            public class And_all_are_below_delay_interval
            {
                public void Initialize()
                {
                    
                }

                public void Then_last_is_returned()
                {
                    
                }
            }

            public class And_one_is_above_delay_interval
            {
                public void Initialize()
                {

                }

                public void Then_two_messages_are_returned()
                {

                }
            }
        }

        public class When_no_message_is_sent
        {
            public void Initialize()
            {

            }

            public void Then_no_is_returned()
            {

            }
        }
    }
}
