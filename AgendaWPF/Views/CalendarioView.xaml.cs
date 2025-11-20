
using AgendaShared.DTOs;
using AgendaWPF.Controles;
using AgendaWPF.Models;
using AgendaWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgendaWPF.Views
{
    /// <summary>
    /// Interação lógica para CalendarioView.xam
    /// </summary>
    public partial class CalendarioView : UserControl
    {
        private Point _dragStart;
        private bool _mouseDown;
        private AgendarView? _agendar;
        private readonly IServiceProvider _sp;
        private AgendaState _agendaState;
        private bool _dragging;
        private AdicionarPagamento _pagamento;
        public CalendarioViewModel viewmodel { get; }
        public CalendarioView(CalendarioViewModel vm, IServiceProvider sp, AgendaState state)
        {
            InitializeComponent();
            viewmodel = vm;
            DataContext = vm;
            _sp = sp;
            _agendaState = state;
            Loaded += async (_, __) => await viewmodel.InicializarAsync();
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
                _agendaState.SelectedDate = dia.Date;
                if (DataContext is CalendarioViewModel vm)
                    vm.DataSelecionada = dia.Date;

                var win = GetAgendar();
                if (win.DataContext is FormAgendamentoVM formvm)
                    formvm.DataSelecionada = dia.Date;
            }
        }
        private static T FindAncestorByName<T>(DependencyObject current, string name) where T : FrameworkElement
        {
            while (current != null)
            {
                if (current is T fe && fe.Name == name) return fe;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        private void ItemBorder_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is AgendamentoDto ag
                && DataContext is CalendarioViewModel vm);
                //vm.SelecionarAgendamento(ag);
        }
        private void Dia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject d && FindAncestorByName<Border>(d, "ItemBorder") != null)
                return;

            if (sender is Border && ((FrameworkElement)sender).DataContext is DiaCalendario dia
                && DataContext is CalendarioViewModel vm)
            {
               // vm.SelecionarDia(dia.Data);
            }
        }



        private void Agendamento_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown || e.LeftButton != MouseButtonState.Pressed)
                return;
            var pos = e.GetPosition(null);
            var diff = new Vector(Math.Abs(pos.X - _dragStart.X),
                                  Math.Abs(pos.Y - _dragStart.Y));

            // Threshold padrão do Windows
            if (diff.X < SystemParameters.MinimumHorizontalDragDistance &&
                diff.Y < SystemParameters.MinimumVerticalDragDistance)
                return;

            // Só aqui inicia o drag
            var fe = (FrameworkElement)sender;
            if (fe.DataContext is not AgendamentoDto ag)
                return;

            var data = new DataObject(typeof(AgendamentoDto), ag);
            _mouseDown = false;
            _dragging = true;
            e.Handled = true;
            DragDrop.DoDragDrop(fe, data, DragDropEffects.Move);


        }

        private void Agendamento_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not AgendamentoDto ag) { _mouseDown = false; _dragging = false; return; }

            // Clique só acontece se NÃO virou arrasto
            if (_mouseDown && !_dragging)
            {
                // Dispara o comando manualmente
                if (DataContext is CalendarioViewModel vm &&
                    vm.AbrirDetalhesCommand.CanExecute(ag))
                {
                    vm.AbrirDetalhesCommand.Execute(ag);
                    e.Handled = true; // opcional
                }
            }

            _mouseDown = false;
            _dragging = false;
        }
        private void Dia_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(AgendamentoDto)))
            {
                e.Effects = DragDropEffects.Move;
                ((Border)sender).Background = Brushes.LightBlue;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
        private void Agendamento_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
        }

        private async void Dia_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(typeof(AgendamentoDto))) return;

                var ag = (AgendamentoDto)e.Data.GetData(typeof(AgendamentoDto));
                var cellVm = (DiaCalendario)((FrameworkElement)sender).DataContext;
                var novaData = cellVm.Data;

                var vm = (CalendarioViewModel)DataContext;


                var oldDate = ag.Data;

                await vm.MoverAgendamentoAsyncCommand.ExecuteAsync((ag, novaData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro no Drop de reagendamento: " + ex);
                
            }
            finally
            {
                ((Border)sender).ClearValue(Border.BackgroundProperty);
                e.Handled = true;
            }
        }
        private void BtnLembretes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
                return;

            if (btn.DataContext is not DiaCalendario dia)
                return;

            // Se quiser, pode ignorar se por algum motivo não tiver lembretes
            if (!dia.TemLembretes)
                return;

            var win = new LembretesDoDiaWindow
            {
                DataContext = dia,
                Owner = Window.GetWindow(this)
            };

            win.ShowDialog();
        }

        private void Dia_PreviewDragLeave(object sender, DragEventArgs e)
        {
            ((Border)sender).ClearValue(Border.BackgroundProperty);
            e.Handled = true;
        }

        private void Agendamento_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not AgendamentoDto) return;

            _mouseDown = true;
            _dragging = false;
            _dragStart = e.GetPosition(null);
         
        }
        private void AbrirPagamento(object sender, RoutedEventArgs e)
        {

            if (sender is not FrameworkElement fe) return;

            if (fe.DataContext is not AgendamentoDto ag) { return; }

            if (DataContext is CalendarioViewModel vm)
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
