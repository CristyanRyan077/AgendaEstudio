
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

        public ClienteResumoDto? Cliente { get; set; }
        public ServicoResumoDto? Servico { get; set; }
        public PacoteResumoDto? Pacote { get; set; }
        public CriancaResumoDto? Crianca { get; set; }
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
    public class ClienteResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;    
    }
    public class ServicoResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
    public class PacoteResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
    public class CriancaResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public IdadeUnidade IdadeUnidade { get; set; }
        public Genero Genero { get; set; }
    }
}
