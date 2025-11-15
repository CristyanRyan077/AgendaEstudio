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

namespace AgendaWPF.Controles
{
    /// <summary>
    /// Lógica interna para AdicionarPagamento.xaml
    /// </summary>
    public partial class AdicionarPagamento : Window
    {
        public PagamentosViewModel _vm { get; }
        public AdicionarPagamento(PagamentosViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = _vm;
            Loaded += async (_, __) => await vm.CarregarAsync();
            if (DataContext is PagamentosViewModel form)
                form.RequestClose += (_, __) => Close();
        }
    }
}
