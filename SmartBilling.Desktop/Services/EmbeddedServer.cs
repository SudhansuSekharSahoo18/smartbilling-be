using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SmartBillingServer;
using System.IO;

namespace SmartBilling.Desktop.Services
{
    /// <summary>
    /// Runs SmartBillingServer inside the desktop app, so a single PC needs nothing but this exe.
    /// The database lives in %LOCALAPPDATA%\SmartBilling and the web UI is embedded in the exe
    /// (see the EmbedClientBuild target in the project file).
    /// </summary>
    public sealed class EmbeddedServer
    {
        private const string WebRootResourcePrefix = "wwwroot";

        public static string DatabasePath => Path.Combine(DesktopSettings.AppDataFolder, "SmartBilling.db");

        private WebApplication? _app;

        public Uri? Address { get; private set; }

        public static bool HasEmbeddedUi =>
            typeof(EmbeddedServer).Assembly.GetManifestResourceInfo($"{WebRootResourcePrefix}.index.html") != null;

        public async Task<Uri> StartAsync()
        {
            if (Address != null)
                return Address;
            if (!HasEmbeddedUi)
                throw new InvalidOperationException(
                    "This build of Smart Billing doesn't contain the web app. Build it in Release (which builds Client/), or enter a server URL.");

            Directory.CreateDirectory(DesktopSettings.AppDataFolder);

            // Building the app applies database migrations, so keep it off the UI thread.
            var app = await Task.Run(() => ServerHost.Build(new ServerHostOptions
            {
                ContentRootPath = DesktopSettings.AppDataFolder,
                DatabasePath = DatabasePath,
                // Loopback only, on a free port: nothing else on the network can reach it, and it can't clash with other apps.
                Urls = "http://127.0.0.1:0",
                WebRootFileProvider = new EmbeddedFileProvider(typeof(EmbeddedServer).Assembly, WebRootResourcePrefix),
                UseHttpsRedirection = false,
            }));
            try
            {
                await app.StartAsync();
                var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault()
                    ?? throw new InvalidOperationException("The built-in server started without an address.");
                _app = app;
                Address = new Uri(address);
                return Address;
            }
            catch
            {
                await app.DisposeAsync();
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (_app == null)
                return;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await _app.StopAsync(timeout.Token);
            await _app.DisposeAsync();
            _app = null;
            Address = null;
        }
    }
}
