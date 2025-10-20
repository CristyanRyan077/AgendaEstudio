using AgendaShared.DTOs;

namespace AgendaMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly List<AgendamentoDto> _agendamentos;
        private readonly List<ClienteDto> _clientes;

        public MainPage(List<AgendamentoDto> agendamentos, List<ClienteDto> clientes)
        {
            InitializeComponent();

            _agendamentos = agendamentos;
            _clientes = clientes;

            //AgendamentosListView.ItemsSource = agendamentos;
            //ClientesListView.ItemsSource = clientes;


        }
    }
}
