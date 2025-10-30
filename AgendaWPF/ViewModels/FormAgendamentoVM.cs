
using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using AgendaWPF.Models;
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
using System.Windows;

namespace AgendaWPF.ViewModels
{
    public partial class FormAgendamentoVM : ObservableObject
    {
        private readonly IClienteService _clienteService;
        private readonly IServicoService _servicoService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly AgendaState _state;

        [ObservableProperty] private bool mostrarSugestoes = false;
        [ObservableProperty] private bool mostrarSugestoesServico = false;

        [ObservableProperty] private ClienteDto? clienteSelecionado;
        [ObservableProperty] private ServicoDto? servicoSelecionado;
        [ObservableProperty] private PacoteDto? pacoteSelecionado;
        [ObservableProperty] private CriancaDto? criancaSelecionada;

        [ObservableProperty] private string nomeDigitado = string.Empty;
        [ObservableProperty] private string servicoDigitado = string.Empty;
        [ObservableProperty] private string idbusca = string.Empty;

        [ObservableProperty] private ClienteDto novoCliente = new();
        [ObservableProperty] private ServicoDto novoServico = new();
        [ObservableProperty] private AgendamentoDto novoAgendamento = new();
        [ObservableProperty] private PagamentoCreateDto novoPagamento = new();
        [ObservableProperty] private AgendamentoCreateDto createDto = new();

        // Listas

        [ObservableProperty] private ObservableCollection<ClienteDto> clientesFiltrados = new();
        [ObservableProperty] private ObservableCollection<ClienteDto> listaClientes = new();
        [ObservableProperty] private ObservableCollection<CriancaDto> listaCriancas = new();
        [ObservableProperty] private ObservableCollection<ServicoDto> servicosFiltrados = new();
        [ObservableProperty] private ObservableCollection<ServicoDto> listaServicos = new();
        [ObservableProperty] private ObservableCollection<PacoteDto> listaPacotes = new();
        [ObservableProperty] private ObservableCollection<PacoteDto> listaPacotesFiltrada = new();
        [ObservableProperty] private ObservableCollection<AgendamentoDto> listaAgendamentos = new();
        public ObservableCollection<PagamentoDto> Pagamentos { get; } = new();

        //Data e horario
        [ObservableProperty] private DateTime dataSelecionada;
        [ObservableProperty] private ObservableCollection<string> horariosDisponiveis = new();


        //Enums
        public IEnumerable<IdadeUnidade> IdadesUnidadeDisponiveis => Enum.GetValues(typeof(IdadeUnidade)).Cast<IdadeUnidade>();
        public IEnumerable<Genero> GenerosLista => Enum.GetValues(typeof(Genero)).Cast<Genero>();
        public IEnumerable<TipoEntrega> TiposEntrega => Enum.GetValues(typeof(TipoEntrega)).Cast<TipoEntrega>();
        [ObservableProperty] private TipoEntrega tipoSelecionado = TipoEntrega.Foto;



        public FormAgendamentoVM(
            AgendaState state,
            IClienteService clienteService,
            IServicoService servicoService,
            IAgendamentoService agendamentoService)
        {
            _state = state;
            _clienteService = clienteService;
            _servicoService = servicoService;
            _agendamentoService = agendamentoService;
            DataSelecionada = _state.SelectedDate;
            CreateDto.Data = _state.SelectedDate;
            _ = CarregarDadosDoBancoAsync();
            AtualizarHorariosDisponiveis();
        }
        public async Task CarregarDadosDoBancoAsync()
        {
            await Task.WhenAll(CarregarClientesAsync(), CarregarServicosAsync(), CarregarPacotesAsync());
        }
        public async Task CarregarClientesAsync()
        {
            var lista = await _clienteService.ObterClientesAsync() ?? new List<ClienteDto>();

            ListaClientes.Clear();
            foreach (var c in lista)
                ListaClientes.Add(c);
        }
        public async Task CarregarServicosAsync()
        {
            var lista = await _servicoService.GetAllAsync() ?? new List<ServicoDto>();
            ListaServicos.Clear();
            foreach (var s in lista)
                ListaServicos.Add(s);
        }
        public async Task CarregarPacotesAsync()
        {
            var pacotes = await _servicoService.GetPacotesAsync() ?? new List<PacoteDto>();

            ListaPacotes.Clear();
            foreach (var p in pacotes)
                ListaPacotes.Add(p);
        }



