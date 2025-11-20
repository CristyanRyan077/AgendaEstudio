using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.DTOs
{
    public class LembreteDto
    {
        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public int? AgendamentoId { get; set; }
        public DateTime DataAlvo { get; set; }      // quando você quer ser lembrado
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? CaminhoImagem { get; set; }
        public LembreteStatus Status { get; set; }  // Pendente, Concluido, Ignorado
        public DateTime CreatedAt { get; set; }
        public DateTime? ConcluidoEm { get; set; }
        public string? ClienteNome { get; set; }

    }
    public class LembreteCreateDto
    {
        public int? ClienteId { get; set; }
        public int? AgendamentoId { get; set; }
        public DateTime DataAlvo { get; set; }      // quando você quer ser lembrado
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? CaminhoImagem { get; set; }
        public LembreteStatus Status { get; set; }
    }
    public class LembreteQuery
    {
        public DateTime? Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public int? ClienteId { get; set; }
        public int? AgendamentoId { get; set; }
        public LembreteStatus? Status { get; set; }
    }


}
