using AgendaApi.Domain.Models;
using AgendaApi.Extensions;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Extensions.MiddleWares;
using AgendaApi.Infra.Interfaces;
using AgendaApi.Infra.Repositories;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly ICriancaRepository _criancaRepo;
        public ClienteService(IClienteRepository repository, ICriancaRepository criancaRepo)
        {
            _repository = repository;
            _criancaRepo = criancaRepo;
        }


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
        public async Task<List<ClienteResumoDto>> GetAllResumoAsync()
        {
            var clientes = await _repository.GetAllAsync();
            return clientes.Select(a => a.ToResumoDto()).ToList();
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

            if(dto.Crianca is not null)
            {
                var criancaNova = dto.Crianca.ToEntity();
                criancaNova.ClienteId = cliente.Id; // define FK
                await _criancaRepo.AddAsync(criancaNova);
            }
            return cliente.ToDto();
        }
        public async Task<List<AgendamentoDto>> GetAgendamentosAsync(int clienteId)
        {
            var agendamentos = await _repository.GetAgendamentos(clienteId);
            return agendamentos.Select(a => a.ToDto()).ToList();
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
        public async Task<PagedResult<ClienteDto>> ObterPaginadoAsync(int page, int pageSize, string? nome)
        {
            var result = await _repository.GetPaginadoAsync(page, pageSize, nome);

            return new PagedResult<ClienteDto>
            {
                Items = result.Items.Select(c => c.ToDto()), // usa seu helper
                TotalItems = result.TotalItems,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
