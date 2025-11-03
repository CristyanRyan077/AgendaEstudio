using AgendaApi.Models;
using AgendaShared.DTOs;
using AgendaShared.Enums;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class FotosMapper
    {
        public static AgendamentoEtapa ToEntity(this EtapaFotosDto dto) =>
        new AgendamentoEtapa
        {
            Etapa = dto.Etapa,
            dataConclusao = dto.DataConclusao,
            Observacao = dto.Observacao,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
        public static EtapaFotosDto ToDto(this AgendamentoEtapa entity) =>
            new EtapaFotosDto
            {
                Etapa = entity.Etapa,
                DataConclusao = entity.dataConclusao,
                Observacao =  entity.Observacao,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
    }
}
