using System;
using Proto;

namespace StreamstoneDemo.App.Infrastructure
{
    public interface IAggregateRepository
    {
        PID Get<T>(Guid id, string address = null, IContext parent = null) where T : IActor;
        PID Get(Type aggregateType, Guid id, string address = null, IContext parent = null);
    }
}