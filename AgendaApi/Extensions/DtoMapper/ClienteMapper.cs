using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class ClienteMapper
    {
        public static Cliente ToEntity(this ClienteCreateDto dto) =>
      new Cliente
      {
          Nome = dto.Nome,
          Telefone = dto.Telefone,
          Email = dto.Email,
          Observacao = dto.Observacao,
          Facebook = dto.Facebook,
          Instagram = dto.Instagram
      };

        public static ClienteDto ToDto(this Cliente entity) =>
        new ClienteDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Telefone = entity.Telefone,
            Email = entity.Email,
            Observacao = entity.Observacao,
            Facebook = entity.Facebook,
            Instagram = entity.Instagram,
            TotalPagoHistorico = entity.TotalPagoHistorico,
            TotalPagoMesAtual = entity.TotalPagoMesAtual,
            Status = entity.Status
        };
        public static void UpdateEntity(this Cliente cliente, ClienteUpdateDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Telefone)) cliente.Telefone = dto.Telefone;
            if (!string.IsNullOrEmpty(dto.Email)) cliente.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Observacao)) cliente.Observacao = dto.Observacao;
            if (!string.IsNullOrEmpty(dto.Facebook)) cliente.Facebook = dto.Facebook;
            if (!string.IsNullOrEmpty(dto.Instagram)) cliente.Instagram = dto.Instagram;
        }
    }
}
