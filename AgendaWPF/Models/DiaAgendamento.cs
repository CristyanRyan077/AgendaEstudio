using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public class DiaAgendamento
    {
        public string Nome { get; set; } // "Segunda", "Terça", ...
        public ObservableCollection<AgendamentoDto> Agendamentos { get; set; } = new();
    }
}