        // ----------- Metodos De Auto Complete -------------- //

        partial void OnNomeDigitadoChanged(string value)
        {

            var termo = value?.ToLower() ?? "";

            ClientesFiltrados.Clear();

            foreach (var c in ListaClientes.Where(c =>
             (!string.IsNullOrEmpty(c.Nome) && c.Nome.Contains(termo)) ||
             (!string.IsNullOrEmpty(c.Telefone) && c.Telefone.Contains(termo))))
                ClientesFiltrados.Add(c);

            MostrarSugestoes = ClientesFiltrados.Count > 0;
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

            ListaCriancas.Clear();
            foreach (var cr in cliente.Criancas)
                ListaCriancas.Add(cr);

            if (cliente.Criancas?.Count > 0)
                CriancaSelecionada = cliente.Criancas[0];

            OnPropertyChanged(nameof(NovoCliente));
        }
        partial void OnServicoDigitadoChanged(string value)
        {

            var termo = value?.ToLower() ?? "";

            servicosFiltrados.Clear();
            foreach (var s in ListaServicos.Where(c =>
             (!string.IsNullOrEmpty(c.Nome) && c.Nome.ToLower().Contains(termo))))
                ServicosFiltrados.Add(s);


            MostrarSugestoesServico = ServicosFiltrados.Any();

        }
        partial void OnServicoSelecionadoChanged(ServicoDto? value)
        {
            ServicoDigitado = value?.Nome ?? string.Empty;
            if (value is not null)
            {
                NovoAgendamento.ServicoId = value.Id;
                FiltrarPacotesPorServico(value.Id);
            }

            Debug.WriteLine($"ServicoSelecionado mudou para: {(value == null ? "null" : value.Id.ToString())}");

        }
        public void FiltrarPacotesPorServico(int servicoId)
        {
            ListaPacotesFiltrada.Clear();

            var pacotesFiltrados = ListaPacotes.Where(p => p.ServicoId == servicoId)
                .OrderBy(p => p.Nome)
                .ToList();

            foreach (var pacote in pacotesFiltrados)
                ListaPacotesFiltrada.Add(pacote);

            if (ListaPacotesFiltrada.Count > 0)
                PacoteSelecionado = ListaPacotesFiltrada[0];
        }
        partial void OnPacoteSelecionadoChanged(PacoteDto? value)
        {
            Debug.WriteLine("onpctslcchanged chamado");
            if (value is not null)
            {
                NovoAgendamento.PacoteId = value.Id;
                NovoAgendamento.Valor = value.Valor;
            }
            Debug.WriteLine($"PacoteSelecionado mudou para: {(value == null ? "null" : value.Id.ToString())}");
        }
        private readonly List<string> _horariosFixos = new()
        {
            "09:00", "10:00", "11:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00"
        };

        // ----------- Metodos De Horarios Disponiveis -------------- //
        public void AtualizarHorariosDisponiveis()
        {

            var horarioStr = NovoAgendamento?.Horario.ToString(@"hh\:mm");

            var ocupados = ListaAgendamentos
                .Where(a => a.Data.Date == DataSelecionada.Date && (NovoAgendamento.Id == 0 || a.Id != NovoAgendamento.Id))
                .Select(a => a.Horario.ToString(@"hh\:mm"))
                .Where(h => !string.IsNullOrEmpty(h))
                .ToList();


            /* if (NovoAgendamento.Horario.HasValue)
             {
                 var horarioSelecionado = NovoAgendamento.Horario..ToString(@"hh\:mm");
                 if (!ocupados.Contains(horarioSelecionado))
                 {
                     ocupados.Add(horarioSelecionado);
                 }
             } */


            var livres = _horariosFixos
                .Where(h => !ocupados.Contains(h))
                .ToList();


            HorariosDisponiveis.Clear();
            foreach (var h in livres)
                HorariosDisponiveis.Add(h);

            if (!string.IsNullOrEmpty(horarioStr) && !HorariosDisponiveis.Contains(horarioStr))
            {
                HorariosDisponiveis.Add(horarioStr);
            }
        }
        partial void OnDataSelecionadaChanged(DateTime value)
        {
            CreateDto.Data = value.Date;
            Debug.WriteLine($"Form.DataSelecionada = {value:yyyy-MM-dd}  -> DTO.Data = {CreateDto.Data:yyyy-MM-dd}");
        }

