using AgendaApi.Domain.Models;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Infra.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AgendaContext _context;
        public ClienteRepository(AgendaContext context) => _context = context;


        public async Task AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity != null)
            {
                _context.Clientes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Agendamento>> GetAgendamentos(int clienteId)
        {
            return await _context.Agendamentos
                .Where(a => a.ClienteId == clienteId)
                .Include(a => a.Pagamentos)
                .Include(a => a.Cliente)
                .Include(a => a.Servico)
                .Include(a => a.Crianca)
                .Include(a => a.Pacote)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes
                .Include(c => c.Criancas)
                .Include(c => c.Agendamentos)
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
               .Include(c => c.Criancas)
               .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<Cliente>> GetPaginadoAsync(int page, int pageSize, string? nome)
        {
            var query = _context.Clientes
                .Include(c => c.Criancas)
                .Include(c => c.Agendamentos)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(c => c.Nome.Contains(nome));

            var total = await query.CountAsync();

            var clientes = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Cliente>
            {
                Items = clientes,
                TotalItems = total,
                Page = page,
                PageSize = pageSize
            };
        }
    };
}
