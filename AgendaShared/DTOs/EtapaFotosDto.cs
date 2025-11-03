using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.DTOs
{
    public class EtapaFotosDto
    {
        public ResumoAgendamentoDto Agendamento { get; set; } = null!;

        public EtapaFotos Etapa { get; set; }

        public DateTime DataConclusao { get; set; }
        public string? Observacao { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
