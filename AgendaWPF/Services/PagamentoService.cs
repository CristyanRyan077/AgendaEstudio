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
    public interface IPagamentoService
    {
        Task<List<HistoricoFinanceiroDto>> GetHistoricoAsync(int agendamentoId);
        Task<ResumoAgendamentoDto> GetResumoAsync(int agendamentoId);
        Task<AgendamentoProdutoDto> AdicionarProdutoAsync(int agendamentoId, AgendamentoProdutoCreateDto dto, CancellationToken ct = default);
        Task<List<ProdutoDto>> GetAllProdutosAsync();
    }
    public class PagamentoService : IPagamentoService
    {
        private readonly HttpClient _http;
        public PagamentoService(HttpClient httpClient) => _http = httpClient;
        public async Task<List<HistoricoFinanceiroDto>> GetHistoricoAsync(int agendamentoId)
        {
            try
            {
                var url = $"api/pagamentos/{agendamentoId}/historico";
                var pagamentos = await _http.GetFromJsonAsync<List<HistoricoFinanceiroDto>>(url);
                return pagamentos ?? new List<HistoricoFinanceiroDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new List<HistoricoFinanceiroDto>();
            }
        }
        public async Task<ResumoAgendamentoDto> GetResumoAsync(int agendamentoId)
        {
            try
            {
                var url = $"api/pagamentos/{agendamentoId}/resumo";
                var resumo = await _http.GetFromJsonAsync<ResumoAgendamentoDto>(url);
                return resumo ?? new ResumoAgendamentoDto(0, "—", "—", DateTime.MinValue, 0m);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new ResumoAgendamentoDto(0, "—", "—", DateTime.MinValue, 0m);
            }
        }
        public async Task<AgendamentoProdutoDto> AdicionarProdutoAsync(int agendamentoId, AgendamentoProdutoCreateDto dto, CancellationToken ct = default)
        {
            var url = $"api/Produtos/{agendamentoId}/produtos";
            using var resp = await _http.PostAsJsonAsync(url, dto, ct);
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<AgendamentoProdutoDto>(cancellationToken: ct)
                    ?? throw new Exception("A API retornou uma resposta bem-sucedida, mas o conteúdo estava vazio (null).");
            }
            else
                throw new Exception($"Erro ao adicionar pagamento: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync(ct)}");
        }
        public async Task<List<ProdutoDto>> GetAllProdutosAsync()
        {
            var url = $"api/Produtos/todos";
            var produtos = await _http.GetFromJsonAsync<List<ProdutoDto>>(url);
            return produtos ?? new List<ProdutoDto>();

        }
    }
}
