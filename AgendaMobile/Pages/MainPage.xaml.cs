using AgendaMobile.ViewModels;
using AgendaShared.DTOs;

namespace AgendaMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly List<AgendamentoDto> _agendamentos;
        private readonly List<ClienteDto> _clientes;
        private readonly CalendarioViewModel _viewModel;

        public MainPage(CalendarioViewModel viewmodel)
        {
            InitializeComponent();
            BindingContext = viewmodel;


        }
    }
}
