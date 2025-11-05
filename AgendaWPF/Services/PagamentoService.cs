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
    }
}
