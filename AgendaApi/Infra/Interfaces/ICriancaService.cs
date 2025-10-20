
using AgendaApi.Domain.DTOs;
using AgendaApi.Models;

namespace AgendaApi.Infra.Interfaces
{
    public interface ICriancaService
    {
        Task<Crianca> GetCriancaOrThrowAsync(int id);
        Task<IEnumerable<CriancaDto>> GetAllAsync();
        Task<CriancaDto?> GetByIdAsync(int id);
        Task<CriancaDto> CreateAsync(CriancaCreateDto dto);
        Task<CriancaDto?> UpdateAsync(int id, CriancaUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}