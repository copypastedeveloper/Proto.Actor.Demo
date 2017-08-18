using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Proto;
using Proto.Persistence;
using Proto.Persistence.SnapshotStrategies;
using StreamstoneDemo.App.Infrastructure;

namespace StreamstoneDemo.App.Aggregates
{
    public abstract class AggregateRoot<TState> : IActor, IEventSink where TState : IStateData, new()
    {
        readonly IProvider _provider;
        readonly Dictionary<Type, MethodInfo> _handleMethods;
        Persistence _persistence;

        protected TState State { get; private set; }
        public Guid Id { get; private set; }
        protected abstract int EventsBetweenSnapshots { get; }

        protected AggregateRoot(IProvider provider)
        {
            _provider = provider;
            State = new TState();
            
            _handleMethods = GetType().GetMethods()
                .Where(x => x.Name == "Handle" && x.GetParameters().Length == 1)
                .ToDictionary(x => x.GetParameters().First().ParameterType, x => x);
        }

        void Hydrate(Snapshot obj)
        {
            if (obj.State is TState state)
                State = state;
        }

        void Mutate(Event @event)
        {
            if (@event.Data is IEvent e)
                State.Apply(e);
        }

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is Started)
            {
                Init(context).Wait();
                return Actor.Done;
            }

            var messageType = context.Message.GetType();
            Console.WriteLine($"Got a message of type {messageType.Name}");

            if (_handleMethods.ContainsKey(messageType))
                _handleMethods[messageType].Invoke(this, new [] {context.Message});
            else 
                Console.WriteLine($"Got a message of type {messageType.Name}, but I'm a {GetType().Name} and I'm not interested");

            return Actor.Done;
        }

        public async Task Init(IContext ctx)
        {
            Id = Guid.Parse(ctx.Self.Id);

            _persistence = Persistence.WithEventSourcingAndSnapshotting(_provider, _provider, Id.ToString(), Mutate,
                Hydrate, new IntervalStrategy(EventsBetweenSnapshots), () => State);

            await _persistence.RecoverStateAsync();
        }

        public void Raise(IEvent @event)
        {
            _persistence.PersistEventAsync(@event).Wait();
            //shoot over to readmodel stuff
        }
    }
}