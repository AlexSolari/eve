using Eve.Core.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Eve.Teset
{
    class Program
    {
        class Event : IContextlessEvent { };

        class Event2 : IContextfulEvent { };
        class Context : IEventContext<Event2>
        {

        }

        
        static void Main(string[] args)
        {
            Task.WaitAll(Go());
        }

        static async Task Go()
        {
            var handler = new EventHandler();

            var a = new Stopwatch();
            a.Start();
            for (int i = 0; i < 50000; i++)
            {
                handler.Subscribe<Event>(() => { });
            }
            handler.Dispatch<Event>();
            a.Stop();
            Console.WriteLine($"contextless: Elapsed: {a.ElapsedMilliseconds}");

            handler = new EventHandler();
            a = new Stopwatch();
            a.Start();
            for (int i = 0; i < 50000; i++)
            {
                handler.Subscribe<Event2, Context>((ctx) => { });
            }
            handler.Dispatch<Event2, Context>(new Context());
            a.Stop();
            Console.WriteLine($"contextful: Elapsed: {a.ElapsedMilliseconds}");

            handler = new EventHandler();
            a = new Stopwatch();
            a.Start();
            for (int i = 0; i < 50000; i++)
            {
                handler.Subscribe<Event2, Context>((ctx) => { });
            }
            await Task.Factory.StartNew(() => handler.Dispatch<Event2, Context>(new Context()));
            a.Stop();
            Console.WriteLine($"contextful from other thread: Elapsed: {a.ElapsedMilliseconds}");

            handler = new EventHandler();
            a = new Stopwatch();
            a.Start();
            var t1 = Task.Factory.StartNew(() => {
                for (int i = 0; i < 25000; i++)
                {
                    handler.Subscribe<Event2, Context>((ctx) => { });
                }
            });
            var t2 = Task.Factory.StartNew(() => {
                for (int i = 0; i < 25000; i++)
                {
                    handler.Subscribe<Event2, Context>((ctx) => { });
                }
            });
            Task.WaitAll(t1, t2);
            handler.Dispatch<Event2, Context>(new Context());
            a.Stop();
            Console.WriteLine($"contextful adding from other thread: Elapsed: {a.ElapsedMilliseconds}");
        }
    }
}
