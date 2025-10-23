using AgendaMobile.Helpers;
using AgendaMobile.Pages;
using AgendaMobile.Services;

namespace AgendaMobile
{

    public partial class App : Application
    {
        private readonly IApiService _apiService;
        public App(IApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            MainPage = new NavigationPage(ServiceHelper.GetService<LoadingPage>());
        }


    }
}