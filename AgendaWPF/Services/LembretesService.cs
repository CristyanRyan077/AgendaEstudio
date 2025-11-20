using AgendaShared;
using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public interface ILembreteService
    {
        Task<LembreteDto> CreateAsync(LembreteCreateDto dto, CancellationToken ct = default);
      //  Task<LembreteDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<LembreteDto>> ListAsync(LembreteQuery filtro, CancellationToken ct = default);

    }
    public class LembretesService : ILembreteService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {

            PropertyNameCaseInsensitive = true

        };
        public LembretesService(HttpClient httpClient) => _http = httpClient;
        private string BuildQueryString(LembreteQuery filtro)
        {
            var parameters = new List<string>();

            parameters.Add($"Inicio={filtro.Inicio:yyyy-MM-ddTHH:mm:ss}");
            parameters.Add($"Fim={filtro.Fim:yyyy-MM-ddTHH:mm:ss}");

            // 2. IDs e Status
            if (filtro.ClienteId.HasValue)
                parameters.Add($"ClienteId={filtro.ClienteId.Value}");

            if (filtro.AgendamentoId.HasValue)
                parameters.Add($"AgendamentoId={filtro.AgendamentoId.Value}");

            if (filtro.Status.HasValue)
                // Envia o nome do Enum (string)
                parameters.Add($"Status={filtro.Status.Value}");
            if (!parameters.Any())
                return string.Empty;

            return "?" + string.Join("&", parameters);
        }
        public async Task<LembreteDto> CreateAsync(LembreteCreateDto dto, CancellationToken ct = default)
        {
            var url = $"api/Lembretes";
            using var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<LembreteDto>(cancellationToken: ct)
                    ?? throw new Exception("A API retornou uma resposta bem-sucedida, mas o conteúdo estava vazio (null).");
            }
            else
                throw new Exception($"Erro ao agendar: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync(ct)}");
        }
        public async Task<IReadOnlyList<LembreteDto>> ListAsync(LembreteQuery filtro, CancellationToken ct = default)
        {
            var query = BuildQueryString(filtro);
            var url = $"api/Lembretes{query}";

            System.Diagnostics.Debug.WriteLine($"LembretesService] GET {_http.BaseAddress}{url}");

            var result = await _http.GetFromJsonAsync<IReadOnlyList<LembreteDto>>(url, _jsonOptions, ct);
            return result ?? new List<LembreteDto>();
        }
    }
}
