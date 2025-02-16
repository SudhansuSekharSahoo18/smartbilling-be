using SmartBilling.Desktop.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartBilling.Desktop.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private UserModel _user;
        private string _displayName;

        public UserViewModel()
        {
            _user = new UserModel();
            SubmitCommand = new RelayCommand(Submit);
        }

        public string Name
        {
            get => _user.Name;
            set
            {
                _user.Name = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; }

        private void Submit()
        {
            DisplayName = $"Hello, {Name}!";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
