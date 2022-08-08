using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.ServerManagementTool.Terraria
{
    public interface IServerProcess
    {
        event EventHandler<DataReceivedEventArgs> OnErrorReceived;
        event EventHandler<DataReceivedEventArgs> OnOutputReceived;
        event EventHandler ServerStopped;
        event EventHandler<ServerInfoEventArgs> ServerRefreshed;

        string Executable { get; }
        string Arguments { get; }

        Encoding InputEncoding { get; }
        Encoding OutputEncoding { get; }

        ServerState ServerState { get; }

        void Run();
        void Stop(bool save);
        void SendInput(string input);
    }

    public enum ServerState
    {
        Stopped,
        Starting,
        Running,
        Stopping
    }
}
