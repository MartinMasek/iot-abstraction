using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nuntius.Tests
{
    /// <summary>
    /// These tests test <see cref="EventSourceBase"/> because it is a class from other classes implementing 
    /// <see cref="IEventSource"/> should be derived. Custom tests must be written if other class is used.
    /// </summary>
    class CommunicationExceptionHandlingTests
    {
        public class When_exception_is_thrown
        {
            [TestFixture]
            public class And_nobody_listens
            {
                [Test]
                public void Then_nothing_happens()
                {
                    
                }
            }
        }

        public class When_exception_is_thrown_before_task_starts
        {

        }

        public class When_exception_is_thrown_in_task
        {

        }

        public class When_exception_is_thrown_in_end_handler
        {

        }
        // not check task and throw exception
        // check task and throw exception
        // exception during end
        // exception in task
        // exception before task
    }
}
