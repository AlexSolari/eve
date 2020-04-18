using Eve.Core.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Tests.Common
{
    public class ContextfulSubscription : ISubscription<ContextfulEvent, Context>
    {
        public virtual void Handle(Context context)
        {
            
        }
    }

    public class ContextlessSubscription : ISubscription<ContextlessEvent>
    {
        public virtual void Handle()
        {

        }
    }

    public class InvalidDualSubscription : ISubscription<DualEvent, DualContext>
    {
        public void Handle(DualContext context)
        {
            
        }
    }

    public class ValidDualSubscription : ISubscription<DualEvent, DualContext>, ISubscription<DualEvent>
    {
        private Action callback;

        public ValidDualSubscription()
        {

        }

        public ValidDualSubscription(Action callback)
        {
            this.callback = callback;
        }

        public void Handle(DualContext context)
        {
            callback();
        }

        public void Handle()
        {
            callback();
        }
    }
}
