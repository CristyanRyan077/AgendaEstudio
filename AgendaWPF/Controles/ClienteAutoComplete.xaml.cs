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
    /// Interação lógica para ClienteAutoComplete.xam
    /// </summary>
    public partial class ClienteAutoComplete : UserControl
    {
        
        public ClienteAutoComplete()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                if (DataContext is AgendaViewModel vm)
                    Debug.WriteLine($"AutoComplete conectado ao VM com {vm.ListaClientes.Count} clientes");
            };
        }
     
        private void AutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is AgendaViewModel vm)
                vm.MostrarSugestoes = true;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AgendaViewModel vm)
                vm.MostrarSugestoes = false;
        }

        private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is AgendaViewModel vm)
                vm.MostrarSugestoes = false;
        }
    }
}
