using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class CriancaMapper
    {
        public static Crianca ToEntity(this CriancaCreateDto dto) =>
            new Crianca
            {
                Nome = dto.Nome,
                Genero = dto.Genero,
                Idade = dto.Idade,
                IdadeUnidade = dto.IdadeUnidade,
                ClienteId = dto.ClienteId
            };

        public static CriancaDto ToDto(this Crianca entity) =>
            new CriancaDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Genero = entity.Genero,
                Idade = entity.Idade,
                IdadeUnidade = entity.IdadeUnidade,
                ClienteId = entity.ClienteId
            };

        public static void UpdateEntity(this Crianca entity, CriancaUpdateDto dto)
        {
            entity.Nome = dto.Nome;
            entity.Genero = dto.Genero;
            entity.Idade = dto.Idade;
            entity.IdadeUnidade = dto.IdadeUnidade;
        }
    }
}