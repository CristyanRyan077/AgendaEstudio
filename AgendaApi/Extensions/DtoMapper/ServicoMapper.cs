using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class ServicoMapper
    {
        public static Servico ToEntity(this ServicoCreateDto dto) =>
            new Servico
            {
                Nome = dto.Nome,
                PossuiCrianca = dto.PossuiCrianca
            };

        public static ServicoDto ToDto(this Servico entity) =>
            new ServicoDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                PossuiCrianca = entity.PossuiCrianca
            };

        public static void UpdateEntity(this Servico entity, ServicoUpdateDto dto)
        {
            entity.Nome = dto.Nome;
            entity.PossuiCrianca = dto.PossuiCrianca;
        }
    }
}