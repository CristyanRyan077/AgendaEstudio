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
    public interface IServicoService
    {
        Task<List<ServicoDto>> GetAllAsync();
        Task<List<PacoteDto>> GetPacotesAsync();
    }

    public class ServicoService : IServicoService
    {
        private readonly HttpClient _http;

        public ServicoService(HttpClient httpClient) => _http = httpClient;
        public async Task<List<ServicoDto>> GetAllAsync()
        {
            try
            {
                var servicos = await _http.GetFromJsonAsync<List<ServicoDto>>("api/Servicos");
                return servicos ?? new List<ServicoDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<ServicoDto>();
            }
        }
        public async Task<List<PacoteDto>> GetPacotesAsync()
        {
            try
            {
                var pacotes = await _http.GetFromJsonAsync<List<PacoteDto>>("api/Pacotes");
                return pacotes ?? new List<PacoteDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<PacoteDto>();
            }
        }
    }
}
