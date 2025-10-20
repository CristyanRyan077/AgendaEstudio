using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface ICriancaRepository
    {
        Task<IEnumerable<Crianca>> GetAllAsync();
        Task<Crianca?> GetByIdAsync(int id);
        Task AddAsync(Crianca crianca);
        Task UpdateAsync(Crianca crianca);
        Task DeleteAsync(int id);
    }
}
