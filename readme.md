# Eve
Lightweight event management system.

https://www.nuget.org/packages/Eve/

## Usage
Create a new instance of `EventHandler` and use it to manage events via Subscribe, Unsubscribe and Dispatch operations.

To create an event create a class that implements either `IContextfulEvent` or `IContextlessEvent`.
In case you want to have context available for event you need it to implement `IContextfulEvent` and create a context class that will implement `IEventContext`.
Example:
```c#
    class Event : IContextfulEvent { }

    class EventContext : IEventContext<Event> 
    {
        public int MyData = 42;
    }
```

In order to subscribe to event you need to use `EventHandler.Subscribe` method.
There is two cases of using it: with class that implements `ISubscription` interface and with anonymous method:

```c#
    ISubscription<Event, EventContext> subscription = new MySubscription(); 
    handler.Subscribe(subscription);

    handler.Subscribe<Event, EventContext>(ctx => { });
```

`EventHandler.Subscribe` method returns `ISubscription`, so you can use it later with `EventHandler.Unsubscribe` method if needed.

In order to fire an event, you need to use `EventHandler.Dispatch` method.
Example:

```c#
    handler.Dispatch<ContextlessEvent>();
    handler.Dispatch<ContextfulEvent, Context>(new Context());
```
