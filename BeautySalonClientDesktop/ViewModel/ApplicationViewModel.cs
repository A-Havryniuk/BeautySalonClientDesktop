using BeautySalonClientDesktop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BeautySalonClientDesktop.Client;
using BeautySalonClientDesktop.Infrastructure;

namespace BeautySalonClientDesktop.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private ClientAccess _client;
        private ClientModel selectedClient;
        ObservableCollection<ClientModel> clients;
        private RelayCommand _deleteCommand;

        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                       (_deleteCommand = new RelayCommand(async obj => { await DeleteAsync(obj); }, CanDelete));
            }
        }

        private bool CanDelete(object parameter)
        {
            return true;
        }

        private async Task DeleteAsync(object parameter)
        {
            try
            {
                if (parameter is ClientModel selectedClient)
                {
                    await _client.DeleteClientAsync(selectedClient.Id);
                    Clients.Remove(selectedClient);
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<ClientModel> Clients { get => clients; set => clients = value; }

        public ClientModel SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                OnPropertyChanged("SelectedClient");
            }
        }

        public ApplicationViewModel(ClientAccess access)
        {
            _client = access;
            clients = new ObservableCollection<ClientModel>();
            InitializeClientsDataAsync();
        }


        private async void InitializeClientsDataAsync()
        {
            var clientDTOs = await _client.GetAllClientsAsync();
            foreach (var clientDTO in clientDTOs)
            {
                // Convert ClientDTO to ClientModel as needed
                var clientModel = new ClientModel
                {
                    Id = clientDTO.Id,
                    Name = clientDTO.Name,
                    Surname = clientDTO.Surname,
                    PhoneNumber = clientDTO.PhoneNumber,
                    Email = clientDTO.Email,
                    Address = clientDTO.Address
                };

                Clients.Add(clientModel);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // Add commands for add, edit, delete operations

        // Implement methods to load, add, edit, and delete clients

        // Implement INotifyPropertyChanged interface
    }
}
