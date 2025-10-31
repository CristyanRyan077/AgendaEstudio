
using AgendaShared;
using AgendaShared.DTOs;
using AgendaWPF.Controles;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgendaWPF.ViewModels.FormAgendamentoVM;

namespace AgendaWPF.ViewModels
{
    public partial class AgendaViewModel : ObservableObject
    {
        public ObservableCollection<DiaAgendamento> DiasSemana { get; set; } = new();
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IMessenger _messenger;
        private readonly ISemanaService _semanaService;
        [ObservableProperty] private DateTime dataSelecionada = DateTime.Today;
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;

        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();
        public AgendaViewModel( IMessenger messenger, IAgendamentoService agendamentoService, IClienteService clienteService, ISemanaService semanaService)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _messenger = messenger;
            _semanaService = semanaService;

            messenger.Register<AgendamentoCriadoMessage>(this, (_, msg) =>
            {
                AdicionarAgendamentoNaSemana(msg.Agendamento);
            });

        }
        public async Task InicializarAsync()
        {
            await Task.Delay(500);
            await CarregarSemanaAtualAsync();
        }

        public async Task CarregarSemanaAtualAsync()
        {
            var inicio = _semanaService.ObterSegunda(DataSelecionada);
            var fim = inicio.AddDays(6);

            var agendamentos = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            // Monta lista agrupada por dia:
            DiasSemana.Clear();
            foreach (var dia in await _semanaService.CarregarSemanaAsync(DataSelecionada))
                DiasSemana.Add(dia);



        }
        private void AdicionarAgendamentoNaSemana(AgendamentoDto criado)
        {
            var inicio = _semanaService.ObterSegunda(DataSelecionada);
            var fim = inicio.AddDays(6);
            if (criado.Data.Date < inicio || criado.Data.Date > fim) return;

            var dia = DiasSemana.FirstOrDefault(d => d.Data.Date == criado.Data.Date);
            if (dia is null)
            {
                dia = _semanaService.CriarDia(criado.Data);
                DiasSemana.Add(dia);
            }

            // Inserção ordenada por horário
            var i = 0;
            while (i < dia.Agendamentos.Count && dia.Agendamentos[i].Horario <= criado.Horario) i++;
            dia.Agendamentos.Insert(i, criado);
        }
    }

}
