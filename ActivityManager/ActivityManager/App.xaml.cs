using Xamarin.Forms;

namespace ActivityManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            //var activityManagerView = new Views.ActivityManagerView();
            //var activtiyManagerViewModel = new ViewModels.ActivityManagerViewModel();
            
            //activityManagerView.BindingContext = activtiyManagerViewModel;

          
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
