using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Core.Subscriptions
{
    internal class InternalContextfulSubscription<TEvent, TEventContext> : ISubscription<TEvent, TEventContext>
        where TEvent : IContextfulEvent
        where TEventContext : IEventContext<TEvent>
    {
        private Action<TEventContext> callback;

        public InternalContextfulSubscription(Action<TEventContext> action)
        {
            callback = action;
        }

        public void Handle(TEventContext context)
        {
            callback(context);
        }
    }
}
