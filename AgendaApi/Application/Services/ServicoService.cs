using AgendaApi.Domain.DTOs;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Models;

namespace AgendaApi.Application.Services
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepository _repository;
        public ServicoService(IServicoRepository repository) => _repository = repository;

        public async Task<Servico> GetServicoOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Serviço com Id {id} não encontrado.");
        }

        public async Task<IEnumerable<ServicoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(s => s.ToDto());
        }

        public async Task<ServicoDto?> GetByIdAsync(int id)
        {
            var s = await GetServicoOrThrowAsync(id);
            return s.ToDto();
        }

        public async Task<ServicoDto> CreateAsync(ServicoCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<ServicoDto?> UpdateAsync(int id, ServicoUpdateDto dto)
        {
            var s = await GetServicoOrThrowAsync(id);
            s.UpdateEntity(dto);
            await _repository.UpdateAsync(s);
            return s.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var s = await GetServicoOrThrowAsync(id);
            await _repository.DeleteAsync(s.Id);
            return true;
        }
    }
}