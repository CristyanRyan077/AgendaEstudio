using AgendaShared;
using AgendaShared.DTOs;
using AgendaShared.Helpers;
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
        public event EventHandler<AgendamentoVM>? RequestBringIntoView;
        private AgendamentoVM? _currentHighlightedAgendamento;
        public ObservableCollection<DiaAgendamento> DiasSemana { get; set; } = new();
        private readonly IAcoesService _acoes;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IClienteService _clienteService;
        private readonly IMessenger _messenger;
        private readonly ISemanaService _semanaService;
        private readonly ILembreteService _lembreteService;

        [ObservableProperty] private AgendamentoVM agendamentoSelecionado;
        [ObservableProperty] private bool mostrarDetalhes;
        [ObservableProperty] private object? pagamentosView;
        [ObservableProperty] private bool mostrarPagamentos;
        [ObservableProperty] private object? editarview;
        [ObservableProperty] private bool mostrarEditar;
        public ObservableCollection<TipoFotoOpcao> OpcoesFiltroFoto { get; } = new();
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;

        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();

        // === FIND (Ctrl+F) ===
        [ObservableProperty] private bool findBarVisivel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(FindLimparCommand))]
        private string? findTermo;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FindIndiceLabel))] // Notifica a Label quando o índice muda
        private int findIndice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FindIndiceLabel))]  // Notifica a Label
        [NotifyPropertyChangedFor(nameof(FindTemResultados))] // Notifica o bool
        [NotifyCanExecuteChangedFor(nameof(FindProximoCommand))] // Habilita/Desabilita o botão
        [NotifyCanExecuteChangedFor(nameof(FindAnteriorCommand))] // Habilita/Desabilita o botão
        private int findTotal;
        [ObservableProperty] private int? findAgendamentoIdAtual;      // para destacar o item atual
        private List<AgendamentoVM> _findHits = new();
        [ObservableProperty] private DateTime? dataFiltroSelecionada;
        public string FindIndiceLabel => FindTotal == 0 ? "0/0" : $"{FindIndice + 1}/{FindTotal}";
        public bool FindTemResultados => FindTotal > 0;
 



        public AgendaViewModel(IMessenger messenger, IAgendamentoService agendamentoService, IClienteService clienteService, ISemanaService semanaService, IAcoesService acoes, ILembreteService lembreteService)
        {
            _agendamentoService = agendamentoService;
            _clienteService = clienteService;
            _messenger = messenger;
            _semanaService = semanaService;
            _acoes = acoes;
            _lembreteService = lembreteService;
            foreach (var tipo in Enum.GetValues<TipoEntrega>())
            {
                OpcoesFiltroFoto.Add(new TipoFotoOpcao(tipo));
            }

            messenger.Register<AgendamentoCriadoMessage>(this, (_, msg) =>
            {
                AdicionarAgendamentoNaSemana(msg.Agendamento);
            });
            messenger.Register<PagamentoCriadoMessage>(this, async (r, m) =>
            {
                var agendamentoAtualizadoDto = await _agendamentoService.GetByIdAsync(m.AgendamentoId);
                if (agendamentoAtualizadoDto != null)
                {
                    var novoAgendamentoVM = new AgendamentoVM(agendamentoAtualizadoDto);
                    AgendamentoSelecionado = novoAgendamentoVM;
                    await CarregarSemanaAtualAsync();
                }
            });
            messenger.Register<ProdutoAdicionadoMessage>(this, async (r, m) =>
            {
                var agendamentoAtualizadoDto = await _agendamentoService.GetByIdAsync(m.AgendamentoId);
                if (agendamentoAtualizadoDto != null)
                {
                    var novoAgendamentoVM = new AgendamentoVM(agendamentoAtualizadoDto);
                    AgendamentoSelecionado = novoAgendamentoVM;
                    await CarregarSemanaAtualAsync();
                }
            });
            messenger.Register<FocusAgendamentoMessage>(this, async (r, msg) =>
            {
                await HandleFocusRequestAsync(msg);
            });

        }
        public async Task InicializarAsync()
        {
            DataSelecionada = DateTime.Today;
            if (DiasSemana.Count == 0)
                for (int i = 0; i < 7; i++) DiasSemana.Add(new DiaAgendamento());
            await CarregarSemanaAtualAsync();
            await PreencherLembretesAsync();
        }

        public async Task CarregarSemanaAtualAsync()
        {
            PreencherEsqueletoDaSemana(DataSelecionada);
            var inicio = _semanaService.ObterSegunda(DataSelecionada);
            var fim = inicio.AddDays(6);
            var ags = await _agendamentoService.ObterAgendamentosPorPeriodo(inicio, fim);

            var agsPorDia = ags.GroupBy(a => a.Data.Date)
                          .ToDictionary(g => g.Key,
                                        g => g.OrderBy(a => a.Horario));
            foreach (var dia in DiasSemana)
            {
                List<AgendamentoVM> vmsDoDia;
                if (agsPorDia.TryGetValue(dia.Data.Date, out var dtosDoDia))
                {
                    // Se sim, "traduza" a lista de DTOs para VMs
                    vmsDoDia = dtosDoDia.Select(dto => new AgendamentoVM(dto)).ToList();
                }
                else
                {
                    // Se não, crie uma lista vazia
                    vmsDoDia = new List<AgendamentoVM>();
                }
                dia.Agendamentos = new ObservableCollection<AgendamentoVM>(vmsDoDia);
            }
            await PreencherLembretesAsync();
        }
        private async Task PreencherLembretesAsync()
        {
            var inicio = _semanaService.ObterSegunda(DataSelecionada);
            var fim = inicio.AddDays(6);
            var query = new LembreteQuery
            {
                Inicio = inicio,
                Fim = fim
            };
            var lista = await _lembreteService.ListAsync(query);


            // 2) Agrupa por dia
            var porDia = lista
                .GroupBy(l => l.DataAlvo.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3) Preenche na sua grade de dias
            foreach (var dia in DiasSemana)
            {

                dia.Lembretes.Clear();

                if (porDia.TryGetValue(dia.Data.Date, out var doDia))
                {
                    foreach (var l in doDia)
                        dia.Lembretes.Add(new LembretesVM(l));
                }
            }
        }
        private void PreencherEsqueletoDaSemana(DateTime referencia)
        {
            var inicio = _semanaService.ObterSegunda(referencia);
            
            for (int i = 0; i < 7; i++)
            {
                var d = inicio.AddDays(i);
                DiasSemana[i].Data = d;   
            }
        }

        private void AdicionarAgendamentoNaSemana(AgendamentoVM criado)
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
        partial void OnFindTermoChanged(string? value)
        {
            _ = RebuildFindHitsAsync();
        }
        [RelayCommand]
        private async Task ToggleFiltroAsync()
        {
            // Apenas chama a busca novamente
            await RebuildFindHitsAsync();
        }
        private async Task RebuildFindHitsAsync()
        {
            var termo = FindTermo?.Trim();

            _findHits.Clear();  
            FindAgendamentoIdAtual = null;
            var tiposSelecionados = OpcoesFiltroFoto
            .Where(o => o.IsSelected)
            .Select(o => o.Valor)
            .ToHashSet();

            if (string.IsNullOrWhiteSpace(termo) && !tiposSelecionados.Any())
            {
                FindTotal = 0;
                FindIndice = 0;
                return;
            }
            foreach (var dia in DiasSemana)
            {
                foreach (var agendamento in dia.Agendamentos)
                {
                    // Se a lista de tipos selecionados não estiver vazia E o tipo do agendamento estiver nela
                    if (tiposSelecionados.Any() && tiposSelecionados.Contains(agendamento.Tipo))
                    {
                        agendamento.IsTypeFilterMatch = true;
                    }
                    else
                    {
                        agendamento.IsTypeFilterMatch = false;
                    }
                }
            }

            // 1) chamada da api para buscar os agendamentos
            List<AgendamentoDto> dtosEncontrados;
            var filtro = new AgendamentoSearchFilter
            {
                SearchTerm = termo,
                TiposDeFotoSelecionados = tiposSelecionados.ToList()
            };
            try { dtosEncontrados = await _agendamentoService.SearchAgendamentoAsync(filtro); }

            catch (Exception ex)
            { 
                Debug.WriteLine($"Erro ao buscar agendamentos: {ex.Message}");
                dtosEncontrados = new List<AgendamentoDto>();
            }
            var todosVMsAtuais = DiasSemana.SelectMany(d => d.Agendamentos).ToList();
 

            // 2. Converter os DTOs em VMs
            _findHits = dtosEncontrados
                .Select(dto => new AgendamentoVM(dto)) // Converte DTO para VM
                .OrderBy(a => a.Data)
                .ThenBy(a => a.Horario)
                .ToList();

            // 3. O resto da lógica é o mesmo
            FindTotal = _findHits.Count;

            if (FindTotal > 0)
            {
                FindIndice = 0;
                var primeiro = _findHits[0];
                FindAgendamentoIdAtual = primeiro.Id;

                UpdateHighlight(primeiro);
                _messenger.Send(new FocusAgendamentoMessage(primeiro.Id, primeiro.Data));
            }
            else
            {
                FindIndice = 0;
            }
        }



        private void IrParaResultado(int novoIndice)
        {
            if (FindTotal == 0) return;


            FindIndice = (novoIndice + FindTotal) % FindTotal;

            var alvo = _findHits[FindIndice];
            FindAgendamentoIdAtual = alvo.Id;
            UpdateHighlight(alvo);
            _messenger.Send(new FocusAgendamentoMessage(alvo.Id, alvo.Data));
        }
        [RelayCommand(CanExecute = nameof(FindTemResultados))]
        private void FindProximo()
        {
            IrParaResultado(FindIndice + 1);
        }

        [RelayCommand(CanExecute = nameof(FindTemResultados))]
        private void FindAnterior()
        {
            IrParaResultado(FindIndice - 1);
        }

        [RelayCommand]
        private void FindLimpar()
        {
            FindTermo = null;
            foreach (var opcao in OpcoesFiltroFoto)
            {
                // Define false sem disparar a notificação propertychanged se quiser performance,
                // ou apenas set = false normal.
                opcao.IsSelected = false;
            }
            if (_currentHighlightedAgendamento != null)
            {
                _currentHighlightedAgendamento.IsCurrentFindHit = false;
                _currentHighlightedAgendamento = null;
            }
        }
        private void UpdateHighlight(AgendamentoVM novoAlvo)
        {
            // 1. Limpa o destaque anterior (se houver)
            if (_currentHighlightedAgendamento != null)
            {
                _currentHighlightedAgendamento.IsCurrentFindHit = false;
                
                _currentHighlightedAgendamento = null;
            }

            // 2. Define o novo destaque
            novoAlvo.IsCurrentFindHit = true;
            _currentHighlightedAgendamento = novoAlvo;
        }
        private async Task HandleFocusRequestAsync(FocusAgendamentoMessage msg)
        {
            // 1. Descobrir o início e fim da semana atual
            // (Assumindo que _semanaService existe e DataSelecionada é a referência)
            var inicioSemana = _semanaService.ObterSegunda(DataSelecionada);
            var fimSemana = inicioSemana.AddDays(6);

            // 2. Se a data do agendamento estiver fora da semana, carrega a nova semana
            if (msg.Data.Date < inicioSemana.Date || msg.Data.Date > fimSemana.Date)
            {
                // Define a nova data E espera o carregamento terminar
                DataSelecionada = msg.Data;
                await CarregarSemanaAtualAsync();
            }

            // 3. Agora que a semana correta está carregada, encontra o AgendamentoVM
            var agendamentoVM = DiasSemana
                .SelectMany(dia => dia.Agendamentos)
                .FirstOrDefault(a => a.Id == msg.AgendamentoId);

            if (agendamentoVM != null)
            {
                UpdateHighlight(agendamentoVM);
                // 4. Aguarda a UI ter a chance de criar os containers (ex: se a semana mudou)
                await System.Windows.Threading.Dispatcher
                .Yield(System.Windows.Threading.DispatcherPriority.Background);

                // 5. Dispara o evento para o code-behind fazer a mágica da UI
                RequestBringIntoView?.Invoke(this, agendamentoVM);
            }
        }
        [RelayCommand]
        private void FindToggle() => FindBarVisivel = !FindBarVisivel;
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
                
        [RelayCommand]
        private async Task AbrirDetalhes(AgendamentoVM ag)
        {
            AgendamentoSelecionado = ag;
            var historicoDtos = await _clienteService.GetHistoricoAsync(ag.ClienteId);
            foreach (var dto in historicoDtos.OrderByDescending(a => a.Data).ThenBy(a => a.Horario))
                AgendamentoSelecionado.HistoricoAgendamentos.Add(dto);
           
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
        public async Task AbrirPagamentosAsync(AgendamentoVM ag)
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
        [RelayCommand]
        public void AbrirEditarAsync(AgendamentoVM ag)
        {
            if (ag is null) return;
            AgendamentoSelecionado = ag;
            MostrarEditar = true;
        }
    }

}
