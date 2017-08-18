namespace StreamstoneDemo.App.Infrastructure
{
    public interface IStateData
    {
        void Apply(IEvent @event);
    }
}