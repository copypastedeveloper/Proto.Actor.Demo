using StreamstoneDemo.App.Infrastructure;

namespace StreamstoneDemo.App.Events
{
    internal class TodoCreated : IEvent
    {
        public TodoCreated(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}