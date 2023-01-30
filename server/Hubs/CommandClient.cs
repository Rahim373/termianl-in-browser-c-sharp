namespace Terminal.Web.Hubs
{
    public interface ICommandClient
    {
        Task<string> ReceiveResponse(string command);
    }
}
