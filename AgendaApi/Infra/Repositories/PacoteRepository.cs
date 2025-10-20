using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Infra.Repositories
{
    public class PacoteRepository : IPacoteRepository
    {
        private readonly AgendaContext _context;
        public PacoteRepository(AgendaContext context) => _context = context;

        public async Task AddAsync(Pacote pacote)
        {
            await _context.Pacotes.AddAsync(pacote);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Pacotes.FindAsync(id);
            if (entity != null)
            {
                _context.Pacotes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Pacote>> GetAllAsync()
        {
            return await _context.Pacotes
                .Include(p => p.Servico)
                .ToListAsync();
        }

        public async Task<Pacote?> GetByIdAsync(int id)
        {
            return await _context.Pacotes
                .Include(p => p.Servico)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Pacote pacote)
        {
            _context.Pacotes.Update(pacote);
            await _context.SaveChangesAsync();
        }
    }
}