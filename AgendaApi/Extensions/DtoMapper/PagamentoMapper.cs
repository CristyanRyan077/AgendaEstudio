using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class PagamentoMapper
    {
        public static Pagamento ToEntity(this PagamentoCreateDto dto) =>
         new Pagamento
         {
             Tipo = dto.Tipo,
             Valor = dto.Valor,
             DataPagamento = dto.DataPagamento,
             Metodo = dto.Metodo,
             Observacao = dto.Observacao,
         };
        public static PagamentoDto ToDto(this Pagamento entity) =>
            new PagamentoDto
            {
                Id = entity.Id,
                AgendamentoId = entity.AgendamentoId,
                Tipo = entity.Tipo,
                Valor = entity.Valor,
                DataPagamento = entity.DataPagamento,
                Metodo =  entity.Metodo,
                Observacao = entity.Observacao,
                CreatedAt = entity.CreatedAt
            };
        public static void UpdateEntity(this Pagamento pg, PagamentoUpdateDto dto)
        {
            pg.AgendamentoId = dto.AgendamentoId;
            pg.Valor = dto.Valor;
            pg.Metodo = dto.Metodo;
            pg.DataPagamento = dto.DataPagamento;
            pg.Tipo = dto.Tipo;
            if (!string.IsNullOrEmpty(dto.observacao)) pg.Observacao = dto.observacao;

        }
    }
}
