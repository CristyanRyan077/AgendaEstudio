using AgendaShared.DTOs;
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
            if (dto.Nome != null)
                entity.Nome = dto.Nome;
            if (dto.Genero.HasValue)
                entity.Genero = dto.Genero.Value;
            if (dto.Idade.HasValue)
                entity.Idade = dto.Idade.Value;
            if (dto.IdadeUnidade.HasValue)
                entity.IdadeUnidade = dto.IdadeUnidade.Value;
        }
    }
}