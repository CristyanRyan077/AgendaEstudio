
using AgendaShared.DTOs;
using AgendaWPF.Converters;
using AgendaWPF.Models;
using AgendaWPF.Services;
using AgendaWPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AgendaWPF.ViewModels.FormAgendamentoVM;

namespace AgendaWPF.ViewModels
{
    public partial class CalendarioViewModel : ObservableObject
    {
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IPagamentoService _pagamentoservice;
        private readonly ILembreteService _lembreteService;
        private readonly IMessenger _messenger;
        private readonly IAcoesService _acoes;
        private CancellationTokenSource? _loadCts;
        [ObservableProperty] private AgendamentoDto? agendamentoSelecionado;
        [ObservableProperty] private DateTime dataSelecionada = DateTime.Today;
        [ObservableProperty] private DateTime mesAtual = DateTime.Today;
        [ObservableProperty] private int agendamentoId;
        [ObservableProperty] private ObservableCollection<HistoricoFinanceiroDto> historico = new();
        [ObservableProperty] private PagamentosViewModel? pagamentosVM;
        [ObservableProperty] private object? pagamentosView;
        [ObservableProperty] private bool mostrarPagamentos;
        public IRelayCommand<SetEtapaParam> AbrirEtapaDialogCommand { get; }
        public IAsyncRelayCommand<(AgendamentoDto ag, DateTime novaData)> MoverAgendamentoAsyncCommand { get; }
        [ObservableProperty] private bool mostrarDetalhes;
        [ObservableProperty] ObservableCollection<AgendamentoDto> listaAgendamentos = new();
        public ObservableCollection<string> DiasSemana { get; set; } = new();
        public CalendarioViewModel(
            IPagamentoService pagamentoservice,
            IAgendamentoService agendamentoService,
            IMessenger messenger,
            IAcoesService acoes,
            IClienteService clienteService,
            ILembreteService lembreteService)
        {
            _agendamentoService = agendamentoService;
            _pagamentoservice = pagamentoservice;
            _messenger = messenger;
            _acoes = acoes;
            _clienteService = clienteService;
            MesAtual = DateTime.Today;
            MoverAgendamentoAsyncCommand = new AsyncRelayCommand<(AgendamentoDto ag, DateTime novaData)>(ReagendarAsync);

            messenger.Register<AgendamentoCriadoMessage>(this, (_, msg) =>
            {
                _ = ReloadMonthAsync();
            });
            messenger.Register<PagamentoCriadoMessage>(this, async (r, m) =>
            {
                var agendamentoAtualizado = await _agendamentoService.GetByIdAsync(m.AgendamentoId);
                if (agendamentoAtualizado != null)
                {
                    AgendamentoSelecionado = agendamentoAtualizado;
                    await ReloadMonthAsync();
                }
            });
            messenger.Register<ProdutoAdicionadoMessage>(this, async (r, m) =>
            {
                var agendamentoAtualizado = await _agendamentoService.GetByIdAsync(m.AgendamentoId);
                if (agendamentoAtualizado != null)
                {

                    AgendamentoSelecionado = agendamentoAtualizado;
                    await ReloadMonthAsync();
                }
            });
            _lembreteService = lembreteService;
        }
        private bool HorarioOcupado(DateTime dia, TimeSpan? horario, int agendamentoIdIgnorar = 0)
        {
            var vmDia = DiasDoMes.FirstOrDefault(d => d.Data.Date == dia.Date);
            if (vmDia is null) return false;
            return vmDia.Agendamentos.Any(a => a.Horario == horario && a.Id != agendamentoIdIgnorar);
        }

