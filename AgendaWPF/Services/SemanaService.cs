using AgendaShared.DTOs;
using AgendaWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public interface ISemanaService
    {
        DateTime ObterSegunda(DateTime baseDate);
        Task<IReadOnlyList<DiaAgendamento>> CarregarSemanaAsync(DateTime baseDate);
        DiaAgendamento CriarDia(DateTime data); // helper opcional
    }

    public class SemanaService : ISemanaService
    {
        private readonly IAgendamentoService _agendamentoService;
        public SemanaService(IAgendamentoService agendamentoService) => _agendamentoService = agendamentoService;
        public DateTime ObterSegunda(DateTime d)
        {
            int delta = ((int)d.DayOfWeek + 6) % 7; // segunda=0
            return d.Date.AddDays(-delta);
        }

        public async Task<IReadOnlyList<DiaAgendamento>> CarregarSemanaAsync(DateTime baseDate)
        {
            var inicio = ObterSegunda(baseDate);
            var fim = inicio.AddDays(6);
            var ags = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            var dias = new List<DiaAgendamento>(7);
            for (int i = 0; i < 7; i++)
            {
                var dia = inicio.AddDays(i);
                var listaDia = ags.Where(a => a.Data.Date == dia.Date)
                                  .OrderBy(a => a.Horario)
                                  .ToList();

                dias.Add(new DiaAgendamento
                {
                    Data = dia,
                    Nome = CultureInfo.GetCultureInfo("pt-BR").TextInfo
                           .ToTitleCase(dia.ToString("dddd", new CultureInfo("pt-BR")).Replace("-feira", "").Trim()),
                    Agendamentos = new ObservableCollection<AgendamentoDto>(listaDia)
                });
            }
            return dias;
        }

        public DiaAgendamento CriarDia(DateTime data)
        {
            return new DiaAgendamento
            {
                Data = data.Date,
                Nome = CultureInfo.GetCultureInfo("pt-BR").TextInfo
                       .ToTitleCase(data.ToString("dddd", new CultureInfo("pt-BR")).Replace("-feira", "").Trim()),
                Agendamentos = new ObservableCollection<AgendamentoDto>()
            };
        }
    }
}
