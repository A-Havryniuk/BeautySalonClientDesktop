using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using BeautySalonClientDesktop.Infrastructure;
using System.Runtime.CompilerServices;
using BeautySalonClientDesktop.Model;
using System.Windows.Controls;
using System.Security;
using BeautySalonClientDesktop.Client;
using BeautySalonClientDesktop.View;

namespace BeautySalonClientDesktop.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private UserModel _userModel;
        private ClientAccess _client = new("https://localhost:7103/", new HttpClient());
        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public UserModel UserModel
        {
            get => _userModel ??= new UserModel();
            set
            {
                _userModel = value;
                OnPropertyChanged(nameof(UserModel));
            }
        }

        private RelayCommand _loginCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                       (_loginCommand = new RelayCommand(async obj => { await LoginAsync(obj); }, CanLogin));
            }
        }

        private bool CanLogin(object parameter)
        {
            // Add validation logic if needed
            return !string.IsNullOrEmpty(_userModel.Username) && _userModel.Password != null && _userModel.Password.Length > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task LoginAsync(object parameter)
        {
            try
            {
                var tokenDTO = await _client.LoginAdminAsync(new LoginDTO()
                {
                    Email = UserModel.Username,
                    Password = UserModel.Password
                });
                await _client.AddAuthorizeHeader(tokenDTO.Token);
                _client.Id = tokenDTO.Id;
                var wind = new MainWindow(_client);
                Application.Current.MainWindow.Close();
                wind.Show();

            }
            catch (ApiException<ValidationProblemDetails> ex)
            {
                ErrorMessage = "ApiException. Please try again.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ErrorMessage = ex.Message;
            }
        }
    }
}
