using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SmartBillingServer;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var app = ServerHost.Build(new ServerHostOptions { Args = args });

            // In development the launch profile opens the React dev server instead.
            if (!app.Environment.IsDevelopment())
            {
                app.Lifetime.ApplicationStarted.Register(() => OpenBrowser(app));
            }

            app.Run();
        }
        // HostAbortedException is how EF Core tools (dotnet ef) stop the app after reading its services; not a crash.
        catch (Exception ex) when (ex is not HostAbortedException)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"log{DateTime.Now.ToFileTime()}.txt");
            File.AppendAllText(filePath, ex.Message);
            throw;
        }

    }

    // Opens the address the server actually listens on (any port, http or https).
    private static void OpenBrowser(WebApplication app)
    {
        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses
            .OrderBy(x => x.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .FirstOrDefault();
        if (address == null)
        {
            return;
        }

        // A server listening on all addresses (http://0.0.0.0:5000, http://[::]:5000, http://+:5000) is opened on this PC.
        var uri = new UriBuilder(address.Replace("://+", "://localhost").Replace("://*", "://localhost"));
        if (uri.Host is "0.0.0.0" or "[::]" or "::")
        {
            uri.Host = "localhost";
        }

        Process.Start(new ProcessStartInfo(uri.Uri.ToString()) { UseShellExecute = true });
    }
}
