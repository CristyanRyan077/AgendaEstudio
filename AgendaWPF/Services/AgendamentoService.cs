using AgendaShared;
using AgendaShared.DTOs;
using AgendaShared.Helpers;
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
        Task<List<AgendamentoDto>> SearchAgendamentoAsync(AgendamentoSearchFilter filtro);
        Task UpdateStatus(int id, StatusUpdateDto status);
    }

    public class AgendamentoService : IAgendamentoService
    {
        private readonly HttpClient _http;

        public AgendamentoService(HttpClient httpClient) => _http = httpClient;

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
                var parametros = new List<string>
                {
                    // Parâmetros de data que são obrigatórios
                    $"inicio={inicio:yyyy-MM-dd}",
                    $"fim={fim:yyyy-MM-dd}"
                };
              
                var queryString = string.Join("&", parametros);
                var url = $"api/agendamentos/periodo?{queryString}";

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
            {
                return await resp.Content.ReadFromJsonAsync<PagamentoDto>(cancellationToken: ct)
                    ?? throw new Exception("A API retornou uma resposta bem-sucedida, mas o conteúdo estava vazio (null).");
            }
            else
                throw new Exception($"Erro ao adicionar pagamento: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync(ct)}");
        }
        public async Task<AgendamentoDto> AgendarAsync(AgendamentoCreateDto dto, CancellationToken ct = default)
        {
            var url = $"api/agendamentos/";
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            Debug.WriteLine("[AGENDAR JSON] " + json);
            var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<AgendamentoDto>(cancellationToken: ct)
                    ?? throw new Exception("A API retornou uma resposta bem-sucedida, mas o conteúdo estava vazio (null).");
            }
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
        public async Task UpdateStatus(int id, StatusUpdateDto status)
        {
            var url = $"api/agendamentos/{id}/atualizarStatus";
            var response = await _http.PatchAsJsonAsync(url, status);

            if (!response.IsSuccessStatusCode)
            {
                string erro = await response.Content.ReadAsStringAsync();
                throw new Exception(erro);
            }
        }
        public async Task<List<AgendamentoDto>> SearchAgendamentoAsync(AgendamentoSearchFilter filtro)
        {
            var parametros = new List<string>();

            if (!string.IsNullOrWhiteSpace(filtro.SearchTerm))
            {
                var termoCodificado = Uri.EscapeDataString(filtro.SearchTerm);
                parametros.Add($"searchTerm={termoCodificado}");
            }
            if (filtro.TiposDeFotoSelecionados != null && filtro.TiposDeFotoSelecionados.Any())
            {

                foreach (var tipo in filtro.TiposDeFotoSelecionados)
                {
                 
                    var tipoCodificado = Uri.EscapeDataString(tipo.ToString());
                    parametros.Add($"tiposDeFotoSelecionados={tipoCodificado}");
                }
            }
            var queryString = parametros.Any() ? "?" + string.Join("&", parametros) : string.Empty;
            var url = $"api/agendamentos/search{queryString}";

            var agendamentos = await _http.GetFromJsonAsync<List<AgendamentoDto>>(url);
            return agendamentos ?? new List<AgendamentoDto>();
        }
    }
}
