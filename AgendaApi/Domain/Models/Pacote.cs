using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaApi.Models;

public partial class Pacote
{
    public int Id { get; set; }

    public int ServicoId { get; set; }
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public Servico Servico { get; set; } = null!;
}
