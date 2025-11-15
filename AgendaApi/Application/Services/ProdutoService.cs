using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra;
using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Application.Services
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByIdAsync(int id);
        Task AddAsync(Produto servico);
        Task UpdateAsync(Produto servico);
        Task DeleteAsync(int id);
        Task<AgendamentoProdutoDto> AdicionarProdutoAsync(int agendamentoId, AgendamentoProdutoCreateDto dto);
        Task<AgendamentoProduto?> GetAgProdutoByIdAsync(int id);
    }
    public class ProdutoService : IProdutoService
    {
        private readonly AgendaContext _context;
        public ProdutoService(AgendaContext context) => _context = context;
        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Produtos.FindAsync(id);
            if (entity != null)
            {
                _context.Produtos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos
                .ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<AgendamentoProduto?> GetAgProdutoByIdAsync(int id)
        {
            return await _context.AgendamentoProdutos
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
        public async Task<AgendamentoProdutoDto> AdicionarProdutoAsync(int agendamentoId, AgendamentoProdutoCreateDto dto)
        {
            // A transação é crucial aqui
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Criar a entidade AgendamentoProduto
                var entidade = new AgendamentoProduto
                {
                    AgendamentoId = agendamentoId,
                    ProdutoId = dto.ProdutoId,
                    Quantidade = dto.Quantidade,
                    ValorUnitario = dto.ValorUnitario,
                    CreatedAt = DateTime.UtcNow // Usar UtcNow no servidor
                };

                _context.AgendamentoProdutos.Add(entidade);
                await _context.SaveChangesAsync(); // Salva para obter o 'entidade.Id'

                // 2. Lógica de Pagamento (se foi enviado)
                if (dto.Pagamento != null)
                {
                    // Pega o nome do produto (igual ao código antigo)
                    var produtoNome = await _context.Produtos
                        .AsNoTracking()
                        .Where(p => p.Id == dto.ProdutoId)
                        .Select(p => p.Nome)
                        .FirstOrDefaultAsync();

                    if (produtoNome == null)
                    {
                        await tx.RollbackAsync();
                        throw new KeyNotFoundException($"Produto com ID {dto.ProdutoId} não encontrado.");
                    }

                    _context.Pagamentos.Add(new Pagamento
                    {
                        AgendamentoId = agendamentoId,
                        Valor = entidade.ValorTotal, // Usa a propriedade calculada!
                        DataPagamento = dto.Pagamento.DataPagamento ?? DateTime.UtcNow,
                        Metodo = dto.Pagamento.Metodo ?? MetodoPagamento.Pix,
                        Observacao = dto.Pagamento.Observacao ?? $"Produto: {produtoNome}",
                        AgendamentoProdutoId = entidade.Id // Link crucial
                    });

                    await _context.SaveChangesAsync();
                }

                // 3. Se tudo deu certo, comita a transação
                await tx.CommitAsync();

                // 4. Carrega a entidade completa para o Mapper (com o nome do Produto)
                var entidadeCompleta = await _context.AgendamentoProdutos
                    .AsNoTracking()
                    .Include(ap => ap.Produto) // Carrega a navegação
                    .FirstAsync(ap => ap.Id == entidade.Id);

                // 5. Retorna o DTO de Resposta usando o Mapper
                return entidadeCompleta.ToDto();
            }
            catch (Exception)
            {
                await tx.RollbackAsync(); // Desfaz tudo se der erro
                throw; // Re-lança a exceção para o controller tratar
            }
        }
    }
}