        public async Task InicializarAsync()
        {
            CarregarDias(MesAtual);
            await ReloadMonthAsync();
        }
        public async Task ReloadMonthAsync()
        {
            _loadCts?.Cancel();
            _loadCts = new CancellationTokenSource();
            var ct = _loadCts.Token;

            CarregarDias(MesAtual);                 
            await PreencherAgendamentosAsync(ct);
            await PreencherLembretesAsync(ct);
        }
        private async Task ReagendarAsync((AgendamentoDto ag, DateTime novaData) p)
        {
            var (ag, novaData) = p;
            if (ag is null) return;
            var horarioEscolhido = ag.Horario;
            if (HorarioOcupado(novaData, ag.Horario, ag.Id))
            {
                // pega sugestões
                var sugestoes = SugerirProximosHorariosLivres(novaData, ag.Horario, 6);

                if (sugestoes.Count == 0)
                {
                    MessageBox.Show(
                        $"O dia {novaData:dd/MM} está lotado nos horários fixos.\n" +
                        $"Tente outro dia ou ajuste manualmente.",
                        "Conflito de Horário", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var picker = new SelecionarHorarioWindow(sugestoes, ag.Horario);
                var ok = picker.ShowDialog() == true;
                if (!ok) return; // cancelado

                horarioEscolhido = picker.HorarioSelecionado;
            }
            var snapshotData = ag.Data;
            var snapshotHora = ag.Horario;

            ag.Horario = horarioEscolhido;
            MoverAgendamentoInMemory(ag, novaData);
            try
            {
                // 2) Persiste no banco
                var dto = new ReagendarDto { NovaData = novaData.Date, NovoHorario = horarioEscolhido };
                await _agendamentoService.ReagendarAsync(ag.Id, dto);
                WeakReferenceMessenger.Default.Send(new AgendamentoReagendadoMessage(
                    agendamentoId: ag.Id,
                    velhaData: snapshotData,
                    velhoHorario: snapshotHora,
                    novaData: novaData.Date,
                    novoHorario: horarioEscolhido
                ));
            }
            catch (Exception ex)
            {
                ag.Horario = snapshotHora;
                MoverAgendamentoInMemory(ag, snapshotData);
                Debug.WriteLine($"Erro ao reagendar: {ex}");
                MessageBox.Show("Não foi possível reagendar. Tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (snapshotData.Month != novaData.Month || snapshotData.Year != novaData.Year)
                await ReloadMonthAsync();
        }
        
        private List<TimeSpan> SugerirProximosHorariosLivres(DateTime dia, TimeSpan? aPartir, int max = 4)
        {
            var livres = HorariosLivres(dia).OrderBy(h => h).ToList();
            var depois = livres.Where(h => h >= aPartir).ToList();
            var antes = livres.Where(h => h < aPartir).ToList();
            var result = new List<TimeSpan>();
            result.AddRange(depois);
            result.AddRange(antes);
            return result.Take(max).ToList();
        }
        private List<TimeSpan> HorariosLivres(DateTime dia)
        {
            var vmDia = DiasDoMes.FirstOrDefault(d => d.Data.Date == dia.Date);

            if (vmDia is null) return _horariosFixos.ToList();

            var ocupados = vmDia.Agendamentos
            .Select(a => a.Horario)
            .ToHashSet();

            var livres = _horariosFixos
            .Where(h => !ocupados.Contains(h))
            .ToList();

            return livres;
        }
        private static (DateTime inicio, DateTime fim) GetMonthRange(DateTime mesRef)
        {
            var inicio = new DateTime(mesRef.Year, mesRef.Month, 1);
            var fim = inicio.AddMonths(1);
            return (inicio, fim);
        }
        private async Task PreencherAgendamentosAsync(CancellationToken ct)
        {
            var (inicio, fim) = GetMonthRange(MesAtual);
  

            // 1) pega todos do mês (fim exclusivo)
            var agendamentos = await _agendamentoService
                .ObterAgendamentosPorPeriodo(inicio, fim);

            // 2) agrupa por dia p/ O(1) na hora de preencher
            var porDia = agendamentos
                .GroupBy(a => a.Data.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3) preenche a grade já montada
            foreach (var dia in DiasDoMes)
            {
                ct.ThrowIfCancellationRequested();

                dia.Agendamentos.Clear();
                if (porDia.TryGetValue(dia.Data.Date, out var doDia))
                    foreach (var ag in doDia) dia.Agendamentos.Add(ag);
            }
        }
        private async Task PreencherLembretesAsync(CancellationToken ct)
        {
            var (inicio, fim) = GetMonthRange(MesAtual);

            // 1) Busca todos os lembretes do mês na API
            var filtro = new LembreteQuery
            {
                Inicio = inicio,
                Fim = fim
            };

            var lembretes = await _lembreteService.ListAsync(filtro, ct);

            // 2) Agrupa por dia
            var porDia = lembretes
                .GroupBy(l => l.DataAlvo.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3) Preenche na sua grade de dias
            foreach (var dia in DiasDoMes)
            {
                ct.ThrowIfCancellationRequested();

                dia.Lembretes.Clear();

                if (porDia.TryGetValue(dia.Data.Date, out var doDia))
                {
                    foreach (var l in doDia)
                        dia.Lembretes.Add(new LembretesVM(l));
                }
            }
        }
        [RelayCommand]
        private void AvancarMes()
        {
            MesAtual = MesAtual.AddMonths(1);
            CarregarDias(MesAtual);
        }

        [RelayCommand]
        private void VoltarMes()
        {
            MesAtual = MesAtual.AddMonths(-1);
            CarregarDias(MesAtual);
        }
        [ObservableProperty]
        private ObservableCollection<DiaCalendario> diasDoMes = new();
        [ObservableProperty]
        private ObservableCollection<AgendamentoDto> agendamentos = new();


        private void CarregarDias(DateTime mesRef)
        {
            DiasSemana.Clear();
            DiasDoMes.Clear();

            string[] diasSemana = { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };

            var inicio = new DateTime(MesAtual.Year, MesAtual.Month, 1);
            var ultimoDia = inicio.AddMonths(1).AddDays(-1);
            var diasNoMes = DateTime.DaysInMonth(MesAtual.Year, MesAtual.Month);

            for (int i = 0; i < diasNoMes; i++)
            {
                DiasDoMes.Add(new DiaCalendario
                {
                    Data = inicio.AddDays(i),
                    Agendamentos = new ObservableCollection<AgendamentoDto>(),
                    
                });
            }

            int startIndex = (int)inicio.DayOfWeek;
            var ordemDias = diasSemana.Skip(startIndex).Concat(diasSemana.Take(startIndex)).ToList();
            foreach (var dia in ordemDias)
                DiasSemana.Add(dia);
        }
        private void MoverAgendamentoInMemory(AgendamentoDto ag, DateTime novaData)
        {
            if (ag == null) return;
            var velhaData = ag.Data.Date;
            var novaDataDate = novaData.Date;

            if (velhaData == novaDataDate) return;

            // 1) Atualiza o modelo (e dispara PropertyChanged)
            ag.Data = novaDataDate;

            // 2) Tira do dia antigo
            var diaAntigo = DiasDoMes.FirstOrDefault(d => d.Data.Date == velhaData);
            diaAntigo?.Agendamentos.Remove(ag);

            // 3) Coloca no novo dia (em ordem)
            var diaNovo = DiasDoMes.FirstOrDefault(d => d.Data.Date == novaDataDate);
            if (diaNovo != null)
            {
                // insere ordenado por horário
                int idx = 0;
                while (idx < diaNovo.Agendamentos.Count && diaNovo.Agendamentos[idx].Horario <= ag.Horario) idx++;
                diaNovo.Agendamentos.Insert(idx, ag);
            }

            // 4) Mantém lista “plana” sincronizada (se você usa)
            var idxPlano = ListaAgendamentos.IndexOf(ag);
            if (idxPlano >= 0)
            {

                ListaAgendamentos.RemoveAt(idxPlano);

                int pos = 0;
                while (pos < ListaAgendamentos.Count &&
                       (ListaAgendamentos[pos].Data < ag.Data ||
                        (ListaAgendamentos[pos].Data == ag.Data && ListaAgendamentos[pos].Horario <= ag.Horario)))
                    pos++;
                ListaAgendamentos.Insert(pos, ag);
            }

            // 5) Ajusta seleção/destaques se você usa highlight por dia
            DataSelecionada = novaDataDate;
            foreach (var d in DiasDoMes)
                d.Selecionado = d.Data.Date == novaDataDate;

        }
        [RelayCommand]
        private async Task AbrirDetalhes(AgendamentoDto ag)
        {
            AgendamentoSelecionado = ag;
            /*var historicoDtos = await _clienteService.GetHistoricoAsync(ag.ClienteId);
            foreach (var dto in historicoDtos.OrderByDescending(a => a.Data).ThenBy(a => a.Horario))
                AgendamentoSelecionado.h */
            Debug.WriteLine($"[ABRIR DETALHES] Agendamento Selecionado = {AgendamentoSelecionado.Id}");
            MostrarDetalhes = true;
        }
        [RelayCommand]
        private void FecharDetalhes()
        {
            MostrarDetalhes = false;
            AgendamentoSelecionado = null;
            
        }
        partial void OnMesAtualChanged(DateTime value) => _ = ReloadMonthAsync();
        private readonly List<TimeSpan> _horariosFixos = new()
        {
            TimeSpan.Parse("09:00"),
            TimeSpan.Parse("10:00"),
            TimeSpan.Parse("11:00"),
            TimeSpan.Parse("14:00"),
            TimeSpan.Parse("15:00"),
            TimeSpan.Parse("16:00"),
            TimeSpan.Parse("17:00"),
            TimeSpan.Parse("18:00"),
            TimeSpan.Parse("19:00")
        };
        [RelayCommand]
        public async Task AbrirPagamentosAsync(AgendamentoDto ag)
        {
            Debug.WriteLine("Abrir pagamentos chamado");
            if (ag is null) return;



            PagamentosVM = await _acoes.CriarPagamentosViewModelAsync(ag.Id);
            MostrarPagamentos = true;



        }
        [RelayCommand]
        public void FecharPagamentos()
        {
            MostrarPagamentos = false;
            PagamentosView = null;
        }

    }
}
