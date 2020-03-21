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
}
