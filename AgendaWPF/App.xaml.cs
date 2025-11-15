//using Microsoft.Extensions.Http;
using AgendaWPF.Controles;
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
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                MessageBox.Show(e.ExceptionObject.ToString(), "Erro não tratado");
            };

            DispatcherUnhandledException += (_, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Erro");
                e.Handled = true; // impede fechar
            };
        }

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
            services.AddTransient<IPagamentoService, PagamentoService>();
            services.AddTransient<IAcoesService, AcoesService>();
            services.AddTransient<IFinanceiroService, FinanceiroService>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            services.AddSingleton<AgendaViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<CalendarioViewModel>();
            services.AddSingleton<ClientesViewModel>();
            services.AddSingleton<FormAgendamentoVM>();
            services.AddSingleton<PagamentosViewModel>();
            services.AddSingleton<FinanceiroViewModel>();


            services.AddTransient<MainWindow>();
            services.AddTransient<CardSemanal>();
            services.AddTransient<ClientesView>();
            services.AddTransient<CalendarioView>();
            services.AddTransient<AgendarView>();
            services.AddTransient<FinanceiroView>();
            services.AddTransient<AdicionarPagamento>();
            services.AddTransient<EditarAgendamento>();

            services.AddSingleton<AgendaState>();
#if DEBUG
            const string apiBaseUrl = "http://localhost:5005/";
#else
    const string apiBaseUrl = "http://192.168.30.121:5010/"; // prod
#endif
            services.AddHttpClient<IAgendamentoService, AgendamentoService>(client => { client.BaseAddress = new Uri(apiBaseUrl);});
            services.AddHttpClient<IClienteService, ClienteService>(client => { client.BaseAddress = new Uri(apiBaseUrl);});
            services.AddHttpClient<IPagamentoService, PagamentoService>(client => { client.BaseAddress = new Uri(apiBaseUrl); });
            services.AddHttpClient<IServicoService, ServicoService>(client => { client.BaseAddress = new Uri(apiBaseUrl); });
            services.AddHttpClient<IFinanceiroService, FinanceiroService>(client => { client.BaseAddress = new Uri(apiBaseUrl); });

            ServiceProvider = services.BuildServiceProvider();
            var mainwindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainwindow.Show();

        }

    }
}
