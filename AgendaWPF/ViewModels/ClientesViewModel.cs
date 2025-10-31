using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly IClienteService _clienteService;
        [ObservableProperty]
        private ObservableCollection<ClienteDto> clientes = new();
        [ObservableProperty] private CriancaDto? criancaSelecionada;
        [ObservableProperty] private ClienteDto? clienteSelecionado;
        [ObservableProperty] private ClienteCreateDto novoCliente = new();
        [ObservableProperty] private CriancaCreateDto novaCrianca = new();
        [ObservableProperty] private ObservableCollection<AgendamentoDto> historico = new();
        public bool TemHistorico => Historico?.Any() == true;

        [ObservableProperty] private int paginaAtual = 1;

        [ObservableProperty] private int totalPaginas = 1;

        [ObservableProperty] private string? filtroNome;
        //Enums
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;

        public IAsyncRelayCommand CarregarClientesCommand { get; }
        public IRelayCommand ProximaPaginaCommand { get; }
        public IRelayCommand PaginaAnteriorCommand { get; }

        private const int PageSize = 10;
        public ClientesViewModel(IClienteService clienteService)
        {
            _clienteService = clienteService;
            CarregarClientesCommand = new AsyncRelayCommand(CarregarClientesAsync);
            ProximaPaginaCommand = new RelayCommand(() => MudarPagina(1), () => PaginaAtual < TotalPaginas);
            PaginaAnteriorCommand = new RelayCommand(() => MudarPagina(-1), () => PaginaAtual > 1);
            _ = CarregarClientesAsync();
        }
        private async Task CarregarClientesAsync()
        {
            await Task.Delay(3500);
            var resultado = await _clienteService.GetClientesPaginadoAsync(PaginaAtual, PageSize, FiltroNome);

            Clientes = new ObservableCollection<ClienteDto>(resultado.Items);
            TotalPaginas = resultado.TotalPages;
        }


        [RelayCommand]
        public async Task AdicionarNovoClienteAsync()
        {
            NovoCliente.Crianca = string.IsNullOrWhiteSpace(NovaCrianca?.Nome)
            ? null
            : NovaCrianca;
            var clienteCriado = await _clienteService.CreateClienteAsync(NovoCliente);

            await CarregarClientesAsync();
            NovoCliente = new ClienteCreateDto();
            NovaCrianca = new CriancaCreateDto();
        }

        private async void MudarPagina(int delta)
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
