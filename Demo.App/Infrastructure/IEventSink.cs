namespace StreamstoneDemo.App.Infrastructure
{
    public interface IEventSink
    {
        void Raise(IEvent @event);
    }
}