using AgendaApi.Models;
using AgendaWPF.Services;
using AgendaWPF.ViewModels;
using AgendaWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AgendaWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddTransient<IAgendamentoService, AgendamentoService>();
            services.AddTransient<IClienteService, ClienteService>();

            services.AddSingleton<AgendaViewModel>();
            services.AddSingleton<MainViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<CardSemanal>();
            services.AddTransient<ClientesView>();

            ServiceProvider = services.BuildServiceProvider();
            var mainwindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainwindow.Show();

        }

    }
}
