using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartBilling.Desktop.ViewModels
{
    /// <summary>
    /// State for the connection overlay shown while the app connects to the server or cannot reach it.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _serverUrl;
        private string _statusMessage = "";
        private bool _isOverlayVisible = true;
        private bool _isBusy;

        public MainViewModel(string serverUrl, Func<Task> retry, Func<Task> saveAndRetry)
        {
            _serverUrl = serverUrl;
            RetryCommand = new RelayCommand(async () => await retry(), () => !IsBusy);
            SaveAndRetryCommand = new RelayCommand(async () => await saveAndRetry(), () => !IsBusy);
        }

        public string ServerUrl
        {
            get => _serverUrl;
            set { _serverUrl = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            private set { _isOverlayVisible = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand RetryCommand { get; }
        public ICommand SaveAndRetryCommand { get; }

        public void ShowConnecting()
        {
            StatusMessage = $"Connecting to {ServerUrl}…";
            IsBusy = true;
            IsOverlayVisible = true;
        }

        public void ShowError(string message)
        {
            StatusMessage = message;
            IsBusy = false;
            IsOverlayVisible = true;
        }

        public void HideOverlay()
        {
            IsBusy = false;
            IsOverlayVisible = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
