using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eve;
using Handler = Eve.EventHandler;
using Eve.Tests.Common;
using Moq;
using System;
using Eve.Core.Subscriptions;

namespace Eve.Tests
{
    [TestClass]
    public class HandlerTest
    {
        [TestMethod]
        public void Handler_ShouldNotify_ContexfulSubscribe()
        {
            //Arrange
            var handler = new Handler();
            var subscriptionMock = new Mock<ContextfulSubscription>();
            var context = new Context()
            {
                Data = 42
            };
            subscriptionMock.Setup(m => m.Handle(It.IsAny<Context>()));
            handler.Subscribe(subscriptionMock.Object);

            //Act
            handler.Dispatch<ContextfulEvent, Context>(context);

            //Assert
            subscriptionMock.Verify(x => x.Handle(context), Times.Once());
        }

        [TestMethod]
        public void Handler_ShouldNotify_ContexfulLambda()
        {
            //Arrange
            var subscriptionCalled = false;
            var handler = new Handler();
            var context = new Context()
            {
                Data = 42
            };
            handler.Subscribe<ContextfulEvent, Context>((ctx) => {
                subscriptionCalled = true;
            });

            //Act
            handler.Dispatch<ContextfulEvent, Context>(context);

            //Assert
            Assert.IsTrue(subscriptionCalled);
        }

        [TestMethod]
        public void Handler_ShouldNotify_ContexlessSubscribe()
        {
            //Arrange
            var handler = new Handler();
            var subscriptionMock = new Mock<ContextlessSubscription>();
            subscriptionMock.Setup(m => m.Handle());
            handler.Subscribe(subscriptionMock.Object);

            //Act
            handler.Dispatch<ContextlessEvent>();

            //Assert
            subscriptionMock.Verify(x => x.Handle(), Times.Once());
        }

        [TestMethod]
        public void Handler_ShouldNotify_ContexlessLambda()
        {
            //Arrange
            var subscriptionCalled = false;
            var handler = new Handler();
            handler.Subscribe<ContextlessEvent>(() => {
                subscriptionCalled = true;
            });

            //Act
            handler.Dispatch<ContextlessEvent>();

            //Assert
            Assert.IsTrue(subscriptionCalled);
        }

        [TestMethod]
        public void Handler_ShouldNotNotify_WhenSubscriptionRemoved()
        {
            //Arrange
            var subscriptionCalled = false;
            var handler = new Handler();
            var subscription = handler.Subscribe<ContextlessEvent>(() => {
                subscriptionCalled = true;
            });
            handler.Unsubscribe(subscription);

            //Act
            handler.Dispatch<ContextlessEvent>();

            //Assert
            Assert.IsFalse(subscriptionCalled);
        }

        [TestMethod]
        public void Handler_ShouldNotify_AllSubscriptions()
        {
            //Arrange
            var subscriptionsHandled = 0;
            var handler = new Handler();
            handler.Subscribe<ContextlessEvent>(() => {
                subscriptionsHandled++;
            });
            handler.Subscribe<ContextlessEvent>(() => {
                subscriptionsHandled++;
            });
            handler.Subscribe<ContextlessEvent>(() => {
                subscriptionsHandled++;
            });

            //Act
            handler.Dispatch<ContextlessEvent>();

            //Assert
            Assert.AreEqual(subscriptionsHandled, 3);
        }

        [TestMethod]
        public void Handler_ShouldThrowException_WhenUnsubscribing_WhenEventIsNotSubscribed()
        {
            //Arrange
            var handler = new Handler();
            var subscription = new ContextfulSubscription();

            //Act
            //Assert
            Assert.ThrowsException<ArgumentException>(() => handler.Unsubscribe(subscription));
        }

        [TestMethod]
        public void Handler_ShouldThrowException_WhenUnsubscribing_WhenSubscriptionIsNull()
        {
            //Arrange
            var handler = new Handler();
            ContextfulSubscription subscription = null;

            //Act
            //Assert
            Assert.ThrowsException<ArgumentException>(() => handler.Subscribe(subscription));
        }

        [TestMethod]
        public void Handler_ShouldThrowException_WhenUnsubscribing_WhenUnsubscribedTwice()
        {
            //Arrange
            var handler = new Handler();
            var subscription = new ContextlessSubscription();
            handler.Subscribe(subscription);

            //Act
            //Assert
            handler.Unsubscribe(subscription);
            Assert.ThrowsException<ArgumentException>(() => handler.Unsubscribe(subscription));
        }

        [TestMethod]
        public void Handler_ShouldThrowException_WhenSubscribedTwice()
        {
            //Arrange
            var handler = new Handler();
            var subscription = new ContextlessSubscription();
            handler.Subscribe(subscription);

            //Act
            //Assert
            Assert.ThrowsException<ArgumentException>(() => handler.Subscribe(subscription));
        }

        [TestMethod]
        public void Handler_ShouldThrow_WhenDispatching_IfContextIsNull()
        {
            //Arrange
            var handler = new Handler();
            var subscription = new ContextfulSubscription();
            handler.Subscribe(subscription);

            //Act
            //Assert
            Assert.ThrowsException<ArgumentException>(() => handler.Dispatch<ContextfulEvent, Context>(null));
        }

        [TestMethod]
        public void Handler_ShouldThrow_WhenSubscribing_IfSubscriptionIsNull()
        {
            //Arrange
            var handler = new Handler();
            ISubscription<ContextlessEvent> subscription = null; 

            //Act
            //Assert
            Assert.ThrowsException<ArgumentException>(() =>handler.Subscribe(subscription));
        }

        [TestMethod]
        public void Handler_ShouldThrow_WhenSubscriptionForDualEvent_IsNotImplementedProperly()
        {
            //Arrange
            var handler = new Handler();
            var context = new DualContext();
            var subs = new InvalidDualSubscription();
            handler.Subscribe(subs);

            //Act
            handler.Dispatch<DualEvent, DualContext>(context);

            //Assert
            Assert.ThrowsException<InvalidOperationException>(() => handler.Dispatch<DualEvent>());
        }

        [TestMethod]
        public void Handler_ShouldNotThrow_WhenSubscriptionForDualEvent_IsImplementedProperly()
        {
            //Arrange
            var timesDispatched = 0;
            var handler = new Handler();
            var context = new DualContext();
            var subs = new ValidDualSubscription(() => { timesDispatched++; });
            handler.Subscribe<DualEvent>(subs);

            //Act
            //Assert
            try
            {
                handler.Dispatch<DualEvent, DualContext>(context);
                handler.Dispatch<DualEvent>();
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception, but got {e.Message}");
            }
            Assert.AreEqual(timesDispatched, 2);
        }

        [TestMethod]
        public void Handler_ShouldDispatchAll_WhenSubscriptionForDualEvent_IsImplementedProperly()
        {
            //Arrange
            var subscriptionsHandled = 0;
            var handler = new Handler();
            var context = new DualContext();
            var subs = new ValidDualSubscription(() => subscriptionsHandled++);
            handler.Subscribe<DualEvent>(subs);

            //Act
            handler.Dispatch<DualEvent, DualContext>(context);
            handler.Dispatch<DualEvent>();

            //Assert
            Assert.AreEqual(2, subscriptionsHandled);
        }
    }
}