        // ----------- POST / PUT Agendamento -------------- //

        public void PrepararCreateDto()
        {
            // validações de nulo/zero aqui já evitam 0 indo pro servidor
            if (ClienteSelecionado is null || ClienteSelecionado.Id <= 0)
                throw new InvalidOperationException("Selecione um cliente válido.");

            if (ServicoSelecionado is null || ServicoSelecionado.Id <= 0)
                throw new InvalidOperationException("Selecione um serviço válido.");

            if (PacoteSelecionado is null || PacoteSelecionado.Id <= 0)
                throw new InvalidOperationException("Selecione um pacote válido.");

            // Zera/normaliza antes de preencher
            CreateDto = new AgendamentoCreateDto();

            CreateDto.ClienteId = ClienteSelecionado.Id;
            CreateDto.ServicoId = ServicoSelecionado.Id;
            CreateDto.PacoteId = PacoteSelecionado.Id;

            // Criança é opcional: só manda se tiver Id > 0
            CreateDto.CriancaId = (CriancaSelecionada != null && CriancaSelecionada.Id > 0)
                ? CriancaSelecionada.Id
                : (int?)null;

            CreateDto.Data = DataSelecionada.Date;

            // Garanta que você tem um TimeSpan válido vindo da UI
            // Se você guarda o horário como string "HH:mm", parseie aqui:
            var hhmm = NovoAgendamento.Horario.ToString(@"hh\:mm"); // ou pegue de um binding específico
            if (!TimeSpan.TryParse(hhmm, out var horario))
                throw new InvalidOperationException("Selecione um horário válido.");
            CreateDto.Horario = horario;

            CreateDto.Status = StatusAgendamento.Confirmado;
            CreateDto.Tema = string.IsNullOrWhiteSpace(NovoAgendamento.Tema) ? string.Empty : NovoAgendamento.Tema;

            // Se tiver pagamento inicial
            CreateDto.PagamentoInicial = (NovoPagamento != null && NovoPagamento.Valor > 0)
                ? new PagamentoCreateDto
                {
                    Valor = NovoPagamento.Valor,
                    DataPagamento = NovoPagamento.DataPagamento == default ? DateTime.UtcNow : NovoPagamento.DataPagamento,
                    Metodo = NovoPagamento.Metodo,
                    Observacao = NovoPagamento.Observacao
                }
                : null;
        }

        [RelayCommand]
        public async Task AgendarAsync()
        {
            // --------------- Validações Iniciais --------------- //
            agendamentoIdAtual = Guid.NewGuid();
            Debug.WriteLine($"Agendamento iniciado - ID: {agendamentoIdAtual}");
            if (!ValidarDadosBasicos()) return;
            int? agendamentoidnotificacao = NovoAgendamento.Id;

            // --------------------------------------------------- //
            PrepararCreateDto();
            try
            {
                 var criado = await _agendamentoService.AgendarAsync(CreateDto);
                 NovoAgendamento = criado;
                await FinalizarAgendamento();
                //else
                //EditarAgendamento();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao agendar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private Guid agendamentoIdAtual;
        private async Task FinalizarAgendamento()
        {
            await CarregarDadosDoBancoAsync();
            AtualizarHorariosDisponiveis();
            NovoAgendamento = new AgendamentoDto { Data = DataSelecionada };
        }
        private bool ValidarDadosBasicos()
        {
            if (ClienteSelecionado == null || ClienteSelecionado.Id <= 0)
            {
                MessageBox.Show("Selecione um cliente.", "Cliente obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(NovoAgendamento.Horario.ToString(@"hh\:mm")))
            {
                MessageBox.Show("Por favor, selecione um horário antes de agendar.", "Horário obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (ServicoSelecionado == null || ServicoSelecionado.Id <= 0)
            {
                MessageBox.Show("Selecione um serviço.", "Serviço obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (PacoteSelecionado == null || PacoteSelecionado.Id <= 0)
            {
                MessageBox.Show("Selecione um pacote.", "Pacote obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
