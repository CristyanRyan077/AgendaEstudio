using AgendaApi.Models;
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class AutoCompleteViewModel : ObservableObject
    {
        [ObservableProperty] private bool mostrarSugestoes = false;
        [ObservableProperty] private bool mostrarSugestoesServico = false;
        [ObservableProperty] private string nomeDigitado = string.Empty;
        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();
        [ObservableProperty] private ClienteDto? clienteSelecionado;
        [ObservableProperty] private ObservableCollection<ClienteDto> clientesFiltrados = new();

        [ObservableProperty] private string telefone;
        public string NomeClienteSelecionado => ClienteSelecionado?.Nome ?? string.Empty;
        public AutoCompleteViewModel()
        {

        }
        partial void OnNomeDigitadoChanged(string value)
        {

            var termo = value?.ToLower() ?? "";
            int idProcurado;
            bool buscaPorId = int.TryParse(termo, out idProcurado);
            var filtrados = ListaClientes
             .Where(c =>
             (!string.IsNullOrEmpty(c.Nome) && c.Nome.ToLower().Contains(termo)) ||
             (buscaPorId && c.Id == idProcurado))
             .ToList();

            ClientesFiltrados = new ObservableCollection<ClienteDto>(filtrados);

            MostrarSugestoes = ClientesFiltrados.Any();
        }
        public void CarregarClientes(IEnumerable<ClienteDto> clientes)
        {
            ListaClientes.Clear();
            foreach (var c in clientes)
                ListaClientes.Add(c);
        }
        partial void OnClienteSelecionadoChanged(ClienteDto? value)
        {
            if (value != null)
            {
                NomeDigitado = value.NomeComId;
                Telefone = value.Telefone;
            }
            else
            {
                Telefone = string.Empty;
            }
        }
    }

}
