using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface IPacoteService
    {
        Task<Pacote> GetPacoteOrThrowAsync(int id);
        Task<IEnumerable<PacoteDto>> GetAllAsync();
        Task<PacoteDto?> GetByIdAsync(int id);
        Task<PacoteDto> CreateAsync(PacoteCreateDto dto);
        Task<PacoteDto?> UpdateAsync(int id, PacoteUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}