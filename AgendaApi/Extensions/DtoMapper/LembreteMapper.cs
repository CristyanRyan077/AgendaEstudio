using AgendaApi.Domain.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class LembreteMapper
    {
        public static Lembrete ToEntity(this LembreteCreateDto dto) =>
         new Lembrete
         {
             Titulo = dto.Titulo,
             Descricao = dto.Descricao,
             DataAlvo = dto.DataAlvo,
             ClienteId = dto.ClienteId,
             AgendamentoId = dto.AgendamentoId,
             Status = dto.Status,
             CaminhoImagem = dto.CaminhoImagem
         };
        public static LembreteDto ToDto(this Lembrete entity) =>
            new LembreteDto
        {
            Id = entity.Id,
            Titulo = entity.Titulo,
            Descricao = entity.Descricao ?? string.Empty,
            DataAlvo = entity.DataAlvo,
            ClienteId = entity.ClienteId,
            AgendamentoId = entity.AgendamentoId,
            CaminhoImagem = entity.CaminhoImagem,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            ConcluidoEm = entity.ConcluidoEm,
            ClienteNome = entity.Cliente != null ? entity.Cliente.Nome : null
            };
    }
}
