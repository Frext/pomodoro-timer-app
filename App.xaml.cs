using System.Threading;
using System.Windows;

namespace PomodoroApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Got help from https://stackoverflow.com/a/34231251/15555081
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e) // Just open one single instance at a time.
        {
            const string appName = "TheatrePlayMusicController";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // App is already running! Exiting the application
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
