
using AgendaShared;
using AgendaShared.DTOs;
using AgendaShared.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AgendaWPF.ViewModels
{
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly IClienteService _clienteService;
        [ObservableProperty]
        private ObservableCollection<ClienteDto> clientes = new();
        [ObservableProperty] private CriancaDto? criancaSelecionada;
        [ObservableProperty] private ClienteDto? clienteSelecionado;
        [ObservableProperty] private ObservableCollection<AgendamentoDto> historico = new();
        [ObservableProperty] private FormMode mode = FormMode.Create;
        [ObservableProperty] private ClienteFormModel cliente = new();
        [ObservableProperty] private int? clienteId;
        [ObservableProperty] private int? criancaId;
        [ObservableProperty] private ObservableCollection<CriancaDto> listaCriancas = new();
        [ObservableProperty] private bool clienteExistenteDetectado;
        public bool IsEdit => Mode == FormMode.Edit;
        public bool TemHistorico => Historico?.Any() == true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProximaPaginaCommand))]
        [NotifyCanExecuteChangedFor(nameof(PaginaAnteriorCommand))]
        private int paginaAtual = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProximaPaginaCommand))]
        [NotifyCanExecuteChangedFor(nameof(PaginaAnteriorCommand))]
        private int totalPaginas = 1;

        [ObservableProperty] private string? pesquisaText;


        //Enums
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;
        private CancellationTokenSource? _detectCts;
        public IAsyncRelayCommand CarregarClientesCommand { get; }
        public IAsyncRelayCommand ProximaPaginaCommand { get; }
        public IAsyncRelayCommand PaginaAnteriorCommand { get; }

        [ObservableProperty] private int pageSize = 10;

        [ObservableProperty] private List<int> opcoesTamanhoPagina = new() { 10, 20, 50 };
        public ClientesViewModel(IClienteService clienteService)
        {
            _clienteService = clienteService;
            CarregarClientesCommand = new AsyncRelayCommand(CarregarClientesAsync);
            ProximaPaginaCommand = new AsyncRelayCommand(
              () => MudarPaginaAsync(1),
              () => PaginaAtual < TotalPaginas
            );

            PaginaAnteriorCommand = new AsyncRelayCommand(
                () => MudarPaginaAsync(-1),
                () => PaginaAtual > 1
            );

        }
        public async Task InitAsync()
        {
            await CarregarClientesAsync();
            await _clienteService.PrecarregarCacheClientesAsync();
            New();
        }
        private async Task CarregarClientesAsync()
        {
                
            var resultado = await _clienteService.GetClientesPaginadoAsync(PaginaAtual, PageSize, PesquisaText);

            Clientes.Clear();
            foreach (var item in resultado.Items)
            {
                Clientes.Add(item);
            }
            Debug.WriteLine($"clientes encontrados {resultado.Items.Count()}");
            TotalPaginas = resultado.TotalPages;
        }
        public void New()
        {
            Mode = FormMode.Create;
            ClienteId = null;
            Cliente = new ClienteFormModel
            {
                Crianca = new CriancaFormModel() 
            };
        }
        partial void OnPageSizeChanged(int value)
        {
            _ = CarregarClientesAsync();
        }
        private CancellationTokenSource _debounceCts = new();
        partial void OnPesquisaTextChanged(string? value)
        {
            _debounceCts.Cancel();
            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    // 3. Espera pelo tempo de debounce
                    await Task.Delay(300, token);

                    // 4. Verifica se o token foi cancelado (se o usuário digitou mais rápido)
                    if (token.IsCancellationRequested) return;

                    await Application.Current.Dispatcher.Invoke(async () => await CarregarClientesAsync());
                    PaginaAtual = 1;

                }
                catch (TaskCanceledException)
                {
                    // Exceção esperada e ignorada quando o cancelamento ocorre
                }
                catch (Exception ex)
                {
                    // Tratar outros erros
                    Debug.WriteLine($"Erro no debounce: {ex.Message}");
                }
            });
        }

        [RelayCommand]
        public async Task SalvarAsync()
        {
            if (Mode == FormMode.Create && ClienteExistenteDetectado == false)
            {
                var create = Cliente.ToCreateDto();             
                var clienteCriado = await _clienteService.CreateClienteAsync(create);

            }
            else if (Mode == FormMode.Create && ClienteExistenteDetectado == true)
            {
                if (ClienteId is null || ClienteId.Value <= 0) return;

                // validação rápida
                if (Cliente.Crianca is null || string.IsNullOrWhiteSpace(Cliente.Crianca.Nome))
                    return; // opcional: avisar usuário

                var dtoCrianca = Cliente.Crianca.ToCreateDto();
                var criada = await _clienteService.CreateCriancaAsync(ClienteId.Value, dtoCrianca);

                // Se quiser, recarregue a lista de crianças do painel lateral:
                ListaCriancas.Clear();
                var criancas = await _clienteService.GetByClienteIdAsync(ClienteId.Value);
                foreach (var c in criancas)
                    ListaCriancas.Add(c);
            }
            else
            {
                if (ClienteId is null) return;
                var update = Cliente.ToUpdateDto(ClienteId.Value); // <- mapper do client
                await _clienteService.UpdateClienteAsync(ClienteSelecionado.Id, update);
            }
            await InitAsync();
            ClienteExistenteDetectado = false;
        }
        public async Task DetectarClientePorCamposAsync(CancellationToken ct)
        {
            if (IsEdit)
                return;

            var tel = Cliente.Telefone?.Trim();
            var email = Cliente.Email?.Trim();
            if (string.IsNullOrEmpty(tel) && string.IsNullOrEmpty(email))
            {
                ClienteExistenteDetectado = false;
                ClienteId = 0;
                ListaCriancas.Clear();
                return;
            }


            var encontrado = _clienteService.DetectExistingLocal(tel, email);
            if (encontrado != null)
            {
                // preenche campos
                ClienteId = encontrado.Id;
                Cliente.Nome = encontrado.Nome;
                Cliente.Telefone = encontrado.Telefone;
                Cliente.Email = encontrado.Email;


                ListaCriancas.Clear();
                var criancas = await _clienteService.GetByClienteIdAsync(encontrado.Id);
                foreach (var c in criancas)
                    ListaCriancas.Add(c);

                ClienteExistenteDetectado = true;
            }
            else
            {
                ClienteExistenteDetectado = false;
            }
            Debug.WriteLine($"IsEdit={IsEdit}");
            Debug.WriteLine($"Telefone='{Cliente.Telefone}', Email='{Cliente.Email}'");
            Debug.WriteLine($"Encontrado? {(encontrado is not null)}");
        }
        [RelayCommand]
        private void EditarClienteSelecionado()
        {

            if (ClienteSelecionado is null)
                return;
            Mode = FormMode.Edit;

            var cliente = Clientes.FirstOrDefault(c => c.Id == ClienteSelecionado.Id);
            if (cliente is null)
                return;

            ClienteId = cliente.Id;
            Cliente.Nome = cliente.Nome;
            Cliente.Telefone = cliente.Telefone;
            Cliente.Email = cliente.Email;
            Cliente.Observacao = cliente.Observacao;
        }
        [RelayCommand]
        public async Task DeleteAsync()
        {
            if (ClienteSelecionado == null) return;

            var criList = await _clienteService.GetByClienteIdAsync(ClienteSelecionado.Id);
            if (criList.Count() >= 1)
            {
                Debug.WriteLine($"criancas encontradas {criList.Count()}");
                var cri = criList[0];
                if (cri is null && (ClienteSelecionado.Criancas?.Count ?? 0) == 1)
                    cri = ClienteSelecionado.Criancas![0];

                if (cri != null)
                {

                    if (System.Windows.MessageBox.Show($"Excluir crianca: {cri.Nome}?",
                        "Confirmar", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

                    await _clienteService.DeleteCriancaAsync(cri.Id);
                    return;
                }
            }
            
            if (System.Windows.MessageBox.Show($"Excluir cliente: {ClienteSelecionado.Nome}?",
                    "Confirmar", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            await _clienteService.DeleteAsync(ClienteSelecionado.Id);

            Clientes.Remove(ClienteSelecionado);
            ClienteSelecionado = null;
            ListaCriancas.Clear();
            foreach (var c in ListaCriancas)
                ListaCriancas.Add(c);
        }
        private async Task DetectarClientePorCamposDebouncedAsync()
        {
            _detectCts?.Cancel();
            _detectCts = new CancellationTokenSource();
            var ct = _detectCts.Token;

            try
            {
                await Task.Delay(350, ct); // debounce

                if (ct.IsCancellationRequested) return;
                await DetectarClientePorCamposAsync(ct);
            }
            catch (TaskCanceledException) { /* ignorar */ }
        }
        partial void OnClienteChanged(ClienteFormModel value)
        {
            if (value is null) return;
            value.PropertyChanged -= ClienteOnPropertyChanged;
            value.PropertyChanged += ClienteOnPropertyChanged;
        }
        private async void ClienteOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(ClienteFormModel.Telefone) or nameof(ClienteFormModel.Email))
                await DetectarClientePorCamposDebouncedAsync();
        }

        private async Task MudarPaginaAsync(int delta)
        {
            PaginaAtual += delta;
            await CarregarClientesAsync();
        }
        private CancellationTokenSource? _ctsHistorico;
        partial void OnClienteSelecionadoChanged(ClienteDto? value)
        {
            _ctsHistorico?.Cancel();
            Historico.Clear();
            if (value is null || value.Id <= 0)
                return;

            _ctsHistorico = new CancellationTokenSource();
            _ = CarregarHistoricoAsync(value.Id, _ctsHistorico.Token);

        }
        private async Task CarregarHistoricoAsync(int clienteId, CancellationToken ct)
        {
            try
            {

                var agendamentos = await _clienteService.GetHistoricoAsync(clienteId);

                Historico.Clear();
                foreach (var a in agendamentos
                    .OrderByDescending(a => a.Data)
                    .ThenByDescending(a => a.Horario))
                {
                    Historico.Add(a);
                }
                OnPropertyChanged(nameof(TemHistorico));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

    }
}
