using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Tests.Common
{
    public struct Context : IEventContext<ContextfulEvent>
    {
        public int Data { get; set; }
    }

    public struct DualContext : IEventContext<DualEvent>
    {
        public int Data { get; set; }

        public DualContext(int data = 42)
        {
            Data = data;
        }
    }
}
