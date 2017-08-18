using System;
using System.Threading.Tasks;
using Proto;

namespace StreamstoneDemo.App.Actors
{
    public class ReadmodelManager : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            Console.WriteLine($"Got message of type {context.Message.GetType().Name}");

            //create more actors here to handle the processing of the event

            return Actor.Done;
        }
    }
}