using AgendaShared;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class EntregaViewModel : ObservableObject
    {
        private readonly IAgendamentoService _service;
        [ObservableProperty] private DateTime periodoInicio;
        [ObservableProperty] private DateTime periodoFim;
        [ObservableProperty] private ObservableCollection<AgendamentoVM> agendamentos = new();
        [ObservableProperty] private string filtroBusca;
        public ObservableCollection<TipoFotoOpcao> OpcoesFiltroFoto { get; } = new();
        public EntregaViewModel(IAgendamentoService service)
        {
            _service = service;
            PeriodoInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            PeriodoFim = PeriodoInicio.AddMonths(1).AddDays(-1);
            foreach (var tipo in Enum.GetValues<TipoEntrega>())
            {
                OpcoesFiltroFoto.Add(new TipoFotoOpcao(tipo));
            }
        }
        [RelayCommand]
        public async Task CarregarAsync()
        {

            var ags = await _service.ObterAgendamentosPorPeriodo(PeriodoInicio, PeriodoFim);
            Agendamentos = new ObservableCollection<AgendamentoVM>(ags.Select(a => new AgendamentoVM(a)));  
        }
        [RelayCommand]
        public async Task QuickMesAtual()
        {
            PeriodoInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            PeriodoFim = PeriodoInicio.AddMonths(1).AddDays(-1);
            await CarregarAsync();
        }
        [RelayCommand]
        public async Task AvancarMes()
        {
            PeriodoInicio = PeriodoInicio.AddMonths(1);
            PeriodoFim = PeriodoInicio.AddMonths(1).AddDays(-1);
            await CarregarAsync();
        }

        [RelayCommand]
        public async Task VoltarMes()
        {
            PeriodoInicio = PeriodoInicio.AddMonths(-1);
            PeriodoFim = PeriodoInicio.AddMonths(1).AddDays(-1);
            await CarregarAsync();
        }


    }
}
