using System;
using Messages.Commands;
using Proto.Persistence;
using StreamstoneDemo.App.Aggregates.State;
using StreamstoneDemo.App.Events;

namespace StreamstoneDemo.App.Aggregates
{
    internal class Todo : AggregateRoot<TodoState>
    {
        protected override int EventsBetweenSnapshots => 2;

        public Todo(IProvider provider) : base(provider) {}
        
        public void Handle(CreateTodo createTodo)
        {
            if (State.Name == null)
                Raise(new TodoCreated(createTodo.Name));
        }

        public void Handle(CompleteTodo createTodo)
        {
            if (!State.Complete)
                Raise(new TodoCompleted());
        }
    }
}