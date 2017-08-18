using StreamstoneDemo.App.Events;
using StreamstoneDemo.App.Infrastructure;

namespace StreamstoneDemo.App.Aggregates.State
{
    internal class TodoState : IStateData
    {
        public string Name { get; set; }
        public bool Complete { get; set; }

        public void Apply(IEvent @event)
        {
            switch (@event)
            {
                case TodoCreated todoCreated:
                    Name = todoCreated.Name;
                    break;
                case TodoCompleted _:
                    Complete = true;
                    break;
            }
        }
    }
}