using Highlighter.Common;
using Highlighter.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Highlighter.Services {
    public class ProcessWatcher : BackgroundService {

        private readonly Settings _settings;
        private readonly ObsProc _obs;

        private int? _currentPid = null;

        public ProcessWatcher(IOptions<Settings> settings) {
            _settings = settings.Value;
            _obs = new ObsProc(_settings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await _obs.Start();

            while (!stoppingToken.IsCancellationRequested) {
                // If current game is still up, keep going
                if (_currentPid.HasValue) {
                    using var current = Process.GetProcessById(_currentPid.Value);
                    if (!current.HasExited)
                        goto End;
                }

                // Otherwise look for new game
                foreach (var process in Process.GetProcesses()) {
                    foreach (var game in _settings.Games) {
                        if (game.Exe == process.ProcessName) {
                            _currentPid = process.Id;
                            await _obs.ReplayGame(game, process);
                            break;
                        }
                    }
                }
                End:
                await Task.Delay(_settings.PollInterval, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken) {
            _obs.Dispose();
        }
    }
}
