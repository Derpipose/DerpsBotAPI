using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace DerpsBotMaui.ViewModel {
    public partial class MainPageViewModel {
        private readonly HttpClient client;
        private string baseUrl;

        // Dictionary to store servers with nicknames
        public Dictionary<string, string> Servers { get; private set; } = new Dictionary<string, string>
        {
            { "Local", "http://localhost:5235" }
        };

        private string selectedServerNickname;
        public string SelectedServerNickname {
            get => selectedServerNickname;
            set {
                if (Servers.ContainsKey(value)) {
                    selectedServerNickname = value;
                    baseUrl = Servers[value];
                }
            }
        }

        public MainPageViewModel(HttpClient _client) {
            client = _client;
            SelectedServerNickname = "Local"; // Default to "Local"

            UpLeftCommand = new AsyncRelayCommand(UpLeft);
            UpRightCommand = new AsyncRelayCommand(UpRight);
            LeftCommand = new AsyncRelayCommand(Left);
            RightCommand = new AsyncRelayCommand(Right);
            DownLeftCommand = new AsyncRelayCommand(DownLeft);
            DownRightCommand = new AsyncRelayCommand(DownRight);
            StopCommand = new AsyncRelayCommand(Stop);
            CornerCommand = new AsyncRelayCommand(Corner);
        }

        public ICommand UpLeftCommand { get; }
        public ICommand UpRightCommand { get; }
        public ICommand LeftCommand { get; }
        public ICommand RightCommand { get; }
        public ICommand DownLeftCommand { get; }
        public ICommand DownRightCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand CornerCommand { get; }

        // Method to add a new server
        public void AddServer(string nickname, string url) {
            if (!Servers.ContainsKey(nickname)) {
                Servers[nickname] = url;
            }
        }

        // Method to update an existing server's nickname
        public void UpdateServerNickname(string oldNickname, string newNickname) {
            if (Servers.ContainsKey(oldNickname) && !Servers.ContainsKey(newNickname)) {
                string url = Servers[oldNickname];
                Servers.Remove(oldNickname);
                Servers[newNickname] = url;

                // Update selected server if necessary
                if (selectedServerNickname == oldNickname) {
                    SelectedServerNickname = newNickname;
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
    }
}