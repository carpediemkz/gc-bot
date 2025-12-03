using System;
using System.Configuration;
using System.Data;
using System.Windows;
using gc_bot.Requests;

namespace gc_bot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Basic service provider without external DI library.
            Services = new SimpleServiceProvider();

            // Resolve MainWindow and show.
            var main = new MainWindow();
            main.Show();
        }

        // Minimal IServiceProvider implementation used to resolve IRequestService.
        private sealed class SimpleServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                if (serviceType == typeof(IRequestService))
                    return new ApiClient();

                return null;
            }
        }
    }
}
