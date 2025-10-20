using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AgendaApi.Domain.Models;

namespace AgendaApi.Models;

public partial class Pagamento
{
    public int Id { get; set; }

    //----------------------------------------------------//
    //             FK/Navegação
    //----------------------------------------------------//
    public int AgendamentoId { get; set; }
    public int? AgendamentoProdutoId { get; set; }
    public AgendamentoProduto AgendamentoProduto { get; set; } = null!;
    public Agendamento Agendamento { get; set; } = null!;


    //----------------------------------------------------//
    //             Enums
    //----------------------------------------------------//
    public TipoLancamento Tipo { get; set; } = TipoLancamento.Pagamento;
    public MetodoPagamento Metodo { get; set; }
    //----------------------------------------------------//
    //             Colunas
    //----------------------------------------------------//
    [MaxLength(100)] public string? Observacao { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }


}
