using AgendaShared.DTOs;
using AgendaWPF.Controles;
using AgendaWPF.Models;
using AgendaWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgendaWPF.Views
{
    /// <summary>
    /// Interação lógica para CardSemanal.xam
    /// </summary>
    public partial class CardSemanal : UserControl
    {
        private AgendarView? _agendar;
        private AdicionarPagamento _pagamento;
        private readonly IServiceProvider _sp;
        private readonly AgendaState _state;
        public AgendaViewModel vm { get; }
        public CardSemanal(AgendaViewModel agendaVm, IServiceProvider sp, AgendaState state)
        {
            InitializeComponent();
            vm = agendaVm;
            DataContext = vm;
            _sp = sp;
            _state = state;
            Loaded += async (_, __) => await vm.InicializarAsync();
        }
        public AgendarView GetAgendar()
        {
            if (_agendar == null || !_agendar.IsLoaded)
            {
                _agendar = _sp.GetRequiredService<AgendarView>();
                _agendar.Closed += (s, e) => _agendar = null;
                _agendar.Show();
            }
            else
            {
                _agendar.Activate();
            }
            return _agendar;
        }

        private void BtnAgenda_Click(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, this);
            Keyboard.ClearFocus();

            if (sender is Button btn && btn.Tag is DateTime dia)
            {
                // Atualiza estado compartilhado e a VM da agenda (opcional)
                _state.SelectedDate = dia.Date;
                if (DataContext is AgendaViewModel vm)
                    vm.DataSelecionada = dia.Date;

                var win = GetAgendar();
                if (win.DataContext is FormAgendamentoVM formvm)
                    formvm.DataSelecionada = dia.Date;
            }
        }

        private void AgendamentoBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("click!");
            if (sender is not FrameworkElement fe) return;
            Debug.WriteLine("F.E check");
            if (fe.DataContext is not AgendamentoDto ag) { return; }
            Debug.WriteLine("Datacontext Check");
            if (DataContext is AgendaViewModel vm &&
                   vm.AbrirDetalhesCommand.CanExecute(ag))
            {
                Debug.WriteLine("can execute check");
                vm.AbrirDetalhesCommand.Execute(ag);
                e.Handled = true; // opcional
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (sender is not FrameworkElement fe) return;
      
            if (fe.DataContext is not AgendamentoDto ag) { return; }

            if (DataContext is AgendaViewModel vm)
            {
        
                vm.AbrirPagamentosCommand.Execute(ag);
                e.Handled = true; // opcional
                var vmPag = ActivatorUtilities.CreateInstance<PagamentosViewModel>(_sp, ag.Id);
                _pagamento = ActivatorUtilities.CreateInstance<AdicionarPagamento>(_sp, vmPag);
                _pagamento.Show();
            }
        }
    }
}
