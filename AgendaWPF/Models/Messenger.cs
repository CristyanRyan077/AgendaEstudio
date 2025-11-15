using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
        public record PagamentoCriadoMessage(int AgendamentoId, PagamentoDto Pagamento);
        public record ProdutoAdicionadoMessage(int AgendamentoId, AgendamentoProdutoDto ProdutoAdicionado);
    public record AgendamentoReagendadoMessage(
            int agendamentoId,
            DateTime velhaData,
            TimeSpan? velhoHorario,
            DateTime novaData,
            TimeSpan? novoHorario);
        public record FocusAgendamentoMessage(int AgendamentoId, DateTime Data);

}
