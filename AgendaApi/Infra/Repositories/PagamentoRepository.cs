using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Pagamentos
            .AsNoTracking()
            .Where(p => p.AgendamentoId == agendamentoId)
            .OrderBy(p => p.DataPagamento)
            .Select(p => new HistoricoFinanceiroDto(
                p.Id,
                p.DataPagamento,
                p.Tipo,
                p.Observacao ?? $"Pagamento do agendamento {agendamentoId}",
                p.Valor,
                p.Metodo 
            ))
            .ToListAsync();
        }
    }
}
