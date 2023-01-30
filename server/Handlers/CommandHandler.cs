using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Terminal.Web.Hubs;

namespace Terminal.Web.Handlers
{
    public class CommandHandler : IDisposable
    {
        private readonly Process _cmdProcess;
        private readonly StreamWriter _streamWriter;
        private readonly IHubContext<CommandHub> _hub;
        private readonly string _connectionId;

        public CommandHandler(IHubContext<CommandHub> hub, string connectionId)
        {
            _connectionId = connectionId;
            _hub = hub;
            
            _cmdProcess = new Process();

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "cmd.exe" : "/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            _cmdProcess.OutputDataReceived += _cmdProcess_OutputDataReceived;
            _cmdProcess.Exited += _cmdProcess_Exited;
            _cmdProcess.StartInfo = processStartInfo;
            _cmdProcess.Start();

            _streamWriter = _cmdProcess.StandardInput;
            _cmdProcess.BeginOutputReadLine();
        }

        private void _cmdProcess_Exited(object? sender, EventArgs e)
        {
            Console.WriteLine("Exited");
        }

        private void _cmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Task.Run(async () => await _hub.Clients.Client(_connectionId).SendAsync("ReceiveResponse", e.Data));
        }

        public void ExecuteCommand(string command)
        {
            _streamWriter.WriteLine(command);
        }

        public void Dispose()
        {
            _cmdProcess.Close();
            _cmdProcess.Dispose();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }
    }
}
