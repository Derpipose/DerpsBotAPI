using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DerpsBotMaui.ViewModel {
    public class ServerItem : INotifyPropertyChanged {
        public string Nickname { get; set; }
        public string Url { get; set; }

        private bool isSelected;
        public bool IsSelected {
            get => isSelected;
            set {
                if (isSelected != value) {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainPageViewModel : INotifyPropertyChanged {
        private readonly HttpClient client;
        private string baseUrl;

        public ObservableCollection<ServerItem> ServersList { get; private set; } = new ObservableCollection<ServerItem>
        {
            new ServerItem { Nickname = "Local", Url = "http://localhost:5235", IsSelected = true }
        };

        private string selectedServerNickname;
        public string SelectedServerNickname {
            get => selectedServerNickname;
            set {
                if (ServersList.Any(s => s.Nickname == value)) {
                    selectedServerNickname = value;
                    var selectedServer = ServersList.First(s => s.Nickname == value);
                    baseUrl = selectedServer.Url;

                    foreach (var server in ServersList) {
                        server.IsSelected = server.Nickname == value;
                    }

                    OnPropertyChanged();
                }
            }
        }

        public ICommand UpLeftCommand { get; }
        public ICommand UpRightCommand { get; }
        public ICommand LeftCommand { get; }
        public ICommand RightCommand { get; }
        public ICommand DownLeftCommand { get; }
        public ICommand DownRightCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand CornerCommand { get; }
        public ICommand SelectServerCommand { get; }
        public ICommand AddServerCommand { get; }

        public string NewServerNickname { get; set; }
        public string NewServerUrl { get; set; }

        public MainPageViewModel(HttpClient _client) {
            client = _client;
            SelectedServerNickname = "Local"; // Default selection

            UpLeftCommand = new AsyncRelayCommand(UpLeft);
            UpRightCommand = new AsyncRelayCommand(UpRight);
            LeftCommand = new AsyncRelayCommand(Left);
            RightCommand = new AsyncRelayCommand(Right);
            DownLeftCommand = new AsyncRelayCommand(DownLeft);
            DownRightCommand = new AsyncRelayCommand(DownRight);
            StopCommand = new AsyncRelayCommand(Stop);
            CornerCommand = new AsyncRelayCommand(Corner);

            SelectServerCommand = new RelayCommand<ServerItem>(SelectServer);
            AddServerCommand = new RelayCommand(AddServer);
        }

        private void SelectServer(ServerItem server) {
            if (server != null) {
                SelectedServerNickname = server.Nickname;
            }
        }

        private void AddServer() {
            if (!string.IsNullOrEmpty(NewServerNickname) && !string.IsNullOrEmpty(NewServerUrl) &&
                !ServersList.Any(s => s.Nickname == NewServerNickname)) {
                ServersList.Add(new ServerItem { Nickname = NewServerNickname, Url = NewServerUrl, IsSelected = false });
                NewServerNickname = string.Empty;
                NewServerUrl = string.Empty;
                OnPropertyChanged(nameof(NewServerNickname));
                OnPropertyChanged(nameof(NewServerUrl));
            }
        }

        private void OnServerSelected(object sender, CheckedChangedEventArgs e) {
            if (sender is RadioButton radioButton && e.Value is bool isChecked && isChecked) {
                if (radioButton.BindingContext is ServerItem selectedServer) {
                    this.SelectedServerNickname = selectedServer.Nickname;
                }
            }
        }




        private async Task UpLeft() => await SendCommand("NW");
        private async Task UpRight() => await SendCommand("NE");
        private async Task Right() => await SendCommand("E");
        private async Task Left() => await SendCommand("W");
        private async Task DownLeft() => await SendCommand("SW");
        private async Task DownRight() => await SendCommand("SE");
        private async Task Stop() => await SendCommand("HOLD");
        private async Task Corner() => await SendCommand("CORNER");

        private async Task SendCommand(string command) {
            if (!string.IsNullOrEmpty(baseUrl)) {
                await client.GetAsync($"{baseUrl}/change/{command}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
