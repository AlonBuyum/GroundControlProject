using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfClient.Models;
using static WpfClient.Models.StateModel;

namespace WpfClient.ViewModel
{
    public class StateViewModel : ViewModelBase
    {
        private ObservableCollection<ViewPlane> _planes;
        public ObservableCollection<ViewPlane> Planes
        {
            get { return _planes; }
            set
            {
                if (_planes != value)
                {
                    _planes = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _messages;
        public string Messages
        {
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _slotNum;
        public string SlotNum
        {
            get {return _slotNum;}
            set
            {
                //makes sure input is not empty. if so, sends 1
                if (string.IsNullOrEmpty(_slotNum) || string.IsNullOrWhiteSpace(_slotNum))
                    _slotNum = "1";

                //makes sure input is a number. if not, sends 1
                _slotNum = int.TryParse(value, out int stateId) ? stateId.ToString() : "1";
                RaisePropertyChanged();
            }
        }

        private RelayCommand _getPlanesCommand;
        public RelayCommand GetPlanesCommand
        {
            get
            {
                return _getPlanesCommand ?? (_getPlanesCommand = new RelayCommand(async () =>
                {
                    await GetStateAsync();
                }));
            }
        }

        private RelayCommand _saveStateCommand;
        public RelayCommand SaveStateCommand
        {
            get
            {
                return _saveStateCommand ?? (_saveStateCommand = new RelayCommand(async () =>
                {
                    await SaveStateAsync();
                }));
            }
        }

        private RelayCommand _loadStateCommand;
        public RelayCommand LoadStateCommand
        {
            get
            {
                return _loadStateCommand ?? (_loadStateCommand = new RelayCommand(async () =>
                {
                    await LoadStateAsync();
                }));
            }
        }

        private RelayCommand _connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommand(() =>
                {
                    ConnectAsync();
                    IsActive = true;
                }));
            }
        }

        public StateModel State { get; set; }
        HttpClient client;
        HubConnection _connection;
        public StateViewModel()
        {
            string _baseUrl = "http://localhost:51404";
            client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);
            Planes = new ObservableCollection<ViewPlane>();
            State = new StateModel();
            SlotNum = default;
            IsActive = false;
            InitSignalR(_baseUrl);
        }

        private void InitSignalR(string _baseUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_baseUrl}/airportHub")
                .Build();
            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };
        }

        private async void ConnectAsync()
        {
            _connection.On<string>("ReceiveObject", (stateJson) =>
           {
               Dispatcher.CurrentDispatcher.Invoke(() =>

                 {
                     if (TryParseJson(stateJson, out JObject jObject))
                     {
                         var state = JsonConvert.DeserializeObject<ViewState>(stateJson);
                         State.GetState = state;
                         Messages = $"Got server response at {DateTime.Now} ";
                         Planes = new ObservableCollection<ViewPlane>(State.GetState.Planes.ToList());
                     }
                     else
                     {
                         Messages = $"Got server error: {stateJson}\nat {DateTime.Now}";
                     }
                 });
           });
            try
            {
                await _connection.StartAsync();
                Messages = "Connection started";
            }
            catch (Exception ex)
            {
                Messages = ex.Message;
            }
        }
        public async Task GetStateAsync()
        {
            var url = "statemanager";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var state = JsonConvert.DeserializeObject<ViewState>(content);
            State.GetState = state;

            Planes = new ObservableCollection<ViewPlane>(State.GetState.Planes.ToList());
        }

        public async Task SaveStateAsync()
        {
            var url = "statemanager";
            var response = await client.GetAsync($"{url}/{SlotNum}");
            var content = await response.Content.ReadAsStringAsync();
            var state = JsonConvert.DeserializeObject<ViewState>(content);
            State.GetState = state;

            Planes = new ObservableCollection<ViewPlane>(State.GetState.Planes.ToList());
        }

        public async Task LoadStateAsync()
        {
            var url = "statemanager";
            var response = await client.GetAsync($"{url}/load{SlotNum}");
            var content = await response.Content.ReadAsStringAsync();
            if (TryParseJson(content, out JObject jObject))
            {
                var state = JsonConvert.DeserializeObject<ViewState>(content);
                State.GetState = state;
            }

            Planes = new ObservableCollection<ViewPlane>(State.GetState.Planes.ToList());
        }

        private static bool TryParseJson(string json, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(json);
                return true;
            }
            catch
            {
                jObject = null;
                return false;
            }
        }
    }
}
