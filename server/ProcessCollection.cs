using Microsoft.AspNetCore.SignalR;
using Terminal.Web.Handlers;
using Terminal.Web.Hubs;

namespace Terminal.Web
{
    public class ProcessCollection
    {
        private readonly Dictionary<string, CommandHandler> collection;
        private readonly IHubContext<CommandHub> hub;

        public ProcessCollection(IHubContext<CommandHub> hub)
        {
            collection = new Dictionary<string, CommandHandler>();
            this.hub = hub;
        }

        public CommandHandler GetHandler(string id)
        {
            collection.TryGetValue(id, out CommandHandler? handler);

            if (handler == null)
            {
                handler = new CommandHandler(hub, id);
                collection[id] = handler;
            }

            return handler;
        }

        public void DisposeHandlerAsync(string id)
        {
            collection.TryGetValue(id, out CommandHandler? handler);

            if (handler is not null)
            {
                handler.Dispose();
                collection.Remove(id);
            }
        }
    }
}
