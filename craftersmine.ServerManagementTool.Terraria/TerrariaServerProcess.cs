using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace craftersmine.ServerManagementTool.Terraria
{
    public sealed class TerrariaServerProcess : IServerProcess, IDisposable
    {
        private Process _proc;
        private readonly DispatcherTimer _timer;
        private ServerInfoEventArgs _info;
        private DateTime _lastTime;
        private TimeSpan _lastTotalProcessorTime;
        private ServerState _serverState = ServerState.Stopped;

        public event EventHandler<ServerInfoEventArgs>? ServerRefreshed;
        public string Executable { get; }
        public string Arguments => _proc.StartInfo.Arguments;
        public string ConfigFile { get; }
        public Encoding InputEncoding => Encoding.Unicode;
        public Encoding OutputEncoding => Encoding.UTF8;
        public ServerState ServerState => _serverState;
        public ServerConsole CurrentConsole { get; private set; }

        public event EventHandler<DataReceivedEventArgs>? OnErrorReceived;
        public event EventHandler<DataReceivedEventArgs>? OnOutputReceived;
        public event EventHandler? ServerStopped;

        public TerrariaServerProcess(string executable, string configFile)
        {
            CurrentConsole = new ServerConsole();

            _info = new ServerInfoEventArgs()
            {
                CpuUsage = 0d,
                MemUsage = 0d,
                ProcessPriority = ProcessPriorityClass.Normal
            };

            Executable = executable;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;

            ConfigFile = configFile;

            ProcessStartInfo procStartInfo =
                new ProcessStartInfo(Executable);
            procStartInfo.Arguments = "-config " + GetConfigFilePath();
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            procStartInfo.StandardErrorEncoding = OutputEncoding;
            procStartInfo.StandardInputEncoding = InputEncoding;
            procStartInfo.StandardOutputEncoding = OutputEncoding;
            procStartInfo.UseShellExecute = false;

            _proc = new Process();
            _proc.EnableRaisingEvents = true;
            _proc.StartInfo = procStartInfo;
            _proc.Exited += ProcOnExited;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            _info.MemUsage = _proc.VirtualMemorySize64;

            double cpuUsage = _proc.TotalProcessorTime.Subtract(_lastTotalProcessorTime).Divide(DateTime.Now.Subtract(_lastTime).TotalMilliseconds).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);

            _lastTotalProcessorTime = _proc.TotalProcessorTime;
            _lastTime = DateTime.Now;

            _info.CpuUsage = cpuUsage;
            _info.ProcessPriority = _proc.PriorityClass;
            _info.ServerState = ServerState;

            ServerRefreshed?.Invoke(this, _info);
            StaticData.RequestRefresh();
        }

        public async void Run()
        {
            CurrentConsole.Clear();
            _serverState = ServerState.Starting;
            _timer.Start();
            _proc.OutputDataReceived += ProcOnOutputDataReceived;
            _proc.ErrorDataReceived += ProcOnErrorDataReceived;
            _proc.Start();
            _proc.StandardInput.AutoFlush = true;
            _proc.BeginOutputReadLine();
            _proc.BeginErrorReadLine();
            await _proc.WaitForExitAsync();
        }

        public void Stop(bool save)
        {
            _serverState = ServerState.Stopping;
            if (save)
                SendInput("exit");
            else
                SendInput("exit-nosave");
        }

        public void Kill()
        {
            _proc.Kill();
        }

        public void SendInput(string input)
        {
            _proc.StandardInput.WriteLine(input);
        }

        private void ProcOnExited(object? sender, EventArgs e)
        {
            _timer.Stop();
            ServerStopped?.Invoke(sender, e);
            _info.MemUsage = 0;
            _info.CpuUsage = 0;
            _info.ProcessPriority = ProcessPriorityClass.Idle;
            _info.ServerState = ServerState.Stopped;
            ServerRefreshed?.Invoke(this, _info);
            _proc.CancelErrorRead();
            _proc.CancelOutputRead();
            _proc.OutputDataReceived -= ProcOnOutputDataReceived;
            _proc.ErrorDataReceived -= ProcOnErrorDataReceived;
        }

        private void ProcOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnErrorReceived?.Invoke(this, e);
        }

        private void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                string data = e.Data;
                if (data.StartsWith(": "))
                    data = data.Substring(2);
                if (data.ToLower() == "server started")
                    _serverState = ServerState.Running;

                CurrentConsole.Add(data);
            }

            OnOutputReceived?.Invoke(this, e);
        }

        public string GetConfigFilePath()
        {
            if (StaticData.CurrentServerInstance!.UseConfigFile)
            {
                return StaticData.CurrentServerInstance.ConfigFile;
            }

            string path = Path.Combine(StaticData.AppDataRoot, "tempConfig.cfg");
            if (StaticData.CurrentServerInstance.Config is null)
            {
                StaticData.CurrentServerInstance.Config = ServerConfig.GenerateDefaultConfig();
                StaticData.CurrentServerInstance.Config.SaveToFile(path);
                return path;
            }

            StaticData.CurrentServerInstance.Config.SaveToFile(path);
            return path;
        }

        public void Dispose()
        {
        }
    }
}
