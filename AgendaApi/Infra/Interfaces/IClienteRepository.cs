using AgendaApi.Domain.Models;
using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
        Task<PagedResult<Cliente>> GetPaginadoAsync(int page, int pageSize, string? nome);
    }
}
