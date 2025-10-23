
using AgendaShared.DTOs;
using AgendaWPF.Models;
using AgendaWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public class AgendaViewModel 
    {
        public ObservableCollection<DiaAgendamento> DiasSemana { get; set; } = new();
        private readonly IAgendamentoService _agendamentoService;
        public AgendaViewModel(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }
        public async Task InicializarAsync()
        {
            await CarregarSemanaAtualAsync();
        }
        public async Task CarregarSemanaAtualAsync()
        {
            var inicio = ObterSegundaDaSemana(DateTime.Today);
            var fim = inicio.AddDays(6);

            var agendamentos = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            // Monta lista agrupada por dia:
            DiasSemana.Clear();

            for (int i = 0; i < 7; i++)
            {
                var dia = inicio.AddDays(i);
                var listaDia = agendamentos
                    .Where(a => a.Data.Date == dia.Date)
                    .OrderBy(a => a.Horario)
                    .ToList();

                DiasSemana.Add(new DiaAgendamento
                {
                    Nome = CultureInfo.GetCultureInfo("pt-BR")
                    .TextInfo
                    .ToTitleCase(
                        dia.ToString("dddd", new CultureInfo("pt-BR"))
                           .Replace("-feira", "")
                           .Trim()
                    ),
                    Agendamentos = new ObservableCollection<AgendamentoDto>(listaDia)
                });
            }
        }
        private static DateTime ObterSegundaDaSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Monday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }
    }
}
