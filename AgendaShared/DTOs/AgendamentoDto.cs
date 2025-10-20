
using AgendaShared;

namespace AgendaShared.DTOs
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int? CriancaId { get; set; }
        public int ServicoId { get; set; }
        public int PacoteId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Horario { get; set; }
        public string? Tema { get; set; }
        public decimal Valor { get; set; }
        public StatusAgendamento Status { get; set; }
        public decimal ValorPago { get; set; }
        public bool EstaPago { get; set; }
    }

    public class AgendamentoCreateDto
    {
        public int ClienteId { get; set; }
        public int? CriancaId { get; set; }
        public int ServicoId { get; set; }
        public int PacoteId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Horario { get; set; }
        public StatusAgendamento Status { get; set; }
        public string? Tema { get; set; }
        public decimal Valor { get; set; }
    }

    public class AgendamentoUpdateDto
    {
        public int ClienteId { get; set; }
        public int? CriancaId { get; set; }
        public int ServicoId { get; set; }
        public int PacoteId { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan? Horario { get; set; }
        public string? Tema { get; set; }
        public decimal Valor { get; set; }
        public StatusAgendamento Status { get; set; }
    }
}
