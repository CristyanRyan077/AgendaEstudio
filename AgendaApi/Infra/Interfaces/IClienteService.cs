using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente> GetClienteOrThrowAsync(int id);
        Task<IEnumerable<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteCreateDto dto);
        Task<ClienteDto?> UpdateAsync(int id, ClienteUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
