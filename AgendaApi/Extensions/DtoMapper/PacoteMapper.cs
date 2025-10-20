using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class PacoteMapper
    {
        public static Pacote ToEntity(this PacoteCreateDto dto) =>
            new Pacote
            {
                ServicoId = dto.ServicoId,
                Nome = dto.Nome,
                Valor = dto.Valor
            };

        public static PacoteDto ToDto(this Pacote entity) =>
            new PacoteDto
            {
                Id = entity.Id,
                ServicoId = entity.ServicoId,
                Nome = entity.Nome,
                Valor = entity.Valor
            };

        public static void UpdateEntity(this Pacote entity, PacoteUpdateDto dto)
        {
            entity.ServicoId = dto.ServicoId;
            entity.Nome = dto.Nome;
            entity.Valor = dto.Valor;
        }
    }
}