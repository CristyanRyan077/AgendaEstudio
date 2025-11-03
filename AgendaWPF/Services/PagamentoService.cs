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
    }
    public class PagamentoService :IPagamentoService
    {
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.30.121:5000/")
        };
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
    }
}
