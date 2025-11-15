using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.DTOs
{
    public class PagamentoDto
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public TipoLancamento Tipo { get; set; } = TipoLancamento.Pagamento;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.Today;
        public MetodoPagamento Metodo { get; set; } = MetodoPagamento.Pix;
        public string? Observacao { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PagamentoCreateDto
    {
        public TipoLancamento Tipo { get; set; } = TipoLancamento.Pagamento;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.Today;
        public MetodoPagamento Metodo { get; set; } = MetodoPagamento.Pix;
        public string? Observacao { get; set; }
    }

    public class PagamentoUpdateDto
    {
        public int AgendamentoId { get; set; }
        public TipoLancamento Tipo { get; set; } = TipoLancamento.Pagamento;
        public decimal Valor;
        public DateTime DataPagamento;
        public MetodoPagamento Metodo;
        public string? observacao;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public record ResumoAgendamentoDto(int ClienteId, string ClienteNome, string ServicoNome, DateTime Data, decimal Valor);
    public record HistoricoFinanceiroDto(int Id, DateTime Data, string Tipo, string Descricao, decimal Valor, MetodoPagamento? Metodo);
   
}
