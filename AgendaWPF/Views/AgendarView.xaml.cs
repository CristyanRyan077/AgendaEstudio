using AgendaWPF.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AgendaWPF.Views
{
    /// <summary>
    /// Lógica interna para AgendarView.xaml
    /// </summary>
    public partial class AgendarView : Window
    {
        public FormAgendamentoVM viewmodel { get; }
        public AgendarView(FormAgendamentoVM vm)
        {
            InitializeComponent();
            viewmodel = vm;
            DataContext = vm;
            Loaded += async (_, __) => await viewmodel.InitAsync();
            if (DataContext is FormAgendamentoVM form)
                form.RequestClose += (_, __) => Close();
        }
        private async void txtIdBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as FormAgendamentoVM;
            if (vm == null) return;

            if (int.TryParse(txtIdBusca.Text.Trim(), out int id))
            {
                var cliente = vm.ListaClientes.FirstOrDefault(c => c.Id == id);
                if (cliente != null)
                {
                    vm.ClienteSelecionado = cliente;
                    txtTelefone.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
                   // txtcrianca.GetBindingExpression(ComboBox.TextProperty)?.UpdateTarget();
                   // txtcrianca.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateTarget();
                }
                else
                {
                    MessageBox.Show("Cliente com esse ID não encontrado.");
                }
            }
        }


    }
}
