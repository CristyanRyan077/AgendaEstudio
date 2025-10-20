using AgendaApi.Domain.DTOs;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;

namespace AgendaApi.Application.Services
{
    public class CriancaService : ICriancaService
    {
        private readonly ICriancaRepository _repository;
        public CriancaService(ICriancaRepository repository) => _repository = repository;

        public async Task<Crianca> GetCriancaOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Criança com Id {id} não encontrada.");
        }

        public async Task<IEnumerable<CriancaDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(c => c.ToDto());
        }

        public async Task<CriancaDto?> GetByIdAsync(int id)
        {
            var cri = await GetCriancaOrThrowAsync(id);
            return cri.ToDto();
        }

        public async Task<CriancaDto> CreateAsync(CriancaCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<CriancaDto?> UpdateAsync(int id, CriancaUpdateDto dto)
        {
            var cri = await GetCriancaOrThrowAsync(id);
            cri.UpdateEntity(dto);
            await _repository.UpdateAsync(cri);
            return cri.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cri = await GetCriancaOrThrowAsync(id);
            await _repository.DeleteAsync(cri.Id);
            return true;
        }
    }
}