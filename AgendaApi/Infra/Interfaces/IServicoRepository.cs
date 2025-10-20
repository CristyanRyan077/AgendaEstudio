using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IServicoRepository
    {
        Task<IEnumerable<Servico>> GetAllAsync();
        Task<Servico?> GetByIdAsync(int id);
        Task AddAsync(Servico servico);
        Task UpdateAsync(Servico servico);
        Task DeleteAsync(int id);
    }
}