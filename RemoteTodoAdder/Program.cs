using System;
using System.Threading.Tasks;
using Messages.Commands;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;

namespace RemoteTodoAdder
{
    class Program
    {
        static void Main(string[] args)
        {
            var createTodo = new CreateTodo{AggregateId = Guid.NewGuid().ToString(),Name = "Remote Todo"};

            var complete = new CompleteTodo {AggregateId = createTodo.AggregateId};

            Remote.Start("127.0.0.1", 12001);
            Cluster.Start("MyCluster", new ConsulProvider(new ConsulProviderOptions()));

            while (Console.ReadLine() != "stop")
            {
                Task.Run(async () =>
                {
                    var pid = await Cluster.GetAsync(createTodo.AggregateId, "Todo");
                    pid.Tell(createTodo);
                    pid.Tell(complete);
                }).Wait();
            }

            Console.ReadLine();
        }
    }
}
