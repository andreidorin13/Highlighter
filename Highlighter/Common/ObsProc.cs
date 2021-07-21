using Highlighter.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using OBS.WebSocket.NET;

namespace Highlighter.Common {
    public sealed class ObsProc : IDisposable {

        #region Fields

        private readonly ObsWebSocket _socket = new ObsWebSocket();

        private readonly ManualResetEventSlim _connected = new ManualResetEventSlim(false);

        private readonly Settings _settings;

        private Process _process;

        private bool _disposed = false;

        #endregion

        public ObsProc(Settings settings) {
            _settings = settings;

            _socket.Connected += (s, e) => _connected?.Set();
            _socket.Disconnected += (s, e) => _connected?.Reset();
        }

        public async ValueTask Start() {
            _process = new Process() {
                StartInfo = {
                    FileName = _settings.Obs.Path,
                    WorkingDirectory = Path.GetDirectoryName(_settings.Obs.Path),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            _process.Start();

            await WaitForConnection(new Uri(_settings.Obs.Uri));
            _socket.Connect(_settings.Obs.Uri, _settings.Obs.Password);
            _connected.Wait();
        }

        private async ValueTask WaitForConnection(Uri uri) {
            while (true) {
                var ports = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                foreach (var port in ports) {
                    if (port.Port == uri.Port)
                        return;
                }
                await Task.Delay(100);
            }
        }

        public async ValueTask ReplayGame(GameSettings game, Process process) {
            var className = Native.GetMainWindowClassName(process);
            while (className == null) {
                Debug.WriteLine($"No classname yet for {process.ProcessName}");
                className = Native.GetMainWindowClassName(process);
                await Task.Delay(100);
            }

            var windowId = $"{process.MainWindowTitle}:{className}:{process.ProcessName}.exe";
            _socket.Api.SetSourceSettings(_settings.Obs.SourceName, JObject.FromObject(new { capture_mode = "window", window = windowId }));

            string dir = Path.Combine(_settings.Obs.RecordDir, game.Name);
            Directory.CreateDirectory(dir);

            _socket.Api.SetRecordingFolder(dir);
            _socket.Api.StartReplayBuffer();
        }

        public void Dispose() {
            if (!_disposed) {
                try {
                    _socket.Api.StopReplayBuffer();
                }
                catch (Exception) { } // Buffer already stopped

                _connected?.Dispose();
                Thread.Sleep(300); // Give the buffer a second to stop, prevent exit dialog
                _process?.CloseMainWindow();
                _process?.Dispose();

                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
