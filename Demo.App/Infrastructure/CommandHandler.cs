using System.Threading.Tasks;
using Messages.Commands;
using Proto;

namespace StreamstoneDemo.App.Infrastructure
{
    public class CommandHandler : IActor
    {
        readonly IAggregateRepository _repository;

        public CommandHandler(IAggregateRepository repository)
        {
            _repository = repository;
        }

        public Task ReceiveAsync(IContext context)
        {
            //if (context.Message is AggregateCommand command)
            //{
            //    var aggregate = _repository.Get(command.AggregateType, command.AggregateId);
            //    aggregate.Tell(command);
            //}
            return Actor.Done;
        }
    }
}