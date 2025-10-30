using AgendaWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AgendaWPF.Controles
{
    /// <summary>
    /// Interação lógica para ServicoAutoComplete.xam
    /// </summary>
    public partial class ServicoAutoComplete : UserControl
    {
        public ServicoAutoComplete()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                if (DataContext is FormAgendamentoVM vm)
                    Debug.WriteLine($"AutoComplete conectado ao VM com {vm.ListaServicos.Count} servicos");
            };
        }
        private void AutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is FormAgendamentoVM vm)
                vm.MostrarSugestoesServico = true;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is FormAgendamentoVM vm)
                vm.MostrarSugestoesServico = false;
        }

        private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is FormAgendamentoVM vm)
                vm.MostrarSugestoesServico = false;
        }
    }
}
