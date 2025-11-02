using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AgendaWPF.Services
{
    public interface IAgendamentoService
    {
        Task<List<AgendamentoDto>> ObterAgendamentosAsync();
        Task<List<AgendamentoDto>> ObterAgendamentosPorPeriodo(DateTime inicio, DateTime fim);
        Task<PagamentoDto> AddPagamentoAsync(int agendamentoId, PagamentoCreateDto dto, CancellationToken ct = default);
        Task<AgendamentoDto> AgendarAsync(AgendamentoCreateDto dto, CancellationToken ct = default);
        Task<AgendamentoDto> GetByIdAsync(int id);
        Task ReagendarAsync(int id, ReagendarDto dto);
    }

    public class AgendamentoService : IAgendamentoService
    {
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.30.121:5000/")
        };
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
        public async Task<AgendamentoDto> GetByIdAsync(int id)
        {
            var agendamento = await _http.GetFromJsonAsync<AgendamentoDto>($"api/agendamentos/{id}");
            return agendamento ?? new AgendamentoDto();
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
        public async Task<PagamentoDto> AddPagamentoAsync(int agendamentoId, PagamentoCreateDto dto, CancellationToken ct = default)
        {
            var url = $"api/agendamentos/{agendamentoId}/pagamentos";
            using var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<PagamentoDto>(cancellationToken: ct);
            else
                throw new Exception($"Erro ao adicionar pagamento: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync(ct)}");
        }
        public async Task<AgendamentoDto> AgendarAsync(AgendamentoCreateDto dto, CancellationToken ct = default)
        {
            var url = $"api/agendamentos/";
            var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<AgendamentoDto>(cancellationToken: ct);
            else
                throw new Exception($"Erro ao agendar: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync(ct)}");
        }
        public async Task ReagendarAsync(int id, ReagendarDto dto)
        {
            var url = $"api/agendamentos/{id}/reagendar";
            var response = await _http.PatchAsJsonAsync(url, dto);
            if (!response.IsSuccessStatusCode)
            {
                string erro = await response.Content.ReadAsStringAsync();
                throw new Exception(erro);
            }
        }
    }
}
