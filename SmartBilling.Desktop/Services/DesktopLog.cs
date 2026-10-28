using System.IO;

namespace SmartBilling.Desktop.Services
{
    /// <summary>
    /// Writes errors to %LOCALAPPDATA%\SmartBilling\Logs, since the single-exe app has no console.
    /// </summary>
    public static class DesktopLog
    {
        public static string Write(Exception ex)
        {
            try
            {
                var folder = Path.Combine(DesktopSettings.AppDataFolder, "Logs");
                Directory.CreateDirectory(folder);
                var file = Path.Combine(folder, $"log{DateTime.Now:yyyyMMdd-HHmmss}.txt");
                File.AppendAllText(file, $"{DateTime.Now:O}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
                return file;
            }
            catch (IOException)
            {
                return "(the log file could not be written)";
            }
            catch (UnauthorizedAccessException)
            {
                return "(the log file could not be written)";
            }
        }
    }
}
