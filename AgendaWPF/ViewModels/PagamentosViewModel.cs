using AgendaShared;
using AgendaShared.DTOs;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AgendaWPF.ViewModels
{
    public partial class PagamentosViewModel : ObservableObject
    {
        private readonly IPagamentoService _service;
        private readonly IAgendamentoService _agendaservice;
        private readonly IClienteService _clienteservice;
        private readonly IMessenger _messenger;
        // private readonly IProdutoService _produtoservice;
        public int AgendamentoId { get; }
        public PagamentosViewModel(IPagamentoService service,
            int agendamentoId,
            IAgendamentoService agendaservice,
            IClienteService clienteservice, IMessenger messenger)
        {
            _service = service;
            AgendamentoId = agendamentoId;
            _agendaservice = agendaservice;
            _clienteservice = clienteservice;
            _messenger = messenger;
            // _produtoservice = produtoservice;
            // LimparFormulario();

        }
        // Resumo / cabeçalho
        [ObservableProperty] private string? clienteNome;
        [ObservableProperty] private string? servicoNome;
        [ObservableProperty] private DateTime dataAgendamento;
        [ObservableProperty] private decimal valor;
        // [ObservableProperty] private ObservableCollection<ProdutoDto> produtosDisponiveis = new();

        // editar
        [ObservableProperty] private bool estaEditando;
        [ObservableProperty] private int id;
        [ObservableProperty] private int clienteId;
        [ObservableProperty] private DateTime dataPagamento = DateTime.Today;
        [ObservableProperty] private string? observacao;

        // Colunas calculadas
        public decimal ValorPago => Historico?.Sum(h => h.Valor) ?? 0m;
        public decimal Falta => Math.Max(0, Valor - ValorPago);
        public int PercentualPago => Valor <= 0 ? 0 : (int)Math.Round(Math.Min(ValorPago, Valor) / Valor * 100m);
        public string TextoBotaoPrimario =>
        tipoLancamento == TipoLancamento.Pagamento ? "Adicionar Pagamento" : "Adicionar Produto";
        // Listas
        [ObservableProperty] private HistoricoFinanceiroDto? itemSelecionado;
        [ObservableProperty] private ObservableCollection<HistoricoFinanceiroDto> pagamentos = new();
        public ObservableCollection<MetodoPagamento> Metodo { get; } =
        new ObservableCollection<MetodoPagamento>((MetodoPagamento[])Enum.GetValues(typeof(MetodoPagamento)));

        // Novo pagamento (inputs)
        [ObservableProperty] private ObservableCollection<HistoricoFinanceiroDto> historico = new();
        [ObservableProperty] private PagamentoCreateDto novoPagamento = new();
        //[ObservableProperty] private ProdutoCreateDto novoProduto = new();
        [ObservableProperty] private bool modoProduto;
        //[ObservableProperty] private ProdutoDto? produtoSelecionado;
        [ObservableProperty]
        private TipoLancamento tipoLancamento = TipoLancamento.Pagamento;

        partial void OnHistoricoChanged(ObservableCollection<HistoricoFinanceiroDto> value)
        {
            OnPropertyChanged(nameof(ValorPago));
            OnPropertyChanged(nameof(Falta));
            OnPropertyChanged(nameof(PercentualPago));
        }
        partial void OnTipoLancamentoChanged(TipoLancamento value)
        {
            // Mantém compatibilidade com a sua lógica já existente
            ModoProduto = (value == TipoLancamento.Produto);
            OnPropertyChanged(nameof(TextoBotaoPrimario));
        }
        [RelayCommand]
        public async Task CarregarAsync()
        {
            try
            {
                var header = await _service.GetResumoAsync(AgendamentoId);
                ClienteId = header.ClienteId;
                ClienteNome = header.ClienteNome;
                ServicoNome = header.ServicoNome;
                DataAgendamento = header.Data;
                Valor = header.Valor;

                //CarregarProdutos();
                OnPropertyChanged(nameof(ValorPago));
                OnPropertyChanged(nameof(Falta));
                OnPropertyChanged(nameof(PercentualPago));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no CarregarAsync: {ex}");
                throw;
            }
        }
        public async Task AtualizarStatusAsync()
        {
            var valorPagoAtual = Historico?.Sum(h => h.Valor) ?? 0m;

            if (valorPagoAtual >= Valor)
            {
                // Total pago, marcar como concluído
               /* _clienteservice.AtivarSePendente(clienteId);
                _agendaservice.UpdateStatus(AgendamentoId, StatusAgendamento.Concluido); */
            }
            else
            {
                // Ainda falta valor, deixar como pendente
               /* _clienteservice.ValorIncompleto(clienteId);
                _agendaservice.UpdateStatus(AgendamentoId, StatusAgendamento.Pendente); */
            }
        }
        [RelayCommand]
        public void DefinirValorRapido(object? valor)
        {
            Debug.WriteLine("definindo valor rapido");
            decimal d = 0;

            switch (valor)
            {
                case decimal dec: d = dec; break;
                case string s when decimal.TryParse(s, NumberStyles.Number,
                            CultureInfo.GetCultureInfo("pt-BR"), out var dd):
                    d = dd; break;
                case IConvertible c: d = Convert.ToDecimal(c, CultureInfo.InvariantCulture); break;
                default: return;
            }

            if (d <= 0) return;
            NovoPagamento ??= new();
            NovoPagamento.Valor = d;
            OnPropertyChanged(nameof(NovoPagamento));
        }

        [RelayCommand]
        public async Task SalvarOuAdicionarHistoricoAsync()
        {
            if (!ModoProduto)
            {
                // === PAGAMENTO ===
                if (NovoPagamento.Valor <= 0) return;

                var tipo = this.TipoLancamento;

                var dto = new PagamentoCreateDto
                {
                    Valor = NovoPagamento.Valor,
                    DataPagamento = NovoPagamento.DataPagamento,
                    Metodo = NovoPagamento.Metodo,
                    Observacao = NovoPagamento.Observacao,
                    Tipo = tipo
                };

                var pagamento = await _agendaservice.AddPagamentoAsync(AgendamentoId, dto);
                await CarregarAsync();
                OnPropertyChanged(nameof(ValorPago));
                OnPropertyChanged(nameof(Falta));
                OnPropertyChanged(nameof(PercentualPago));
                _messenger.Send(new PagamentoCriadoMessage(AgendamentoId, pagamento));

                System.Diagnostics.Debug.WriteLine($"[DEBUG] ValorPago={ValorPago} Valor={Valor} Historico.Count={Historico.Count}");
                //await AtualizarStatusAsync();
                //LimparFormulario();
               // NotificarFinanceiro();
            }
        }
    }
}
