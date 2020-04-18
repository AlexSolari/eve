using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Tests.Common
{
    public class Context : IEventContext<ContextfulEvent>
    {
        public int Data { get; set; }
    }

    public class DualContext : IEventContext<DualEvent>
    {
        public int Data { get; set; } = 42;
    }
}
