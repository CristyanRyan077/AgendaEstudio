using System;
using System.Collections.Generic;

namespace AgendaApi.Models;

public partial class AgendamentoProduto
{
    public int Id { get; set; }
    //----------------------------------------------------//
    //             FK/Navegação
    //----------------------------------------------------//
    public int AgendamentoId { get; set; }
    public Agendamento Agendamento { get; set; } = null!;
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    //----------------------------------------------------//
    //             Colunas
    //----------------------------------------------------//
    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    //----------------------------------------------------//
}
