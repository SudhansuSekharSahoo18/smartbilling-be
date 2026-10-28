using System.Windows;

namespace SmartBilling.Desktop.Views
{
    /// <summary>
    /// Modal list of installed printers for choosing the barcode label printer.
    /// </summary>
    public partial class PrinterPickerWindow : Window
    {
        public PrinterPickerWindow(IReadOnlyList<string> printers, string currentPrinter)
        {
            InitializeComponent();
            PrinterList.ItemsSource = printers;
            PrinterList.SelectedItem = printers.Contains(currentPrinter) ? currentPrinter : null;
        }

        public string? SelectedPrinter => PrinterList.SelectedItem as string;

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedPrinter == null)
            {
                MessageBox.Show(this, "Select a printer.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DialogResult = true;
        }
    }
}
