using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Core.Subscriptions
{
    public interface ISubscription<TEvent, TEventContext> : ISubscription
        where TEvent : IContexfulEvent
        where TEventContext: IEventContext<TEvent>
    {
        void Handle(TEventContext context);
    }

    public interface ISubscription<TEvent> : ISubscription
        where TEvent : IContextlessEvent
    {
        void Handle();
    }

    public interface ISubscription { }
}
