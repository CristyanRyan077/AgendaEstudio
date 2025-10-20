using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.Services
{
    public interface IApiService
    {
        Task<List<AgendamentoDto>> ObterAgendamentosAsync();
        Task<List<ClienteDto>> ObterClientesAsync();
    }
    public class ApiService : IApiService
    {
        private readonly HttpClient _http;

        public ApiService()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://192.168.30.121:5000/") };
        }

        public Task<List<AgendamentoDto>> ObterAgendamentosAsync()
            => _http.GetFromJsonAsync<List<AgendamentoDto>>("api/agendamentos");

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
    }
}
