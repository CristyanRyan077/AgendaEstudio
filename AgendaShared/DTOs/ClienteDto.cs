
using AgendaShared.DTOs;
using AgendaShared;

namespace AgendaShared.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Observacao { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public decimal TotalPagoHistorico { get; set; }
        public decimal TotalPagoMesAtual { get; set; }

        public int TotalAgendamentos { get; set; }
        public int? CriancaId { get; set; }

        public StatusCliente Status = StatusCliente.SA;
        public string NomeComId => $"{Nome} (ID: {Id})";
        public List<CriancaDto>? Criancas { get; set; }
        public List<AgendamentoDto>? Agendamentos { get; set; }
       
    }
    
        public class ClienteCreateDto
        {
            public string Nome { get; set; } = string.Empty;
            public string Telefone { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Observacao { get; set; }
            public string? Facebook { get; set; }
            public string? Instagram { get; set; }

            public CriancaCreateDto? Crianca { get; set; }
        }
        public class ClienteUpdateDto
        {
            public string Nome { get; set; } = string.Empty;
            public string Telefone { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Observacao { get; set; }
            public string? Facebook { get; set; }
            public string? Instagram { get; set; }
            public CriancaUpdateDto? Crianca { get; set; }
        }
        public record ClienteQuery(
        int Page = 1,
        int PageSize = 20,
        ClienteSortPreset Sort = ClienteSortPreset.Proximos,
        string? Search = null,
        string? Status = null,
        int mesRef = 0,
        int anoRef = 0);



}
