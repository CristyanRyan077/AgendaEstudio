using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AgendaApi.Domain.Models;

namespace AgendaApi.Models;

public partial class Crianca
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    public Genero Genero { get; set; }

    public IdadeUnidade IdadeUnidade { get; set; }

    public Cliente Cliente { get; set; } = null!;
}
