using Eve.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Core.Subscriptions
{
    internal class InternalContextlessSubscription<TEvent> : ISubscription<TEvent>
        where TEvent : IContextlessEvent
    {
        private Action callback;

        public InternalContextlessSubscription(Action action)
        {
            callback = action;
        }

        public void Handle()
        {
            callback();
        }
    }
}
