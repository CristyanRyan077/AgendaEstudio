using AgendaApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaApi.Models;

public partial class Cliente
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Telefone { get; set; }
    [MaxLength(100)]
    public string? Email { get; set; }
    [MaxLength(100)]
    public string? Observacao { get; set; }

    public StatusCliente Status { get; set; } = StatusCliente.SA;
    [MaxLength(100)]
    public string? Facebook { get; set; }
    [MaxLength(100)]
    public string? Instagram { get; set; }

    public decimal TotalPagoHistorico { get; set; }

    public decimal TotalPagoMesAtual { get; set; }
    //----------------------------------------------------//
    //            Coleções
    //----------------------------------------------------//
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public ICollection<Crianca> Criancas { get; set; } = new List<Crianca>();
}
