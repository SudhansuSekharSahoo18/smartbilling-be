using SmartBilling.Desktop.Views;
using System.Printing;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartBilling.Desktop.Services
{
    /// <summary>
    /// Handles barcode label requests from the web app and prints them silently to the saved label printer.
    /// </summary>
    /// <remarks>
    /// Labels are drawn and printed with WPF rather than WebView2: Chromium printing rotates pages that are
    /// wider than tall (as labels are) by 90°, while a WPF print ticket passes the exact portrait page size
    /// to the printer driver.
    /// </remarks>
    public class LabelPrintService
    {
        private const double MmPerInch = 25.4;
        private const double DipsPerInch = 96;
        private const double PointsPerInch = 72;
        private const int DefaultPrinterDpi = 203;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly DesktopSettings _settings;
        private readonly Window _owner;

        public LabelPrintService(DesktopSettings settings, Window owner)
        {
            _settings = settings;
            _owner = owner;
        }

        private record LabelElement(
            string? Type, double XMm, double YMm, double WMm, double HMm,
            string? Text, double FontSizePt, bool Bold, string? Align, string? Modules);

        private record LabelJob(double PageWidthMm, double PageHeightMm, int LabelCount, List<List<LabelElement>>? Pages);

        private record BridgeMessage(string? Type, string? RequestId, LabelJob? Job);

        /// <summary>Returns the JSON reply to post back to the page, or null if the message is not ours.</summary>
        public string? HandleMessage(string messageJson)
        {
            BridgeMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<BridgeMessage>(messageJson, JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }

            switch (message?.Type)
            {
                case "printLabels":
                    var (ok, text) = PrintLabels(message.Job);
                    return JsonSerializer.Serialize(
                        new { type = "printLabelsResult", requestId = message.RequestId, ok, message = text }, JsonOptions);

                case "chooseLabelPrinter":
                    var printer = ChooseLabelPrinter();
                    return JsonSerializer.Serialize(
                        new { type = "chooseLabelPrinterResult", requestId = message.RequestId, ok = printer != null, printer }, JsonOptions);

                default:
                    return null;
            }
        }

        private (bool Ok, string Message) PrintLabels(LabelJob? job)
        {
            if (job?.Pages == null || job.Pages.Count == 0)
                return (false, "There are no labels to print.");
            if (job.PageWidthMm is < 10 or > 500 || job.PageHeightMm is < 10 or > 500)
                return (false, "The label size is not valid. Check it on the Label Design page.");

            var printerName = _settings.LabelPrinter;
            if (string.IsNullOrEmpty(printerName) || !GetInstalledPrinters().Contains(printerName))
            {
                printerName = ChooseLabelPrinter();
                if (printerName == null)
                    return (false, "Printing cancelled: no label printer selected.");
            }

            try
            {
                using var printServer = new LocalPrintServer();
                using var queue = FindQueue(printServer, printerName);
                if (queue == null)
                    return (false, $"Printer '{printerName}' was not found.");

                var requested = new PrintTicket
                {
                    PageMediaSize = new PageMediaSize(MmToDip(job.PageWidthMm), MmToDip(job.PageHeightMm)),
                    PageOrientation = PageOrientation.Portrait,
                    CopyCount = 1,
                };
                var ticket = queue.MergeAndValidatePrintTicket(queue.UserPrintTicket ?? queue.DefaultPrintTicket, requested).ValidatedPrintTicket;
                var dpi = ticket.PageResolution?.X ?? DefaultPrinterDpi;

                var document = BuildDocument(job, dpi);
                PrintQueue.CreateXpsDocumentWriter(queue).Write(document, ticket);

                var message = $"{job.LabelCount} {(job.LabelCount == 1 ? "label" : "labels")} sent to {printerName}";
                return (true, message + PaperSizeWarning(ticket, job));
            }
            catch (PrintingCanceledException)
            {
                return (false, "Printing was cancelled.");
            }
            catch (Exception ex) when (ex is PrintQueueException or PrintSystemException or InvalidOperationException)
            {
                return (false, $"Printing to '{printerName}' failed: {ex.Message}");
            }
        }

        // Some drivers replace a custom size with their nearest supported paper; say so, since labels
        // then land across label gaps.
        private static string PaperSizeWarning(PrintTicket ticket, LabelJob job)
        {
            var size = ticket.PageMediaSize;
            if (size?.Width == null || size.Height == null)
                return "";
            var widthMm = DipToMm(size.Width.Value);
            var heightMm = DipToMm(size.Height.Value);
            if (Math.Abs(widthMm - job.PageWidthMm) < 1 && Math.Abs(heightMm - job.PageHeightMm) < 1)
                return "";
            return $". Note: the printer is using {widthMm:0.#} × {heightMm:0.#} mm paper instead of {job.PageWidthMm:0.#} × {job.PageHeightMm:0.#} mm; set the label size in the printer's preferences.";
        }

        private static FixedDocument BuildDocument(LabelJob job, int dpi)
        {
            var pageSize = new Size(MmToDip(job.PageWidthMm), MmToDip(job.PageHeightMm));
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            foreach (var elements in job.Pages!)
            {
                var page = new FixedPage { Width = pageSize.Width, Height = pageSize.Height, Background = Brushes.White };
                foreach (var element in elements)
                {
                    var visual = element.Type == "barcode" ? CreateBarcode(element, dpi) : CreateText(element);
                    FixedPage.SetLeft(visual, MmToDip(element.XMm));
                    FixedPage.SetTop(visual, MmToDip(element.YMm));
                    page.Children.Add(visual);
                }
                var content = new PageContent();
                ((IAddChild)content).AddChild(page);
                document.Pages.Add(content);
            }
            return document;
        }

        // Text vertically centred in its box and clipped to it, matching the CSS preview (line-height = box height).
        private static FrameworkElement CreateText(LabelElement element)
        {
            var text = new TextBlock
            {
                Text = element.Text ?? "",
                FontFamily = new FontFamily("Arial"),
                FontSize = Math.Max(1, element.FontSizePt) * DipsPerInch / PointsPerInch,
                FontWeight = element.Bold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.NoWrap,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = element.Align switch
                {
                    "left" => TextAlignment.Left,
                    "right" => TextAlignment.Right,
                    _ => TextAlignment.Center,
                },
            };
            return new Grid
            {
                Width = MmToDip(element.WMm),
                Height = MmToDip(element.HMm),
                ClipToBounds = true,
                Children = { text },
            };
        }

        // Bars are drawn on whole printer dots (each module the same number of dots), so they stay crisp and
        // in proportion for scanning. The barcode is centred in its box.
        private static FrameworkElement CreateBarcode(LabelElement element, int dpi)
        {
            var box = new Canvas { Width = MmToDip(element.WMm), Height = MmToDip(element.HMm), ClipToBounds = true };
            var modules = element.Modules ?? "";
            if (modules.Length == 0)
                return box;

            var dipsPerDot = DipsPerInch / dpi;
            var boxWidthDots = (int)Math.Floor(box.Width / dipsPerDot);
            var dotsPerModule = Math.Max(1, boxWidthDots / modules.Length);
            var startDot = Math.Max(0, (boxWidthDots - dotsPerModule * modules.Length) / 2);

            var bars = new GeometryGroup();
            for (var i = 0; i < modules.Length;)
            {
                if (modules[i] != '1') { i++; continue; }
                var runStart = i;
                while (i < modules.Length && modules[i] == '1') i++;
                var x = (startDot + runStart * dotsPerModule) * dipsPerDot;
                var width = (i - runStart) * dotsPerModule * dipsPerDot;
                bars.Children.Add(new RectangleGeometry(new Rect(x, 0, width, box.Height)));
            }

            var path = new Path { Data = bars, Fill = Brushes.Black };
            RenderOptions.SetEdgeMode(path, EdgeMode.Aliased);
            box.Children.Add(path);
            return box;
        }

        private static double MmToDip(double mm) => mm / MmPerInch * DipsPerInch;
        private static double DipToMm(double dip) => dip / DipsPerInch * MmPerInch;

        private string? ChooseLabelPrinter()
        {
            var picker = new PrinterPickerWindow(GetInstalledPrinters(), _settings.LabelPrinter) { Owner = _owner };
            if (picker.ShowDialog() != true || picker.SelectedPrinter == null)
                return null;

            _settings.LabelPrinter = picker.SelectedPrinter;
            _settings.Save();
            return picker.SelectedPrinter;
        }

        private static PrintQueueCollection GetQueues(LocalPrintServer printServer) =>
            printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

        private static PrintQueue? FindQueue(LocalPrintServer printServer, string printerName) =>
            GetQueues(printServer).FirstOrDefault(queue => queue.FullName == printerName);

        private static List<string> GetInstalledPrinters()
        {
            using var printServer = new LocalPrintServer();
            return GetQueues(printServer)
                .Select(queue => queue.FullName)
                .OrderBy(name => name)
                .ToList();
        }
    }
}
