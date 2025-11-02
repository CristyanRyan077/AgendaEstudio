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
        Task<CriancaDto> CreateCriancaAsync(int clienteId, CriancaCreateDto criancaCreateDto);
        Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto dto, CancellationToken ct = default);
        public ClienteResumoDto? DetectExistingLocal(string? telefone, string? email);
        Task PrecarregarCacheClientesAsync(CancellationToken ct = default);
        Task DeleteAsync(int id);
        Task DeleteCriancaAsync(int id);

    }

    public class ClienteService : IClienteService
    {
        private List<ClienteResumoDto> _cacheClientes = new();
        public bool CachePronto { get; private set; }

        private readonly HttpClient _http;
        public ClienteService()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://192.168.30.121:5000/") };
        }
        public async Task PrecarregarCacheClientesAsync(CancellationToken ct = default)
        {
            CachePronto = false;
            _cacheClientes = await _http.GetFromJsonAsync<List<ClienteResumoDto>>("api/clientes/resumos", ct)
                              ?? new List<ClienteResumoDto>();
            CachePronto = true;
        }
        public ClienteResumoDto? DetectExistingLocal(string? telefone, string? email)
        {
            string NormTel(string? t) => string.IsNullOrWhiteSpace(t) ? "" : new string(t.Where(char.IsDigit).ToArray());
            string NormMail(string? m) => string.IsNullOrWhiteSpace(m) ? "" : m.Trim().ToLowerInvariant();

            var tel = NormTel(telefone);
            var mail = NormMail(email);

            if (!string.IsNullOrEmpty(tel))
            {
                var byTel = _cacheClientes.FirstOrDefault(c => NormTel(c.Telefone) == tel);
                if (byTel != null) return byTel;
            }
            if (!string.IsNullOrEmpty(mail))
            {
                var byMail = _cacheClientes.FirstOrDefault(c => NormMail(c.Email) == mail);
                if (byMail != null) return byMail;
            }
            return null;
        }
        public async Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto dto, CancellationToken ct = default)
        {
            try
            {
                var resp = await _http.PutAsJsonAsync($"api/clientes/{id}", dto, ct);

                // Alguns endpoints retornam 204 NoContent no update:
                if (resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // opcional: buscar o atualizado
                    return await GetByIdAsync(id);
                }

                resp.EnsureSuccessStatusCode();
                var updated = await resp.Content.ReadFromJsonAsync<ClienteDto>(cancellationToken: ct);
                return updated ?? new ClienteDto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new ClienteDto();
            }
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
        public async Task<CriancaDto> CreateCriancaAsync(int id, CriancaCreateDto criancaCreateDto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"api/criancas/by-cliente/{id}", criancaCreateDto);
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
                var criancasdocliente = await _http.GetFromJsonAsync<List<CriancaDto>>($"api/criancas/{id}/by-clienteId");
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
        public async Task DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/clientes/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return;
            resp.EnsureSuccessStatusCode();
        }
        public async Task DeleteCriancaAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/criancas/{id}");
            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return;
            resp.EnsureSuccessStatusCode();
        }
    }
}
