using Eve.Core.Events;
using Eve.Core.Subscriptions;
using System;
using System.Collections.Generic;

namespace Eve
{
    public class EventHandler
    {
        private object lockObject = new object();
        private Dictionary<string, List<ISubscription>> Subscriptions = new Dictionary<string, List<ISubscription>>();

        public ISubscription<TEvent, TEventContext> Subscribe<TEvent, TEventContext>(ISubscription<TEvent, TEventContext> subscription)
            where TEvent : IContextfulEvent
            where TEventContext : IEventContext<TEvent>
        {
            AddSubscription<TEvent>(subscription);

            return subscription;
        }
        
        public ISubscription<TEvent> Subscribe<TEvent>(ISubscription<TEvent> subscription)
            where TEvent : IContextlessEvent
        {
            AddSubscription<TEvent>(subscription);

            return subscription;
        }

        public ISubscription<TEvent, TEventContext> Subscribe<TEvent, TEventContext>(Action<TEventContext> action) 
            where TEvent : IContextfulEvent
            where TEventContext : IEventContext<TEvent>
        {
            var subscription = new InternalContextfulSubscription<TEvent, TEventContext>(action);

            Subscribe(subscription);

            return subscription;
        }

        public ISubscription<TEvent> Subscribe<TEvent>(Action action)
            where TEvent : IContextlessEvent
        {
            var subscription = new InternalContextlessSubscription<TEvent>(action);

            Subscribe(subscription);

            return subscription;
        }

        public void Unsubscribe<TEvent, TEventContext>(ISubscription<TEvent, TEventContext> subscription)
            where TEvent : IContextfulEvent
            where TEventContext : IEventContext<TEvent>
        {
            var wasRemoved = false || RemoveSubscription(subscription, GetEventKey<TEvent>());
            if (!wasRemoved)
            {
                throw new ArgumentException("Subscription cannot be removed since it is not registered in a system.");
            }
        }

        public void Unsubscribe<TEvent>(ISubscription<TEvent> subscription)
            where TEvent : IContextlessEvent
        {
            var wasRemoved = false || RemoveSubscription(subscription, GetEventKey<TEvent>());
            if (!wasRemoved)
            {
                throw new ArgumentException("Subscription cannot be removed since it is not registered in a system.");
            }
        }

        public void Dispatch<TEvent, TEventContext>(TEventContext context)
            where TEvent : IContextfulEvent
            where TEventContext : IEventContext<TEvent>
        {
            var key = GetEventKey<TEvent>();
            if (Subscriptions.ContainsKey(key))
            {
                var subscriptionsToNotify = Subscriptions[key];

                subscriptionsToNotify.ForEach(subscription => ((ISubscription<TEvent, TEventContext>)subscription).Handle(context));
            }
        }

        public void Dispatch<TEvent>()
            where TEvent : IContextlessEvent
        {
            var key = GetEventKey<TEvent>();
            if (Subscriptions.ContainsKey(key))
            {
                var subscriptionsToNotify = Subscriptions[key];

                subscriptionsToNotify.ForEach(subscription => ((ISubscription<TEvent>)subscription).Handle());
            }
        }

        private bool RemoveSubscription(ISubscription subscription, string key)
        {
            var result = false;

            lock (lockObject)
            {
                var eventSubscriptions = Subscriptions[key];
                result = eventSubscriptions.Remove(subscription);
            };

            return result;
        }
        private void AddSubscription<TEvent>(ISubscription subscription)
            where TEvent : IEvent
        {
            var key = GetEventKey<TEvent>();
            lock (lockObject)
            {
                if (!Subscriptions.ContainsKey(key))
                {
                    Subscriptions.Add(key, new List<ISubscription>());
                }

                Subscriptions[key].Add(subscription);
            };
        }
        private string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }
    }
}
