
using AgendaNovo.ViewModels;
using AgendaNovo.Views;
using HandyControl.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace AgendaNovo
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



            //Views
            services.AddScoped<WindowManager>();
            services.AddTransient<Agendar>();
            services.AddTransient<MainWindow>();
            services.AddTransient<GerenciarClientes>();
            services.AddTransient<Login>();
            services.AddTransient<Calendario>();
            services.AddTransient<Financeiro>();
            services.AddTransient<ProcessoFotos>();

            //ViewModels
            services.AddTransient<CalendarioViewModel>();
            services.AddTransient<ClienteCriancaViewModel>();
            services.AddSingleton<AgendaViewModel>();
            services.AddTransient<FinanceiroViewModel>();
            services.AddTransient<PagamentosViewModel>();
            services.AddTransient<FotosViewModel>();

            ServiceProvider = services.BuildServiceProvider();
            var login = ServiceProvider.GetRequiredService<Login>();
            login.Show();

        }

    }
}
