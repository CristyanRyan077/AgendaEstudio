using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AgendaApi.Infra.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly AgendaContext _context;
        public PagamentoRepository(AgendaContext context) => _context = context;
        public async Task AddAsync(Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento != null)
            {
                _context.Pagamentos.Remove(pagamento);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Pagamento>> GetAllAsync()
        {
            return await _context.Pagamentos
                .Include(p => p.Agendamento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pagamento?> GetByIdAsync(int id)
        {
            return await _context.Pagamentos
                .Include(p => p.Agendamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();
        }
        public async Task<ResumoAgendamentoDto?> ObterResumoAgendamentoAsync(int agendamentoId)
        {          
            return await _context.Agendamentos
                .AsNoTracking()
                .Where(a => a.Id == agendamentoId)
                .Select(a => new ResumoAgendamentoDto(
                    a.Cliente.Id,
                    a.Cliente.Nome,
                    a.Servico != null ? a.Servico.Nome : "—",
                    a.Data,
                    a.Valor
                ))
                .FirstOrDefaultAsync();
        }
        public async Task<List<HistoricoFinanceiroDto>> ListarHistoricoAsync(int agendamentoId)
        {
            // 1. Pagamentos
            var pagamentos = await _context.Pagamentos
                 .AsNoTracking()
                 .Where(p => p.AgendamentoId == agendamentoId)
                 .ToListAsync();

            // 2. Produtos
            var produtos = await _context.AgendamentoProdutos
                .AsNoTracking()
                .Where(ap => ap.AgendamentoId == agendamentoId)
                .Include(ap => ap.Produto)
                .Include(ap => ap.Agendamento) // traz a data
                .ToListAsync();

            var pagosViaProduto = pagamentos
                 .Where(p => p.AgendamentoProdutoId != null)
                 .Select(p => p.AgendamentoProdutoId!.Value)
                 .ToHashSet();

            var historicoPagamentos = pagamentos.Select(p => new HistoricoFinanceiroDto(
             p.Id,
             p.DataPagamento,
             "Pagamento",
             p.Observacao ?? "Pagamento de serviço",
             p.Valor,
             p.Metodo
         ));
            var historicoProdutos = produtos
            .Where(ap => ap.Agendamento != null)
            .Select(ap => new HistoricoFinanceiroDto(
                ap.Id,
                ap.CreatedAt,
                "Produto",
                ap.Produto.Nome,
                ap.ValorUnitario * ap.Quantidade,
                null
            ));
            var historicoProdutosNaoPagos = produtos
                .Where(ap => !pagosViaProduto.Contains(ap.Id))
                .Select(ap => new HistoricoFinanceiroDto(
                    ap.Id, ap.CreatedAt, "Produto", ap.Produto.Nome, ap.ValorTotal, null));

            return historicoPagamentos
           .Union(historicoProdutosNaoPagos)
           .OrderBy(h => h.Data)
           .ToList();
          
        }

    }
}
