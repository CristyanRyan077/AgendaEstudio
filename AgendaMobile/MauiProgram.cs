using AgendaMobile.Helpers;
using AgendaMobile.Pages;
using AgendaMobile.Services;
using AgendaMobile.ViewModels;
using CommunityToolkit.Maui;
using Plugin.Maui.Calendar;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace AgendaMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();
            builder.ConfigureSyncfusionToolkit();

            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<CalendarioViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<LoadingPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            ServiceHelper.Services = app.Services;

            return app;
        }
    }
}