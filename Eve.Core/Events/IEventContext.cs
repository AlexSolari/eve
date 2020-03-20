using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Core.Events
{
    public interface IEventContext<TEvent>
        where TEvent : IContextfulEvent
    {

    }
}
