using Highlighter.Models;
using Highlighter.Services;
using Highlighter.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace Highlighter {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private readonly IHost host;

        public static IServiceProvider ServiceProvider { get; private set; }

        public App() {
            host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((c, b) => b.AddJsonFile("settings.json"))
                .ConfigureServices((c, s) => {
                    s.Configure<Settings>(c.Configuration);
                    s.AddHostedService<ProcessWatcher>();
                    s.AddSingleton<MainViewModel>();
                    s.AddTransient<MainWindow>();
                    s.Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromSeconds(1));
                })
                .ConfigureLogging(l => { })
                .Build();

            ServiceProvider = host.Services;
        }

        protected override async void OnStartup(StartupEventArgs e) {
            await host.StartAsync();
            host.Services.GetRequiredService<MainWindow>().Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e) {
            using (host)
                await host.StopAsync();

            base.OnExit(e);
        }
    }
}
