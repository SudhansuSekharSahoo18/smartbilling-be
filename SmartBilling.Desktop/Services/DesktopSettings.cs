using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartBilling.Desktop.Services
{
    /// <summary>
    /// Desktop app settings, saved under %LOCALAPPDATA%\SmartBilling because the exe's folder may be
    /// read-only. An optional desktopsettings.json next to the exe supplies defaults for a new user.
    /// </summary>
    public class DesktopSettings
    {
        private const string FileName = "desktopsettings.json";

        public static readonly string AppDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartBilling");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        // Empty runs the built-in server (single PC). Set it to use a shared SmartBillingServer on the network.
        public string ServerUrl { get; set; } = "";

        [JsonIgnore]
        public bool UsesBuiltInServer => string.IsNullOrWhiteSpace(ServerUrl);
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
