using SmartBillingServer;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var app = ServerHost.Build(new ServerHostOptions { Args = args });

            var uri = app.Environment.IsDevelopment() ? "https://localhost:7271" : "http://localhost:5000";

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {uri}")
                {
                    CreateNoWindow = true
                });
            });

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
}
