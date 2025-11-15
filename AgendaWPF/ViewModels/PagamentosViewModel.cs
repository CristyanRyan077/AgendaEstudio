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
using System.Windows;
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
            _ = CarregarProdutosAsync();
            // _produtoservice = produtoservice;
            // LimparFormulario();

        }
        // Resumo / cabeçalho
        [ObservableProperty] private string? clienteNome;
        [ObservableProperty] private string? servicoNome;
        [ObservableProperty] private DateTime dataAgendamento;
        [ObservableProperty] private decimal valor;
        [ObservableProperty] private ObservableCollection<ProdutoDto> produtosDisponiveis = new();

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
        TipoLancamento == TipoLancamento.Pagamento ? "Adicionar Pagamento" : "Adicionar Produto";
        // Listas
        [ObservableProperty] private HistoricoFinanceiroDto? itemSelecionado;
        [ObservableProperty] private ObservableCollection<HistoricoFinanceiroDto> pagamentos = new();
        public ObservableCollection<MetodoPagamento> Metodo { get; } =
        new ObservableCollection<MetodoPagamento>((MetodoPagamento[])Enum.GetValues(typeof(MetodoPagamento)));

        // Novo pagamento (inputs)
        [ObservableProperty] private ObservableCollection<HistoricoFinanceiroDto> historico = new();
        [ObservableProperty] private PagamentoCreateDto novoPagamento = new();
        [ObservableProperty] private AgendamentoProdutoCreateDto novoProduto = new();
        [ObservableProperty] private bool modoProduto;
        [ObservableProperty] private ProdutoDto? produtoSelecionado;
        [ObservableProperty]
        private TipoLancamento tipoLancamento = TipoLancamento.Pagamento;

        partial void OnHistoricoChanged(ObservableCollection<HistoricoFinanceiroDto> value)
        {
            OnPropertyChanged(nameof(ValorPago));
            OnPropertyChanged(nameof(Falta));
            OnPropertyChanged(nameof(PercentualPago));
        }
        public async Task UpdateStatusAsync()
        {
            var status = new StatusUpdateDto { Status = StatusAgendamento.Concluido };
            await _agendaservice.UpdateStatus(AgendamentoId, status);
            await CarregarAsync();
        }
        public async Task CarregarProdutosAsync()
        {
            var produtos = await _service.GetAllProdutosAsync();
            ProdutosDisponiveis = new ObservableCollection<ProdutoDto>(produtos);
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
        public event EventHandler? RequestClose;
        [RelayCommand]
        public async Task SalvarOuAdicionarHistoricoAsync()
        {
            if (NovoPagamento.Valor <= 0) return;

            if (!ModoProduto)
            {
                // === PAGAMENTO ===
                

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
                await UpdateStatusAsync();

                OnPropertyChanged(nameof(ValorPago));
                OnPropertyChanged(nameof(Falta));
                OnPropertyChanged(nameof(PercentualPago));

                

                RequestClose?.Invoke(this, EventArgs.Empty);
                _messenger.Send(new PagamentoCriadoMessage(AgendamentoId, pagamento));

                System.Diagnostics.Debug.WriteLine($"[DEBUG] ValorPago={ValorPago} Valor={Valor} Historico.Count={Historico.Count}");

            }
        }
        [RelayCommand]
        public async Task AdicionarProduto()
        {
            // Adicione uma verificação de nulo
            if (ProdutoSelecionado is null)
            {
                // (Opcional: mostrar um aviso)
                MessageBox.Show("Por favor, selecione um produto."); 
                return; // Para a execução
            }
            if (NovoProduto != null)
            {
                NovoProduto.Pagamento = new PagamentoParaProdutoCreateDto
                {
                    // O Valor será calculado no servidor (entidade.ValorTotal)
                    // Você precisa preencher o método e a data/observação
                    Metodo = MetodoPagamento.Pix, // Supondo que você tem uma propriedade para isso
                    DataPagamento = DateTime.Now,
                    Observacao = $"Produto: {ProdutoSelecionado.Nome}"
                };
                var tipo = this.TipoLancamento;
                var produto = await _service.AdicionarProdutoAsync(AgendamentoId, NovoProduto);
                await UpdateStatusAsync();

                OnPropertyChanged(nameof(ValorPago));
                OnPropertyChanged(nameof(Falta));
                OnPropertyChanged(nameof(PercentualPago));
                RequestClose?.Invoke(this, EventArgs.Empty);
                _messenger.Send(new ProdutoAdicionadoMessage(AgendamentoId, produto));
            }
            else
                MessageBox.Show("Produto não encontrado.");



        }
        partial void OnProdutoSelecionadoChanged(ProdutoDto? p)
        {
            if (p is null)
            {
                NovoProduto = new AgendamentoProdutoCreateDto();
                return;
            }

            NovoProduto = new AgendamentoProdutoCreateDto
            {
                ProdutoId = p.Id,
                Quantidade = NovoProduto?.Quantidade > 0 ? NovoProduto.Quantidade : 1,
                ValorUnitario = (!EstaEditando || NovoProduto?.ValorUnitario <= 0) ? p.Valor : NovoProduto!.ValorUnitario
            };
        }
    }
}
