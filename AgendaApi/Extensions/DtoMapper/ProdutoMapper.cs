using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class ProdutoMapper
    {
        public static AgendamentoProduto ToEntity(this AgendamentoProdutoCreateDto dto) =>
        new AgendamentoProduto
        {
            ProdutoId = dto.ProdutoId,
            ValorUnitario = dto.ValorUnitario,
            Quantidade = dto.Quantidade,
            
        };

        public static AgendamentoProdutoDto ToDto(this AgendamentoProduto entity)
        {
            return new AgendamentoProdutoDto
            {
                Id = entity.Id,
                AgendamentoId = entity.AgendamentoId,
                ProdutoId = entity.ProdutoId,
                // O serviço deve garantir que 'entity.Produto' foi carregado (via .Include)
                ProdutoNome = entity.Produto?.Nome ?? "N/A",
                Quantidade = entity.Quantidade,
                ValorUnitario = entity.ValorUnitario,
                ValorTotal = entity.ValorTotal, // Usa a propriedade da entidade
                CreatedAt = entity.CreatedAt
            };
        }
        public static ProdutoDto ToDto(this Produto entity)
        {
            return new ProdutoDto
            {
                Id = entity.Id,
                Valor = entity.Valor,
                Nome = entity.Nome
                
            };
        }

    }
}
