using AgendaShared.DTOs;
using AgendaWPF.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public interface IAcoesService
    {
        // 1) Carrega tudo que a tela de edição precisa
       // Task<AgendamentoUpdateDto?> PrepararEdicaoAsync(int agendamentoId);

        // 2) Monta PagamentosVM já carregado (sem abrir a janela)
        Task<PagamentosViewModel> CriarPagamentosViewModelAsync(int agendamentoId);

        // 3) Historico do cliente (dados prontos)
       // Task<IReadOnlyList<AgendamentoHistoricoVM>> ObterHistoricoClienteAsync(int clienteId);
    }

    public class AcoesService : IAcoesService
    {
        private readonly IAgendamentoService _ag;
        private readonly IClienteService _cli;
        private readonly IServicoService _srv;
        private readonly IPagamentoService _pag;
        private readonly IMessenger _msg;

        public AcoesService(
          IAgendamentoService ag, IClienteService cli,
          IServicoService srv,
          IPagamentoService pag, IMessenger msg)
        {
            _ag = ag; _cli = cli; _srv = srv; _pag = pag; _msg = msg;
        }
        public async Task<PagamentosViewModel> CriarPagamentosViewModelAsync(int agendamentoId)
        {
            var vm = new PagamentosViewModel(_pag, agendamentoId, _ag, _cli, _msg);
            await vm.CarregarAsync();
            return vm;
        }
    }
}
