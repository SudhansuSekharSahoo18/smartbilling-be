using System.IO;
using System.Text.Json;

namespace SmartBilling.Desktop.Services
{
    /// <summary>
    /// Desktop app settings. Defaults ship next to the exe; changes made in the app are
    /// saved under %LOCALAPPDATA%\SmartBilling because the install folder may be read-only.
    /// </summary>
    public class DesktopSettings
    {
        private const string FileName = "desktopsettings.json";

        public static readonly string AppDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartBilling");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public string ServerUrl { get; set; } = "http://localhost:5000";
        public string LabelPrinter { get; set; } = "";

        public static DesktopSettings Load()
        {
            var userFile = Path.Combine(AppDataFolder, FileName);
            var defaultFile = Path.Combine(AppContext.BaseDirectory, FileName);

            foreach (var file in new[] { userFile, defaultFile })
            {
                try
                {
                    if (File.Exists(file))
                        return JsonSerializer.Deserialize<DesktopSettings>(File.ReadAllText(file)) ?? new DesktopSettings();
                }
                catch (JsonException)
                {
                    // A corrupt file falls through to the next one, then to the built-in defaults.
                }
            }
            return new DesktopSettings();
        }

        public void Save()
        {
            Directory.CreateDirectory(AppDataFolder);
            File.WriteAllText(Path.Combine(AppDataFolder, FileName), JsonSerializer.Serialize(this, JsonOptions));
        }
    }
}
