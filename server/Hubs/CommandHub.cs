using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Channels;
using Terminal.Web.ViewModel;

namespace Terminal.Web.Hubs
{
    public class CommandHub : Hub
    {
        private readonly ProcessCollection _processCollection;

        public CommandHub(ProcessCollection processCollection)
        {
            _processCollection = processCollection;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _processCollection.DisposeHandlerAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public void SendCommand(CommandViewModel model)
        {
            var handler = _processCollection.GetHandler(Context.ConnectionId);
            handler.ExecuteCommand(model.Command);
        }
    }
}