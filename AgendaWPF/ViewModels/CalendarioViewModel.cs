using AgendaShared.DTOs;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class CalendarioViewModel : ObservableObject
    {
        private readonly IAgendamentoService _agendamentoService;
        public CalendarioViewModel(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
            _ = InicializarAsync();
        }

        public async Task InicializarAsync()
        {
            await Task.Delay(500);
            GerarDiasDoMesAtual();
            RefreshCalendar();
        }

        [ObservableProperty]
        private ObservableCollection<DiaCalendario> diasDoMes = new();
        [ObservableProperty]
        private ObservableCollection<AgendamentoDto> agendamentos = new();

        public async void RefreshCalendar()
        {
            var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);

            // Obtém todos os agendamentos do mês
            var agendamentos = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);


            foreach (var dia in DiasDoMes)
            {
                dia.Agendamentos.Clear();

                // 🔍 Filtra apenas os agendamentos do periodo selecionado
                 var agsDoDia = agendamentos
                    .Where(a => a.Data.Date == dia.Data.Date); 

                foreach (var ag in agsDoDia)
                    dia.Agendamentos.Add(ag);
            } 
        }
        private void GerarDiasDoMesAtual()
        {
            DiasDoMes.Clear();

            var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var totalDias = DateTime.DaysInMonth(inicio.Year, inicio.Month);

            for (int i = 0; i < totalDias; i++)
            {
                DiasDoMes.Add(new DiaCalendario
                {
                    Data = inicio.AddDays(i),
                    Agendamentos = new ObservableCollection<AgendamentoDto>()
                });
            }
        }
    }
}
