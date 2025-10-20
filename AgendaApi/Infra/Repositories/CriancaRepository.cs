using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Infra.Repositories
{
    public class CriancaRepository : ICriancaRepository
    {
        private readonly AgendaContext _context;
        public CriancaRepository(AgendaContext context) => _context = context;


        public async Task AddAsync(Crianca crianca)
        {
            await _context.Criancas.AddAsync(crianca);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Criancas.FindAsync(id);
            if (entity != null)
            {
                _context.Criancas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Crianca>> GetAllAsync()
        {
            return await _context.Criancas
                .ToListAsync();
        }

        public async Task<Crianca?> GetByIdAsync(int id)
        {
            return await _context.Criancas
               .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Crianca crianca)
        {
            _context.Criancas.Update(crianca);
            await _context.SaveChangesAsync();
        }
    }
}
