using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IServicoService
    {
        Task<Servico> GetServicoOrThrowAsync(int id);
        Task<IEnumerable<ServicoDto>> GetAllAsync();
        Task<ServicoDto?> GetByIdAsync(int id);
        Task<ServicoDto> CreateAsync(ServicoCreateDto dto);
        Task<ServicoDto?> UpdateAsync(int id, ServicoUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}