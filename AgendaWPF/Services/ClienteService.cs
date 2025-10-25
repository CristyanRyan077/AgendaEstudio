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
