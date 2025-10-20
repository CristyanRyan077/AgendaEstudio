using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaApi.Models;

public partial class Produto
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Nome { get; set; } = null!;

    public decimal Valor { get; set; }

    public ICollection<AgendamentoProduto> AgendamentoProdutos { get; set; } = new List<AgendamentoProduto>();
}
