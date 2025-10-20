using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IPacoteRepository
    {
        Task<IEnumerable<Pacote>> GetAllAsync();
        Task<Pacote?> GetByIdAsync(int id);
        Task AddAsync(Pacote pacote);
        Task UpdateAsync(Pacote pacote);
        Task DeleteAsync(int id);
    }
}