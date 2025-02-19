using DerpsBotMaui.ViewModel;

namespace DerpsBotMaui {
    public partial class MainPage : ContentPage {
        

        public MainPage(MainPageViewModel mpvm) {

            InitializeComponent();
            BindingContext = mpvm;
        }

        private void OnServerSelected(object sender, CheckedChangedEventArgs e) {
            if (sender is RadioButton radioButton && e.Value is bool isChecked && isChecked) {
                if (radioButton.BindingContext is ServerItem selectedServer) {
                    var viewModel = (MainPageViewModel)BindingContext;
                    viewModel.SelectedServerNickname = selectedServer.Nickname;
                }
            }
        }

    }

}
