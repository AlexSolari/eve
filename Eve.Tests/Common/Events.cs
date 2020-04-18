using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Tests.Common
{
    public class ContextfulEvent : IContextfulEvent { }

    public class ContextlessEvent : IContextlessEvent { }

    public class DualEvent : IContextfulEvent, IContextlessEvent { }
}
