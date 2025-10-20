using AgendaApi.Models;
using System.ComponentModel.DataAnnotations;

namespace AgendaApi.Domain.Models
{
    public class AgendamentoEtapa
    {
        public int Id { get; set; }

        //----------------------------------------------------//
        //             FK/Navegação
        //----------------------------------------------------//
        public int AgendamentoId { get; set; }
        public Agendamento Agendamento { get; set; } = null!;

        //----------------------------------------------------//
        //             Colunas
        //----------------------------------------------------//
        public EtapaFotos Etapa { get; set; }

        public DateTime dataConclusao;

        [MaxLength(255)] public string? Observacao;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
    }
}
