using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class AgendamentoMapper
    {
        public static Agendamento ToEntity(this AgendamentoCreateDto dto) =>
     new Agendamento
     {
         ClienteId = dto.ClienteId,
         CriancaId = dto.CriancaId,
         ServicoId = dto.ServicoId,
         PacoteId = dto.PacoteId,
         Valor = dto.Valor,
         Data = dto.Data,
         Horario = dto.Horario,
         Tema = dto.Tema,
         Status = dto.Status
     };

        public static AgendamentoDto ToDto(this Agendamento entity) =>
        new AgendamentoDto
        {
            Id = entity.Id,
            ClienteId = entity.ClienteId,
            CriancaId = entity.CriancaId,
            ServicoId = entity.ServicoId,
            PacoteId = entity.PacoteId,
            Valor = entity.Valor,
            Data = entity.Data,
            Horario = entity.Horario,
            Tema = entity.Tema,
            Status = entity.Status,
            ValorPago = entity.ValorPago
        };
        public static void UpdateEntity(this Agendamento ag, AgendamentoUpdateDto dto)
        {
            ag.ClienteId = dto.ClienteId;
            ag.CriancaId = dto.CriancaId;
            ag.ServicoId = dto.ServicoId;
            ag.PacoteId = dto.PacoteId;
            ag.Data = dto.Data;
            ag.Valor = dto.Valor;
            ag.Status = dto.Status;

            if (dto.Horario.HasValue) ag.Horario = dto.Horario.Value;
            if (!string.IsNullOrEmpty(dto.Tema)) ag.Tema = dto.Tema;

        }
        
    }
}
