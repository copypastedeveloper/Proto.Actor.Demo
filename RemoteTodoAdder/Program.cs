using System;
using System.Linq;
using System.Threading.Tasks;
using Messages.Commands;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;

namespace RemoteTodoAdder
{
    class Program
    {
        static void Main(string[] args)
        {
            var createTodo = new CreateTodo { AggregateId = Guid.NewGuid().ToString(), Name = "remote Todo" };
            var complete = new CompleteTodo { AggregateId = createTodo.AggregateId };

            Remote.Start("127.0.0.1", 12001);
            Cluster.Start("MyCluster", new ConsulProvider(new ConsulProviderOptions{DeregisterCritical = TimeSpan.FromSeconds(1)}));

            EventStream.Instance.Subscribe(x =>
            {
                Console.WriteLine($"{DateTime.Now} - {x}");

                switch (x)
                {
                    case ClusterTopologyEvent e:
                        e.Statuses.ToList().ForEach(s => Console.WriteLine($"    {s.Address}:{s.Port}/{s.MemberId} - Alive:{s.Alive}"));
                        break;
                    case EndpointTerminatedEvent e:
                        Console.WriteLine($"    {e.Address} was terminated");
                        break;
                }

            });


            while (Console.ReadLine() != "stop")
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var pid = await Cluster.GetAsync(createTodo.AggregateId, "Todo");
                        pid.Tell(createTodo);
                        pid.Tell(complete);
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine(ex.Message);
                        foreach (var innerException in ex.InnerExceptions)
                        {
                            Console.WriteLine(innerException.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }

            Console.ReadLine();
        }
    }
}
