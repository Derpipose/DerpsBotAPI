using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DerpsBotMaui.ViewModel {
    public partial class MainPageViewModel {
        HttpClient client;
        public MainPageViewModel(HttpClient _client) {
            client = _client; UpLeftCommand = new AsyncRelayCommand(UpLeft);
            UpRightCommand = new AsyncRelayCommand(UpRight);
            LeftCommand = new AsyncRelayCommand(Left);
            RightCommand = new AsyncRelayCommand(Right);
            DownLeftCommand = new AsyncRelayCommand(DownLeft);
            DownRightCommand = new AsyncRelayCommand(DownRight);
            StopCommand = new AsyncRelayCommand(Stop);
        }

        public ICommand UpLeftCommand { get; }
        public ICommand UpRightCommand { get; }
        public ICommand LeftCommand { get; }
        public ICommand RightCommand { get; }
        public ICommand DownLeftCommand { get; }
        public ICommand DownRightCommand { get; }
        public ICommand StopCommand { get; }

        private async Task UpLeft() => await client.GetAsync("http://localhost:5235/change/NW");
        private async Task UpRight() => await client.GetAsync("http://localhost:5235/change/NE");
        private async Task Right() => await client.GetAsync("http://localhost:5235/change/E");
        private async Task Left() => await client.GetAsync("http://localhost:5235/change/W");
        private async Task DownLeft() => await client.GetAsync("http://localhost:5235/change/SW");
        private async Task DownRight() => await client.GetAsync("http://localhost:5235/change/SE");
        private async Task Stop() => await client.GetAsync("http://localhost:5235/change/HOLD");

    }
}
