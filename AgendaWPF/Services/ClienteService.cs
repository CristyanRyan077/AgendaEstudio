using AgendaApi.Domain.Models;
using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AgendaWPF.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> ObterClientesAsync();
        Task<PagedResult<ClienteDto>> GetClientesPaginadoAsync(int page, int pagesize, string? nome);
        Task<List<CriancaDto>> GetByClienteIdAsync(int id);
        Task<List<AgendamentoDto>> GetHistoricoAsync(int id);
        Task<ClienteDto> GetByIdAsync(int id);
        Task<ClienteDto> CreateClienteAsync(ClienteCreateDto clienteCreateDto);
        Task<CriancaDto> CreateCriancaAsync(CriancaCreateDto criancaCreateDto);
    }
    public class ClienteService : IClienteService
    {
        private readonly HttpClient _http;
        public ClienteService()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://192.168.30.121:5000/") };
        }
        public async Task<List<ClienteDto>> ObterClientesAsync()
        {
            try
            {
                var clientes = await _http.GetFromJsonAsync<List<ClienteDto>>("api/clientes");
                return clientes ?? new List<ClienteDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<ClienteDto>();
            }
        }
        public async Task<ClienteDto> GetByIdAsync(int id)
        {
            try
            {
                var cliente = await _http.GetFromJsonAsync<ClienteDto>($"api/clientes/{id}");
                return cliente ?? new ClienteDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new ClienteDto();
            }

        }
        public async Task<List<AgendamentoDto>> GetHistoricoAsync(int id)
        {
            try
            {
                var agendamentos = await _http.GetFromJsonAsync<List<AgendamentoDto>>($"api/clientes/{id}/agendamentos");
                return agendamentos ?? new List<AgendamentoDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<AgendamentoDto>();
            }
        }

        public async Task<ClienteDto> CreateClienteAsync(ClienteCreateDto clienteCreateDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/clientes", clienteCreateDto);
                response.EnsureSuccessStatusCode();
                var createdCliente = await response.Content.ReadFromJsonAsync<ClienteDto>();
                return createdCliente ?? new ClienteDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new ClienteDto();
            }
        }
        public async Task<CriancaDto> CreateCriancaAsync(CriancaCreateDto criancaCreateDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/criancas", criancaCreateDto);
                response.EnsureSuccessStatusCode();
                var createdCrianca = await response.Content.ReadFromJsonAsync<CriancaDto>();
                return createdCrianca ?? new CriancaDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new CriancaDto();
            }
        }

        public async Task<List<CriancaDto>> GetByClienteIdAsync(int id)
        {
            try
            {
                var criancasdocliente = await _http.GetFromJsonAsync<List<CriancaDto>>("api/criancas/{id}/by-clienteId");
                return criancasdocliente ?? new List<CriancaDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<CriancaDto>();
            }
        }
        public async Task<PagedResult<ClienteDto>>GetClientesPaginadoAsync(int page, int pagesize, string? nome)
        {
            try
            {
                var query = $"?page={page}&pageSize={pagesize}";
                if (!string.IsNullOrWhiteSpace(nome))
                    query += $"&nome={Uri.EscapeDataString(nome)}";

                var url = $"api/clientes/paginado{query}";
                var result = await _http.GetFromJsonAsync<PagedResult<ClienteDto>>(url);

                return result ?? new PagedResult<ClienteDto>();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[API ERROR] {ex.Message}");
                return new PagedResult<ClienteDto>();
            }

        }
    }
}
