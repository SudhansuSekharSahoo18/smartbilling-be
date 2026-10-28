using Microsoft.Web.WebView2.Core;
using SmartBilling.Desktop.Services;
using SmartBilling.Desktop.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace SmartBilling.Desktop
{
    /// <summary>
    /// Hosts the SmartBilling web app in WebView2, served by the built-in server or by a
    /// SmartBillingServer on the network when a server URL is set.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WebView2DownloadUrl = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(5) };

        private readonly DesktopSettings _settings = DesktopSettings.Load();
        private readonly EmbeddedServer _embeddedServer = new();
        private readonly MainViewModel _viewModel;
        private LabelPrintService? _labelPrintService;
        private Uri? _serverUri;
        private bool _webViewReady;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(_settings.ServerUrl, ConnectAsync, SaveServerUrlAndConnectAsync);
            DataContext = _viewModel;
            Loaded += async (_, _) => await InitializeWebViewAsync();
        }

        private async Task InitializeWebViewAsync()
        {
            _viewModel.ShowConnecting();
            try
            {
                var environment = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: Path.Combine(DesktopSettings.AppDataFolder, "WebView2"));
                await AppView.EnsureCoreWebView2Async(environment);
            }
            catch (WebView2RuntimeNotFoundException)
            {
                _viewModel.ShowError(
                    $"Microsoft Edge WebView2 Runtime is not installed. Install it from {WebView2DownloadUrl} and restart Smart Billing.");
                return;
            }

            var core = AppView.CoreWebView2;
            core.NavigationStarting += OnNavigationStarting;
            core.NavigationCompleted += OnNavigationCompleted;
            core.NewWindowRequested += OnNewWindowRequested;

            _labelPrintService = new LabelPrintService(_settings, this);
            core.WebMessageReceived += (_, e) =>
            {
                var reply = _labelPrintService.HandleMessage(e.WebMessageAsJson);
                if (reply != null)
                    core.PostWebMessageAsJson(reply);
            };

            _webViewReady = true;
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            if (!_webViewReady)
                return;

            var url = _viewModel.ServerUrl.Trim();
            var uri = url == "" ? await StartBuiltInServerAsync() : await CheckServerUrlAsync(url);
            if (uri == null)
                return;

            _serverUri = uri;
            _viewModel.HideOverlay();
            AppView.CoreWebView2.Navigate(uri.AbsoluteUri);
        }

        private async Task<Uri?> StartBuiltInServerAsync()
        {
            _viewModel.ShowConnecting();
            try
            {
                return await _embeddedServer.StartAsync();
            }
            catch (Exception ex)
            {
                var logFile = DesktopLog.Write(ex);
                _viewModel.ShowError($"Smart Billing could not start.\n\n{ex.Message}\n\nDetails: {logFile}");
                return null;
            }
        }

        private async Task<Uri?> CheckServerUrlAsync(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                _viewModel.ShowError("Enter a valid server URL, for example http://192.168.1.10:5000, or leave it blank to use the built-in server.");
                return null;
            }

            _viewModel.ShowConnecting();
            try
            {
                using var response = await Http.GetAsync(uri);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _viewModel.ShowError(
                    $"Cannot reach the SmartBilling server at {url}. Check that the server is running and the address is correct, or clear the URL to use the built-in server.\n\n{ex.Message}");
                return null;
            }
            return uri;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Off the UI thread: StopAsync's continuations must not wait for this (blocked) dispatcher.
            try
            {
                Task.Run(_embeddedServer.StopAsync).Wait(TimeSpan.FromSeconds(6));
            }
            catch (AggregateException ex)
            {
                DesktopLog.Write(ex);
            }
        }

        private async Task SaveServerUrlAndConnectAsync()
        {
            _settings.ServerUrl = _viewModel.ServerUrl.Trim();
            _settings.Save();
            await ConnectAsync();
        }

        // Keep the window on the SmartBilling server; anything else opens in the default browser.
        private bool IsAppUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;
            if (uri.Scheme is "blob" or "data" or "about")
                return true;
            return _serverUri != null
                && Uri.Compare(uri, _serverUri, UriComponents.SchemeAndServer, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (IsAppUrl(e.Uri))
                return;
            e.Cancel = true;
            OpenInDefaultBrowser(e.Uri);
        }

        private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess && e.WebErrorStatus != CoreWebView2WebErrorStatus.OperationCanceled)
                _viewModel.ShowError($"Could not load the SmartBilling app from {_serverUri} ({e.WebErrorStatus}).");
        }

        private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            if (IsAppUrl(e.Uri))
                AppView.CoreWebView2.Navigate(e.Uri);
            else
                OpenInDefaultBrowser(e.Uri);
        }

        private static void OpenInDefaultBrowser(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        }
    }
}
