using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AgendaMobile.Helpers
{
    public class AgendamentoPorDia
    {
        public DateTime Dia { get; set; }
        public DateTime MesReferencia { get; set; }
        public ObservableCollection<AgendamentoDto> Agendamentos { get; set; } = new();
        public string DiaTexto => Dia.Day.ToString();
        public bool EhDoMesAtual => Dia.Month == MesReferencia.Month && Dia.Year == MesReferencia.Year;
        public ICommand SelecionarDiaCommand { get; set; }

        public Color CorTexto => EhDoMesAtual ? Colors.Black : Colors.Gray;

    }
}
