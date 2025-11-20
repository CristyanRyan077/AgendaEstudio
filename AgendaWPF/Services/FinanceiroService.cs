using AgendaShared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public interface IFinanceiroService
    {
        Task<FinanceiroResumo> CalcularKpisAsync(FinanceiroFiltroRequest filtro);
        Task<List<RecebivelDTO>> ListarEmAbertoAsync(FinanceiroFiltroRequest filtro);
        Task<List<ServicoFaturamentoDTO>> ResumoPorServicoAsync(FinanceiroFiltroRequest filtro);
        Task<List<ProdutoResumoVM>> ResumoPorProdutoAsync(FinanceiroFiltroRequest filtro);
    }
    public class FinanceiroService : IFinanceiroService
    {
        private readonly HttpClient _http;
        public FinanceiroService(HttpClient httpClient) => _http = httpClient;
        private string BuildQueryString(FinanceiroFiltroRequest filtro)
        {
            var parameters = new List<string>();

            // 1. Datas (Formato ISO 8601 para garantia)
            parameters.Add($"Inicio={filtro.Inicio:yyyy-MM-ddTHH:mm:ss}");
            parameters.Add($"Fim={filtro.Fim:yyyy-MM-ddTHH:mm:ss}");

            // 2. IDs e Status
            if (filtro.ServicoId.HasValue)
                parameters.Add($"ServicoId={filtro.ServicoId.Value}");

            if (filtro.ProdutoId.HasValue)
                parameters.Add($"ProdutoId={filtro.ProdutoId.Value}");

            if (filtro.Status.HasValue)
                // Envia o nome do Enum (string)
                parameters.Add($"Status={filtro.Status.Value}");

            // 3. Nome do Cliente (Obrigatório o Encoding!)
            if (!string.IsNullOrWhiteSpace(filtro.ClienteNome))
            {
                // Codifica o valor para evitar que caracteres como '&' ou espaço quebrem a URL
                var encodedName = Uri.EscapeDataString(filtro.ClienteNome.Trim());
                parameters.Add($"ClienteNome={encodedName}");
            }

            // Junta todos os parâmetros com '&' e adiciona o '?' inicial
            return "?" + string.Join("&", parameters);
        }

        public async Task<FinanceiroResumo> CalcularKpisAsync(FinanceiroFiltroRequest filtro)
        {
            // 1. Validação (Opcional, mas altamente recomendada)
            if (filtro.Inicio > filtro.Fim)          
                throw new ArgumentException("A data de início não pode ser posterior à data de fim.");

            var query = BuildQueryString(filtro);
            var url = $"api/Financeiro/kpis{query}";

            System.Diagnostics.Debug.WriteLine($"[Financeiroservice] GET {url}");

            var result = await _http.GetFromJsonAsync<FinanceiroResumo>(url);
            return result ?? new FinanceiroResumo();
        }
        public async Task<List<RecebivelDTO>> ListarEmAbertoAsync(FinanceiroFiltroRequest filtro)
        {
            var query = BuildQueryString(filtro);
            var url = $"api/Financeiro/recebiveis{query}";

            var result = await _http.GetFromJsonAsync<List<RecebivelDTO>>(url);
            return result ?? new List<RecebivelDTO>();
       }
        public async Task<List<ServicoFaturamentoDTO>> ResumoPorServicoAsync(FinanceiroFiltroRequest filtro)
        {
            var query = BuildQueryString(filtro);
            var url = $"api/Financeiro/resumo/servicos{query}";

            var result = await _http.GetFromJsonAsync<List<ServicoFaturamentoDTO>>(url);
            return result ?? new List<ServicoFaturamentoDTO>();
        }
        public async Task<List<ProdutoResumoVM>> ResumoPorProdutoAsync(FinanceiroFiltroRequest filtro)
        {
            var query = BuildQueryString(filtro);
            var url = $"api/Financeiro/resumo/produtos{query}";

            var result = await _http.GetFromJsonAsync<List<ProdutoResumoVM>>(url);
            return result ?? new List<ProdutoResumoVM>();
        }
    }
}
