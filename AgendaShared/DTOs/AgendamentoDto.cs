
using AgendaShared;
using AgendaShared.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? Mesversario { get; set; }
        public StatusAgendamento Status { get; set; }
        public TipoEntrega Tipo { get; set; }
        public List<PagamentoDto>? Pagamentos { get; set; }
        public ClienteResumoDto? Cliente { get; set; }
        public ServicoResumoDto? Servico { get; set; }
        public PacoteResumoDto? Pacote { get; set; }
        public CriancaResumoDto? Crianca { get; set; }

        public List<EtapaFotosDto>? Etapas { get; set; }
        //----------------------------------------------------//
        //           NotMapped
        //----------------------------------------------------//
        [NotMapped] public decimal ValorPago => Pagamentos?.Sum(p => p.Valor) ?? 0m;
        [NotMapped] public int? NumeroMes { get; set; }
        [NotMapped]
        public string? MesversarioFormatado
        {
            get
            {
                if (Crianca == null)
                    return "";

                return Crianca.IdadeUnidade switch
                {
                    IdadeUnidade.Ano or IdadeUnidade.Anos => $"{Crianca.Idade} anos",
                    IdadeUnidade.Mês or IdadeUnidade.Meses => $"{Crianca.Idade} meses",
                    _ => $"Mês {Mesversario}"
                };
            }
        }
        public bool EstaPago => Math.Round(Valor, 2) <= Math.Round(ValorPago, 2);
        [NotMapped] public bool TemReserva => Pagamentos?.Any(p => p.Tipo == TipoLancamento.Reserva) == true;
        [NotMapped] public string? MesReserva => Pagamentos?.FirstOrDefault(p => p.Tipo == TipoLancamento.Reserva)?.Observacao;
        [NotMapped] public decimal? ValorReserva => Pagamentos.FirstOrDefault(p => p.Tipo == TipoLancamento.Reserva)?.Valor;

        
        //----------------------------------------------------//
    }

    public class AgendamentoCreateDto
    {
        public int ClienteId { get; set; }
        public int? CriancaId { get; set; }
        public int ServicoId { get; set; }
        public int PacoteId { get; set; }
        public int? Mesversario { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Horario { get; set; }
        public StatusAgendamento Status { get; set; }
        public TipoEntrega Tipo { get; set; }
        public string? Tema { get; set; }
        public decimal Valor { get; set; }

        public PagamentoCreateDto? PagamentoInicial { get; set; }
    }

    public class ReagendarDto
    {
        [Required]
        public DateTime NovaData { get; set; }

        [Required]
        public TimeSpan NovoHorario { get; set; }
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
        public string Email { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
    }
    public class ServicoResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int PrazoTratarDias { get; set; } = 3;
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
