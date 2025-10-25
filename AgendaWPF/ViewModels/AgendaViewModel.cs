
using AgendaShared.DTOs;
using AgendaWPF.Controles;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class AgendaViewModel : ObservableObject
    {
        public ObservableCollection<DiaAgendamento> DiasSemana { get; set; } = new();
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;

        [ObservableProperty] private bool mostrarSugestoes = false;
        [ObservableProperty] private bool mostrarSugestoesServico = false;
        [ObservableProperty] private ClienteDto? clienteSelecionado;
        [ObservableProperty] private string nomeDigitado = string.Empty;
        [ObservableProperty] private ClienteDto novoCliente = new();
        [ObservableProperty] private ObservableCollection<ClienteDto> clientesFiltrados = new();
        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();
        public AgendaViewModel(IAgendamentoService agendamentoService, IClienteService clienteService)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            
        }
        public async Task InicializarAsync()
        {
            await Task.Delay(1500);
            await CarregarSemanaAtualAsync();
            await CarregarClientesAsync();
        }
        public async Task CarregarClientesAsync()
        {
            var lista = await _clienteService.ObterClientesAsync() ?? new List<ClienteDto>();

            ListaClientes.Clear();
            foreach (var c in lista)
                ListaClientes.Add(c);
        }
        public async Task CarregarSemanaAtualAsync()
        {
            var inicio = ObterSegundaDaSemana(DateTime.Today);
            var fim = inicio.AddDays(6);

            var agendamentos = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            // Monta lista agrupada por dia:
            DiasSemana.Clear();

            for (int i = 0; i < 7; i++)
            {
                var dia = inicio.AddDays(i);
                var listaDia = agendamentos
                    .Where(a => a.Data.Date == dia.Date)
                    .OrderBy(a => a.Horario)
                    .ToList();

                DiasSemana.Add(new DiaAgendamento
                {
                    Nome = CultureInfo.GetCultureInfo("pt-BR")
                    .TextInfo
                    .ToTitleCase(
                        dia.ToString("dddd", new CultureInfo("pt-BR"))
                           .Replace("-feira", "")
                           .Trim()
                    ),
                    Agendamentos = new ObservableCollection<AgendamentoDto>(listaDia)
                });
            }
        }
        private static DateTime ObterSegundaDaSemana(DateTime data)
        {
            int diff = (7 + (data.DayOfWeek - DayOfWeek.Monday)) % 7;
            return data.AddDays(-1 * diff).Date;
        }

        // ----------- Metodos De Auto Complete -------------- //
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
        partial void OnClienteSelecionadoChanged(ClienteDto? cliente)
        {
            if (cliente == null) return;

            NomeDigitado = cliente.Nome;

            NovoCliente.Id = cliente.Id;
            NovoCliente.Nome = cliente.Nome;
            NovoCliente.Telefone = cliente.Telefone;
            NovoCliente.Email = cliente.Email;
            NovoCliente.Observacao = cliente.Observacao;

            OnPropertyChanged(nameof(NovoCliente));
        }
    }
}
