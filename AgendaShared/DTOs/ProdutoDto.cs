using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
    // DTO de requisição (o que o client envia)
    public class AgendamentoProdutoCreateDto
    {
        // Informações do Produto
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; } = 1;
        public decimal ValorUnitario { get; set; }

        // Informações do Pagamento (agora agrupadas e opcionais)
        public PagamentoParaProdutoCreateDto? Pagamento { get; set; }
    }

    // Sub-DTO para o pagamento
    public class PagamentoParaProdutoCreateDto
    {
        public MetodoPagamento? Metodo { get; set; }
        public string? Observacao { get; set; }
        public DateTime? DataPagamento { get; set; }
    }

    // DTO de Resposta (o que a API devolve)
    public class AgendamentoProdutoDto
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty; // Útil para o client
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
