using AgendaShared;
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public partial class AgendamentoVM : ObservableObject
    {
        private AgendamentoDto _dto;
        public int Id => _dto.Id;
        public int ClienteId => _dto.ClienteId;
        public int CriancaId => _dto.CriancaId ?? 0;
        public int ServicoId => _dto.ServicoId; 
        public int PacoteId => _dto.PacoteId;
        public string Tema => _dto.Tema ?? string.Empty;
        public int? Mesversario => _dto.Mesversario;
        public string? Observacao => _dto.Observacao;

        [ObservableProperty] private bool isCurrentFindHit;
 

        public ClienteResumoDto Cliente => _dto.Cliente ?? ClienteResumoDto.Empty;
        public ServicoResumoDto Servico => _dto.Servico ?? ServicoResumoDto.Empty;
        public PacoteResumoDto Pacote => _dto.Pacote ?? PacoteResumoDto.Empty;
        public CriancaResumoDto? Crianca => _dto.Crianca;

        [ObservableProperty] private DateTime data;

        [ObservableProperty] private TimeSpan? horario;

        [ObservableProperty] private decimal valor;
        [ObservableProperty] private StatusAgendamento status;
        [ObservableProperty] private TipoEntrega tipo;

        [ObservableProperty] private ObservableCollection<PagamentoDto> pagamentos = new();
        [ObservableProperty] private ObservableCollection<ClienteDto> clientes = new();
        [ObservableProperty] private ObservableCollection<AgendamentoDto> historicoAgendamentos = new();

        [NotMapped] public decimal ValorPago => Pagamentos?.Sum(p => p.Valor) ?? 0m;
        public bool EstaPago => Math.Round(Valor, 2) <= Math.Round(ValorPago, 2);

        [NotMapped]
        public string? MesversarioFormatado
        {
            get
            {
                if (Crianca == null)
                    return "";

                return Crianca.IdadeUnidade switch
                {
                    IdadeUnidade.Ano or IdadeUnidade.Anos => $"{Mesversario} anos",
                    IdadeUnidade.Mês or IdadeUnidade.Meses => $"{Mesversario} meses",
                    _ => $"Mês {Mesversario}"
                };
            }
        }
        [NotMapped] public bool TemReserva => Pagamentos?.Any(p => p.Tipo == TipoLancamento.Reserva) == true;
        [NotMapped] public string? MesReserva => Pagamentos?.FirstOrDefault(p => p.Tipo == TipoLancamento.Reserva)?.Observacao;
        [NotMapped] public decimal? ValorReserva => Pagamentos.FirstOrDefault(p => p.Tipo == TipoLancamento.Reserva)?.Valor;

        public AgendamentoVM(AgendamentoDto dto)
        {
            _dto = dto;
            Data = dto.Data;
            Horario = dto.Horario;
            Valor = dto.Valor;
            Status = dto.Status;
            Tipo = dto.Tipo;
            Pagamentos = new ObservableCollection<PagamentoDto>(dto.Pagamentos ?? new List<PagamentoDto>());
            HistoricoAgendamentos = new ObservableCollection<AgendamentoDto>();
        }

    }
}
