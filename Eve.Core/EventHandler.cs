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

        #region Public API
        
        public ISubscription<TEvent, TEventContext> Subscribe<TEvent, TEventContext>(ISubscription<TEvent, TEventContext> subscription)
            where TEvent : IContextfulEvent
            where TEventContext : struct, IEventContext<TEvent>
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
            where TEventContext : struct, IEventContext<TEvent>
        {
            var subscription = new InternalContextfulSubscription<TEvent, TEventContext>(action);

            return Subscribe(subscription);
        }

        public ISubscription<TEvent> Subscribe<TEvent>(Action action)
            where TEvent : IContextlessEvent
        {
            var subscription = new InternalContextlessSubscription<TEvent>(action);

            return Subscribe(subscription);
        }

        public void Unsubscribe<TEvent, TEventContext>(ISubscription<TEvent, TEventContext> subscription)
            where TEvent : IContextfulEvent
            where TEventContext : struct, IEventContext<TEvent>
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
            where TEventContext : struct, IEventContext<TEvent>
        {
            var key = GetEventKey<TEvent>();
            if (Subscriptions.ContainsKey(key))
            {
                var subscriptionsToNotify = Subscriptions[key];

                subscriptionsToNotify.ForEach(subscription => {
                    var castedSubscription = subscription as ISubscription<TEvent, TEventContext>;

                    if (castedSubscription == null)
                        throw new InvalidOperationException($"Subscription {subscription.GetType().FullName} does not implements ISubscription<TEvent, TEventContext>");

                    castedSubscription.Handle(context);
                });
            }
        }

        public void Dispatch<TEvent>()
            where TEvent : IContextlessEvent
        {
            var key = GetEventKey<TEvent>();
            if (Subscriptions.ContainsKey(key))
            {
                var subscriptionsToNotify = Subscriptions[key];
                

                subscriptionsToNotify.ForEach(subscription => {
                    var castedSubscription = subscription as ISubscription<TEvent>;

                    if (castedSubscription == null)
                        throw new InvalidOperationException($"Subscription {subscription.GetType().FullName} does not implements ISubscription<TEvent>");

                    castedSubscription.Handle();
                });
            }
        }

        #endregion

        #region Internals

        private bool RemoveSubscription(ISubscription subscription, string key)
        {
            if (subscription == null)
            {
                throw new ArgumentException("Subscription cannot be null");
            }

            var result = false;

            lock (lockObject)
            {
                if (Subscriptions.ContainsKey(key))
                {
                    var eventSubscriptions = Subscriptions[key];
                    result = eventSubscriptions.Remove(subscription);
                }
            };

            return result;
        }

        private void AddSubscription<TEvent>(ISubscription subscription)
            where TEvent : IEvent
        {
            if (subscription == null)
            {
                throw new ArgumentException("Subscription cannot be null");
            }

            var key = GetEventKey<TEvent>();
            lock (lockObject)
            {
                if (!Subscriptions.ContainsKey(key))
                {
                    Subscriptions.Add(key, new List<ISubscription>());
                }

                if (Subscriptions[key].Contains(subscription))
                {
                    throw new ArgumentException("This subscription is already registered in a system. Try creating new subscription instead.");
                }

                Subscriptions[key].Add(subscription);
            };
        }

        private string GetEventKey<TEvent>()
        {
            return typeof(TEvent).FullName;
        }

        #endregion
    }
}
