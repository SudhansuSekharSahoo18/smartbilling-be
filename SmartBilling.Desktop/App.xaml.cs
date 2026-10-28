using SmartBilling.Desktop.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace SmartBilling.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Per Windows session: two copies would run two built-in servers over the same database.
        private const string SingleInstanceMutexName = @"Local\SmartBilling.Desktop.SingleInstance";
        private const int SW_RESTORE = 9;

        private Mutex? _singleInstanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _singleInstanceMutex = new Mutex(initiallyOwned: true, SingleInstanceMutexName, out var createdNew);
            if (!createdNew)
            {
                ActivateRunningInstance();
                _singleInstanceMutex.Dispose();
                _singleInstanceMutex = null;
                Shutdown();
                return;
            }

            DispatcherUnhandledException += (_, args) =>
            {
                var logFile = DesktopLog.Write(args.Exception);
                MessageBox.Show($"Something went wrong: {args.Exception.Message}\n\nDetails: {logFile}",
                    "Smart Billing", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            new MainWindow().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _singleInstanceMutex?.ReleaseMutex();
            _singleInstanceMutex?.Dispose();
            base.OnExit(e);
        }

        private static void ActivateRunningInstance()
        {
            var current = Process.GetCurrentProcess();
            var running = Process.GetProcessesByName(current.ProcessName)
                .FirstOrDefault(p => p.Id != current.Id && p.MainWindowHandle != IntPtr.Zero);
            if (running == null)
                return;
            // Restoring a window that isn't minimized would un-maximize it.
            if (IsIconic(running.MainWindowHandle))
                ShowWindow(running.MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(running.MainWindowHandle);
        }

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }

}
