using AgendaWPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private object viewAtual;
        public IRelayCommand IrParaSemanalCommand { get; }
        public IRelayCommand IrParaClientesCommand { get; }
        public IRelayCommand IrParaCalendarioCommand { get; }
        public MainViewModel(
            CardSemanal cardView,
            ClientesView clientesView,
            CalendarioView calendarioView)

        {
            IrParaSemanalCommand = new RelayCommand(() => ViewAtual = cardView);
            IrParaClientesCommand = new RelayCommand(() => ViewAtual = clientesView);
            IrParaCalendarioCommand = new RelayCommand(() => ViewAtual = calendarioView);
          //IrparaAgendarCommand => CardSemanal.xaml.cs


            ViewAtual = cardView; // Tela inicial
        }

    }
}
