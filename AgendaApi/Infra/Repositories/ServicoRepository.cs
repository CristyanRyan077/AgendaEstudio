using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Infra.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly AgendaContext _context;
        public ServicoRepository(AgendaContext context) => _context = context;

        public async Task AddAsync(Servico servico)
        {
            await _context.Servicos.AddAsync(servico);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Servicos.FindAsync(id);
            if (entity != null)
            {
                _context.Servicos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Servico>> GetAllAsync()
        {
            return await _context.Servicos
                .Include(s => s.Pacotes)
                .ToListAsync();
        }

        public async Task<Servico?> GetByIdAsync(int id)
        {
            return await _context.Servicos
                .Include(s => s.Pacotes)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Servico servico)
        {
            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();
        }
    }
}