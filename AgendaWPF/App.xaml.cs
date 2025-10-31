using AgendaApi.Models;
using AgendaWPF.Models;
using AgendaWPF.Services;
using AgendaWPF.ViewModels;
using AgendaWPF.Views;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Tools;
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
            var cultura = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultura;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultura;
            ConfigHelper.Instance.SetLang("pt-br");
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddTransient<IAgendamentoService, AgendamentoService>();
            services.AddTransient<IClienteService, ClienteService>();
            services.AddTransient<IServicoService, ServicoService>();
            services.AddTransient<ISemanaService, SemanaService>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            services.AddSingleton<AgendaViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<CalendarioViewModel>();
            services.AddSingleton<ClientesViewModel>();
            services.AddSingleton<FormAgendamentoVM>();

            services.AddTransient<MainWindow>();
            services.AddTransient<CardSemanal>();
            services.AddTransient<ClientesView>();
            services.AddTransient<CalendarioView>();
            services.AddTransient<AgendarView>();

            services.AddSingleton<AgendaState>();

            ServiceProvider = services.BuildServiceProvider();
            var mainwindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainwindow.Show();

        }

    }
}
