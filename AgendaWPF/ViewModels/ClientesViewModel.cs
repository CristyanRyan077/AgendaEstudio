using AgendaApi.Models;
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

        [ObservableProperty] private int paginaAtual = 1;

        [ObservableProperty] private int totalPaginas = 1;

        [ObservableProperty] private string? filtroNome;

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
            await Task.Delay(1500);
            var resultado = await _clienteService.GetClientesPaginadoAsync(PaginaAtual, PageSize, FiltroNome);

            Clientes = new ObservableCollection<ClienteDto>(resultado.Items);
            TotalPaginas = resultado.TotalPages;
        }

        private async void MudarPagina(int delta)
        {
            PaginaAtual += delta;
            await CarregarClientesAsync();
        }
    }
}
