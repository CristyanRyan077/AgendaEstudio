using AgendaMobile.Helpers;
using AgendaMobile.Models;
using AgendaMobile.Services;
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using CommunityToolkit.Mvvm.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.ViewModels
{

    public partial class CalendarioViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty] private bool estaCarregando;
        [ObservableProperty] private string erroMensagem;
        [ObservableProperty] private DateTime dataReferencia;
        [ObservableProperty] private ObservableCollection<AgendamentoPorDia> dias;
        [ObservableProperty]
        private ObservableCollection<AgendamentoViewModel> agendamentosSelecionados = new();
        private List<AgendamentoDto> _agendamentosDoMes = new();
        private List<ClienteResumoDto> _clientes = new();
        public ObservableCollection<AgendamentoViewModel> Agendamentos { get; } = new();
        public ObservableCollection<ClienteViewModel> Clientes { get; } = new();
        public CalendarioViewModel(IApiService api)
        {
            _apiService = api;
            Dias = new ObservableCollection<AgendamentoPorDia>();
            DataReferencia = DateTime.Today;
            _ = CarregarCalendarioCompletoAsync();

        }
        [RelayCommand]
        private void MesAnterior() => DataReferencia = DataReferencia.AddMonths(-1);

        [RelayCommand]
        private void ProximoMes() => DataReferencia = DataReferencia.AddMonths(1);
        [RelayCommand]
        private async Task SelecionarDiaAsync(AgendamentoPorDia dia)
        {
            if (dia == null) return;

            // Filtra agendamentos da lista já carregada do mês
            var agsDoDia = _agendamentosDoMes
                .Where(a => a.Data.Date == dia.Dia.Date)
                .Select(a =>
                {
                    // Preenche ClienteResumoDto
                    var cliente = _clientes.FirstOrDefault(c => c.Id == a.ClienteId);
                    if (cliente != null) a.Cliente = cliente;
                    return new AgendamentoViewModel(a);
                })
                .ToList();

            AgendamentosSelecionados.Clear();
            foreach (var ag in agsDoDia)
                AgendamentosSelecionados.Add(ag);
        }

        [RelayCommand]
        public async Task CarregarDadosAsync()
        {
        try
        {
            EstaCarregando = true;
            ErroMensagem = string.Empty;

            var agendamentosDto = await _apiService.ObterAgendamentosAsync();
            var clientes = await _apiService.ObterClientesAsync();

            Agendamentos.Clear();
            foreach (var ag in agendamentosDto)
            {
                var clienteCompleto = clientes.FirstOrDefault(c => c.Id == ag.ClienteId);
                    if (clienteCompleto != null)
                    {
                        ag.Cliente = new ClienteResumoDto
                        {
                            Id = clienteCompleto.Id,
                            Nome = clienteCompleto.Nome,
                            Telefone = clienteCompleto.Telefone
                        };
                    }
                 Agendamentos.Add(new AgendamentoViewModel(ag));
            }

            }
            catch (Exception ex)
            {
                ErroMensagem = ex.Message;
            }
            finally             {
                EstaCarregando = false;
            }
        }
        private bool _clientesCarregados;
        private async Task CarregarCalendarioCompletoAsync()
        {
            try
            {
                var sw = Stopwatch.StartNew();


                EstaCarregando = true;
                ErroMensagem = string.Empty;

                // 1️⃣ Buscar clientes e agendamentos do mês
                var inicioMes = new DateTime(DataReferencia.Year, DataReferencia.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

                if (!_clientesCarregados)
                {
                    var clientesDto = await _apiService.ObterClientesAsync();
                    _clientes = clientesDto.Select(c => new ClienteResumoDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Telefone = c.Telefone
                    }).ToList();
                    _clientesCarregados = true;
                }
                var agendamentos = await _apiService.ObterAgendamentosPorPeriodo(inicioMes, fimMes);
                _agendamentosDoMes = agendamentos.ToList();

                // 2️⃣ Criar os 42 dias do calendário
                var primeiroDiaMes = new DateTime(DataReferencia.Year, DataReferencia.Month, 1);
                int diaSemana = ((int)primeiroDiaMes.DayOfWeek + 6) % 7; // Segunda como início
                var inicioGrid = primeiroDiaMes.AddDays(-diaSemana);

                var diasTemp = Enumerable.Range(0, 42)
                    .Select(i =>
                    {
                        var data = inicioGrid.AddDays(i);
                        var agsDoDia = _agendamentosDoMes.Where(a => a.Data.Date == data.Date);
                        return new AgendamentoPorDia
                        {
                            Dia = data,
                            MesReferencia = DataReferencia,
                            Agendamentos = new ObservableCollection<AgendamentoDto>(agsDoDia)
                        };
                    })
                    .ToList();

                // 3️⃣ Atualizar ObservableCollection de uma vez
                foreach (var dia in diasTemp)
                    dia.SelecionarDiaCommand = new AsyncRelayCommand<AgendamentoPorDia>(SelecionarDiaAsync);
                sw.Stop();
                Debug.WriteLine($"[MAUI] Tempo de montagem da tela: {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                ErroMensagem = ex.Message;
            }
            finally
            {
                EstaCarregando = false;

            }
        }
        partial void OnDataReferenciaChanged(DateTime value)
        {
            // Esse método é chamado automaticamente quando DataReferencia muda
            _ = CarregarCalendarioCompletoAsync();
        }

    }
}
