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
        private EditarAgendamento? _editarAgendamento;
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
            Loaded += OnCardSemanalLoaded;
            Unloaded += OnCardSemanalUnloaded;

        }
        private void BtnLembretes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
                return;

            if (btn.DataContext is not DiaAgendamento dia)
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
        private void OnCardSemanalLoaded(object sender, RoutedEventArgs e)
        {
            // Se inscreve no evento da ViewModel
            if (vm != null)
            {
                vm.RequestBringIntoView += Vm_RequestBringIntoView;
            }
            // A inicialização que estava no construtor antigo
            _ = vm?.InicializarAsync();
        }

        private void OnCardSemanalUnloaded(object sender, RoutedEventArgs e)
        {
            // Limpa a inscrição para evitar memory leaks
            if (vm != null)
            {
                vm.RequestBringIntoView -= Vm_RequestBringIntoView;
            }
        }
        private void Vm_RequestBringIntoView(object? sender, AgendamentoVM agendamento)
        {
            // O ViewModel já fez o Dispatcher.Yield, então podemos chamar diretamente
            BringCardIntoView(agendamento);
        }
        private void BringCardIntoView(AgendamentoVM alvo)
        {
            if (alvo == null || vm == null) return;

            // 1. Encontrar o VM do DIA
            var diaVM = vm.DiasSemana.FirstOrDefault(d => d.Data.Date == alvo.Data.Date);
            if (diaVM == null) return; // Dia não está na tela

            // 2. Encontrar o Container do DIA (o StackPanel no UniformGrid)
            // Usa o x:Name "DiasSemanaItemsControl"
            var diaContainer = DiasSemanaItemsControl.ItemContainerGenerator.ContainerFromItem(diaVM) as FrameworkElement;
            if (diaContainer == null)
            {
                DiasSemanaItemsControl.UpdateLayout(); // Força a renderização
                diaContainer = DiasSemanaItemsControl.ItemContainerGenerator.ContainerFromItem(diaVM) as FrameworkElement;
                if (diaContainer == null) return; // Não foi possível encontrar o container do dia
            }

            // 3. Encontrar o ItemsControl *interno* (dos agendamentos)
            // Usa o x:Name "AgendamentosItemsControl"
            var agendamentosItemsControl = FindVisualChild<ItemsControl>(diaContainer, "AgendamentosItemsControl");
            if (agendamentosItemsControl == null) return; // Não foi possível encontrar o ItemsControl interno

            // 4. Encontrar o Container do AGENDAMENTO (o Border)
            var agendamentoContainer = agendamentosItemsControl.ItemContainerGenerator.ContainerFromItem(alvo) as FrameworkElement;
            if (agendamentoContainer == null)
            {
                agendamentosItemsControl.UpdateLayout(); // Força a renderização interna
                agendamentoContainer = agendamentosItemsControl.ItemContainerGenerator.ContainerFromItem(alvo) as FrameworkElement;
                if (agendamentoContainer == null) return; // Não foi possível encontrar o card
            }

            // 5. Rolar e Piscar
            // Chamar BringIntoView no container final rola *todos* os ScrollViewers pais.
            agendamentoContainer.BringIntoView();
            FlashBorder(agendamentoContainer);
        }
        private T? FindVisualChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;

            T? foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is not T childType)
                {
                    foundChild = FindVisualChild<T>(child, childName); // Busca recursiva
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (childType.Name == childName)
                    {
                        foundChild = childType;
                        break;
                    }
                }
            }
            return foundChild;
        }

        /// <summary>
        /// O método FlashBorder (mantido como estava).
        /// </summary>
        private async void FlashBorder(FrameworkElement? fe)
        {
            if (fe is null) return;

            // O seu XAML usa um Border para o card, então isso deve funcionar.
            // Se o card for outro controle, ajuste o 'fe as Border'.
            var b = fe as Border;
            if (b == null) return;

            var old = b.BorderBrush;
            var oldThick = b.BorderThickness;

            b.BorderBrush = Brushes.Gold; // (adicione using System.Windows.Media;)
            b.BorderThickness = new Thickness(3);
            await Task.Delay(500);
            b.BorderBrush = old;
            b.BorderThickness = oldThick;
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
                if (_agendar.WindowState == WindowState.Minimized)
                {
                    _agendar.WindowState = WindowState.Normal;
                }
                _agendar.Activate();
            }
            return _agendar;
        }
        public EditarAgendamento GetEditar()
        {
            if (_editarAgendamento == null || !_editarAgendamento.IsLoaded)
            {
                _editarAgendamento = _sp.GetRequiredService<EditarAgendamento>();
                _editarAgendamento.Closed += (s, e) => _editarAgendamento = null;
                _editarAgendamento.Show();
            }
            else
            {
                _editarAgendamento.Activate();
            }
            return _editarAgendamento;
        }



        private void BtnAgenda_Click(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, this);
            Keyboard.ClearFocus();

            if (sender is Button btn && btn.Tag is DateTime dia)
            {
                
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
            if (fe.DataContext is not AgendamentoVM ag) { return; }
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
      
            if (fe.DataContext is not AgendamentoVM ag) { return; }

            if (DataContext is AgendaViewModel vm)
            {
        
                vm.AbrirPagamentosCommand.Execute(ag);
                e.Handled = true; // opcional
                var vmPag = ActivatorUtilities.CreateInstance<PagamentosViewModel>(_sp, ag.Id);
                _pagamento = ActivatorUtilities.CreateInstance<AdicionarPagamento>(_sp, vmPag);
                _pagamento.Show();
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var win = GetEditar();

            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not AgendamentoVM ag) { return; }
            if (DataContext is AgendaViewModel vm)
            {
                
                e.Handled = true; // opcional
                
            }
        }
    }
}
