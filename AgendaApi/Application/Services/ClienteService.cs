using AgendaApi.Domain.DTOs;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Infra.Repositories;
using AgendaApi.Models;

namespace AgendaApi.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        public ClienteService(IClienteRepository repository) => _repository = repository;

        public async Task<Cliente> GetClienteOrThrowAsync(int id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Cliente com Id {id} não encontrado.");
        }
        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            var clientes = await _repository.GetAllAsync();
            return clientes.Select(a => a.ToDto());
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var cliente = await GetClienteOrThrowAsync(id);
            return cliente.ToDto();
        }

        public async Task<ClienteDto> CreateAsync(ClienteCreateDto dto)
        {
            var cliente = dto.ToEntity();
            await _repository.AddAsync(cliente);
            return cliente.ToDto();
        }

        public async Task<ClienteDto?> UpdateAsync(int id, ClienteUpdateDto dto)
        {
            var cliente = await GetClienteOrThrowAsync(id);
            cliente.UpdateEntity(dto);
            await _repository.UpdateAsync(cliente);

            return cliente.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {

            var cliente = await GetClienteOrThrowAsync(id);
            await _repository.DeleteAsync(cliente.Id);
            return true;
        }
    }
}
