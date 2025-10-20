using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaApi.Models;

public partial class Servico
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public bool PossuiCrianca { get; set; }
    //----------------------------------------------------//
    //             Coleções
    //----------------------------------------------------//
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public ICollection<Pacote> Pacotes { get; set; } = new List<Pacote>();
}
