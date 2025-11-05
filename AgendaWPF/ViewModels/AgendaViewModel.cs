using AgendaShared;
using AgendaShared.DTOs;
using AgendaWPF.Controles;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AgendaWPF.ViewModels.FormAgendamentoVM;

namespace AgendaWPF.ViewModels
{
    public partial class AgendaViewModel : ObservableObject
    {
        [ObservableProperty] private PagamentosViewModel? pagamentosVM;
        public ObservableCollection<DiaAgendamento> DiasSemana { get; set; } = new();
        private readonly IAcoesService _acoes;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IMessenger _messenger;
        private readonly ISemanaService _semanaService;
        [ObservableProperty] private AgendamentoDto agendamentoSelecionado;
        [ObservableProperty] private bool mostrarDetalhes;
        [ObservableProperty] private object? pagamentosView;
        [ObservableProperty] private bool mostrarPagamentos;
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;

        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();


        public AgendaViewModel(IMessenger messenger, IAgendamentoService agendamentoService, IClienteService clienteService, ISemanaService semanaService, IAcoesService acoes)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _messenger = messenger;
            _semanaService = semanaService;
            _acoes = acoes;

            messenger.Register<AgendamentoCriadoMessage>(this, (_, msg) =>
            {
                AdicionarAgendamentoNaSemana(msg.Agendamento);
            });
            messenger.Register<PagamentoCriadoMessage>(this, (r, m) =>
            {
                _ = CarregarSemanaAtualAsync();
                OnPropertyChanged(nameof(AgendamentoSelecionado));

            });

        }
        public async Task InicializarAsync()
        {
            DataSelecionada = DateTime.Today;
            if (DiasSemana.Count == 0)
                for (int i = 0; i < 7; i++) DiasSemana.Add(new DiaAgendamento());
            await CarregarSemanaAtualAsync();
        }

        public async Task CarregarSemanaAtualAsync()
        {
            PreencherEsqueletoDaSemana(DataSelecionada);
            var inicio = _semanaService.ObterSegunda(DataSelecionada);
            var fim = inicio.AddDays(6);
            var ags = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            foreach (var dia in DiasSemana)
            {
                dia.Agendamentos.Clear();
                foreach (var ag in ags.Where(a => a.Data.Date == dia.Data.Date)
                                      .OrderBy(a => a.Horario))
                    dia.Agendamentos.Add(ag);
            }
        }
        private void PreencherEsqueletoDaSemana(DateTime referencia)
        {
            var inicio = _semanaService.ObterSegunda(referencia);
            // Crie os 7 itens uma vez (opção 2) e atualize in-place:
            for (int i = 0; i < 7; i++)
            {
                var d = inicio.AddDays(i);
                DiasSemana[i].Data = d;   // dispara Nome/DataFormatada no DiaAgendamento
                DiasSemana[i].Agendamentos.Clear(); // temporariamente vazios
            }
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
        [RelayCommand]
        private void SemanaAnterior() => DataSelecionada = DataSelecionada.AddDays(-7);

        [RelayCommand]
        private void ProximaSemana() => DataSelecionada = DataSelecionada.AddDays(7);

        public string SemanaExibida => $"{InicioDaSemana:dd/MM} - {FimDaSemana:dd/MM}";
        public void IrParaSemana(DateTime data)
        {
            var inicio = InicioDaSemanaPicker(data); // dom ou seg, conforme sua regra
            if (inicio.Date == DataSelecionada.Date) return;

            DataSelecionada = inicio;
        }
        private static DateTime InicioDaSemanaPicker(DateTime d)
        {
            int delta = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            if (delta < 0) delta += 7;
            return d.Date.AddDays(-delta);

        }
        private DateTime InicioDaSemana
        {
            get
            {
                int diasDesdeSegunda = ((int)DataSelecionada.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                return DataSelecionada.AddDays(-diasDesdeSegunda);
            }
        }
        private DateTime FimDaSemana => InicioDaSemana.AddDays(6);

        private DateTime _dataSelecionada = DateTime.Today;
        public DateTime DataSelecionada
        {
            get => _dataSelecionada;
            set
            {
                if (_dataSelecionada != value)
                {
                    _dataSelecionada = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SemanaExibida));
                    _ = CarregarSemanaAtualAsync();
                }
            }
        }
        partial void OnDataFiltroSelecionadaChanged(DateTime? value)
        {
            if (value is null) return;
            IrParaSemana(value.Value);
        }
                [ObservableProperty] private DateTime? dataFiltroSelecionada;
        [RelayCommand]
        private void AbrirDetalhes(AgendamentoDto ag)
        {
            AgendamentoSelecionado = ag;
            Debug.WriteLine($"[ABRIR DETALHES] Agendamento Selecionado = {AgendamentoSelecionado.Id}");
            MostrarDetalhes = true;
        }
        [RelayCommand]
        private void FecharDetalhes()
        {
            MostrarDetalhes = false;
            AgendamentoSelecionado = null;
        }
        [RelayCommand]
        public async Task AbrirPagamentosAsync(AgendamentoDto ag)
        {
            Debug.WriteLine("Abrir pagamentos chamado");
            if (ag is null) return;



            PagamentosVM = await _acoes.CriarPagamentosViewModelAsync(ag.Id);
            MostrarPagamentos = true;

           
           // OnPropertyChanged(nameof(TemHistorico));
           
            //var view = new HistoricoUsuario { DataContext = this };
            //TelaHistoricoCliente = view;
            //MostrarHistoricoCliente = true;
        }
        [RelayCommand]
        public void FecharPagamentos()
        {
            MostrarPagamentos = false;
            PagamentosView = null;
        }
    }

}
