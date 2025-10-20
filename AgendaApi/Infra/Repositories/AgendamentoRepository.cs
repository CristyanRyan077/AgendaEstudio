using AgendaApi.Interfaces;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;
using AgendaApi.Infra;

namespace AgendaApi.Repositories
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AgendaContext _context;
        public AgendamentoRepository(AgendaContext context)  => _context = context;

        public async Task<IEnumerable<Agendamento>> GetAllAsync()
        {
            return await _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Crianca)
                .Include(a => a.Servico)
                .Include(a => a.Pacote)
                .Include(a => a.Pagamentos)
                .ToListAsync();
        }

        public async Task<Agendamento?> GetByIdAsync(int id)
        {
            return await _context.Agendamentos
                .Include(a => a.Pagamentos)
                .Include(a => a.Cliente)
                .Include(a => a.Crianca)
                .Include(a => a.Servico)
                .Include(a => a.Pacote)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Agendamento agendamento)
        {
            await _context.Agendamentos.AddAsync(agendamento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Agendamentos.FindAsync(id);
            if (entity != null)
            {
                _context.Agendamentos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

    } 
} 
