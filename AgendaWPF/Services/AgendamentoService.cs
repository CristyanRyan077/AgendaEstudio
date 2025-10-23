using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public interface IAgendamentoService
    {
        Task<List<AgendamentoDto>> ObterAgendamentosAsync();
        Task<List<AgendamentoDto>> ObterAgendamentosPorPeriodo(DateTime inicio, DateTime fim);
    }
    public class AgendamentoService : IAgendamentoService
    {
        private readonly HttpClient _http;
        public AgendamentoService()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://192.168.30.121:5000/") };
        }
        public async Task<List<AgendamentoDto>> ObterAgendamentosAsync()
        {
            try
            {
                var agendamentos = await _http.GetFromJsonAsync<List<AgendamentoDto>>("api/agendamentos");
                return agendamentos ?? new List<AgendamentoDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<AgendamentoDto>();
            }
        }

        public async Task<List<AgendamentoDto>> ObterAgendamentosPorPeriodo(DateTime inicio, DateTime fim)
        {
            try
            {
                var url = $"api/agendamentos/periodo?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}";

                var sw = Stopwatch.StartNew();
                var agendamentos = await _http.GetFromJsonAsync<List<AgendamentoDto>>(url);
                sw.Stop();

                Debug.WriteLine($"[API] Tempo de resposta: {sw.ElapsedMilliseconds} ms");
                return agendamentos ?? new List<AgendamentoDto>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API ERROR] {ex.Message}");
                return new List<AgendamentoDto>();
            }
        }
        public async Task<List<ServicoDto>> ObterServicosAsync()
        {
            try
            {
                var servicos = await _http.GetFromJsonAsync<List<ServicoDto>>("api/servicos");
                return servicos ?? new List<ServicoDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<ServicoDto>();
            }
        }
    }
}
