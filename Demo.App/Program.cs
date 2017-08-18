using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Messages.Commands;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Mailbox;
using Proto.Persistence;
using Proto.Persistence.SqlServer;
using Proto.Remote;
using StreamstoneDemo.App.Aggregates;
using StreamstoneDemo.App.Infrastructure;
using StructureMap;

namespace StreamstoneDemo.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(CommandsReflection.Descriptor);
            var container = new Container();
            container.Configure(c =>
            {
                var provider = new SqlServerProvider("Data Source=(local);Initial Catalog=NContracts;Integrated Security=SSPI;MultipleActiveResultSets=true", true, "actors");

                c.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                    scanner.AddAllTypesOf<IActor>();
                });

                c.For<IProvider>().Use(x => provider).Singleton();
                c.For<IAggregateRepository>()
                    .Use(ctx => new AggregateRepository(container));
            });

            Remote.RegisterKnownKind("Todo", Actor.FromProducer(() => container.GetInstance<Todo>()));

            Remote.Start("127.0.0.1", int.Parse(ConfigurationManager.AppSettings["clusterport"]));
            Cluster.Start("MyCluster", new ConsulProvider(new ConsulProviderOptions()));

            //var createTodo = new CreateTodo { AggregateId = Guid.NewGuid().ToString(), Name = "local Todo" };
            //var complete = new CompleteTodo { AggregateId = createTodo.AggregateId };
            //var a = Actor.SpawnNamed(Actor.FromProducer(() => container.GetInstance<Todo>()), createTodo.AggregateId);

            //a.Tell(createTodo);
            //a.Tell(complete);

            Console.ReadLine();
        }
    }
}
