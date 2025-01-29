using DerpsBotMaui.ViewModel;

namespace DerpsBotMaui {
    public partial class MainPage : ContentPage {
        

        public MainPage(MainPageViewModel mpvm) {

            InitializeComponent();
            BindingContext = mpvm;
        }

    }

}
