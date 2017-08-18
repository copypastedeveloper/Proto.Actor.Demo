using System;
using System.Collections.Generic;
using System.Reflection;
using Proto;
using StructureMap;
using StructureMap.Pipeline;
using IContext = Proto.IContext;

namespace StreamstoneDemo.App.Infrastructure
{
    public class AggregateRepository : IAggregateRepository
    {
        readonly IContainer _serviceProvider;
        readonly MethodInfo _getGenericMethod;

        public AggregateRepository(IContainer container)
        {
            _serviceProvider = container;
            _getGenericMethod = GetType().GetMethod("Get`1", BindingFlags.Public | BindingFlags.Instance);
        }

        public PID Get<T>(Guid id, string address = null, IContext parent = null) where T : IActor
        {
            return Get(id.ToString(), address, parent, () => CreateActor(id.ToString(), parent,
                () => new Props().WithProducer(() => _serviceProvider.GetInstance<T>())));
        }

        public PID Get(Type aggregateType, Guid id, string address = null, IContext parent = null)
        {
            return (PID)_getGenericMethod.MakeGenericMethod(aggregateType).Invoke(this, new object[] {id, address, parent});
        }

        static PID Get(string id, string address, IContext parent, Func<PID> create)
        {
            address = address ?? "nonhost";

            var pidId = id;
            if (parent != null)
                pidId = $"{parent.Self.Id}/{id}";

            var pid = new PID(address, pidId);
            var reff = ProcessRegistry.Instance.Get(pid);
            if (reff is DeadLetterProcess)
                pid = create();
            return pid;
        }

        static PID CreateActor(string id, IContext parent, Func<Props> producer) => parent == null ? Actor.SpawnNamed(producer(), id) : parent.SpawnNamed(producer(), id);
    }
}