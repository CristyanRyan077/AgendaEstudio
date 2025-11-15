using AgendaShared;

namespace AgendaApi.Domain.Models
{
    public class Lembrete
    {
        public int Id { get; set; }

        public int? ClienteId { get; set; }
        public int? AgendamentoId { get; set; }

        public DateTime DataAlvo { get; set; }      // quando você quer ser lembrado 
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public string? LembreteTipo { get; set; }      // Financeiro, FollowUp, Outro...
        public LembreteStatus Status { get; set; }  // Pendente, Concluido, Ignorado

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConcluidoEm { get; set; }
    }
}
