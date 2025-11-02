using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public class Messenger
    {
        public int? ClienteId { get; }
        public int? CriancaId { get; }

        public int? AgendamentoId { get; }

        public Messenger(int? clienteId = null, int? criancaId = null, int? agendamentoId = null)
        {
            ClienteId = clienteId;
            CriancaId = criancaId;
            AgendamentoId = agendamentoId;
        }
        public record AgendamentoReagendadoMessage(
            int agendamentoId,
            DateTime velhaData,
            TimeSpan velhoHorario,
            DateTime novaData,
            TimeSpan novoHorario);

    }
}
