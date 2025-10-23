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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgendaWPF.Views
{
    /// <summary>
    /// Interação lógica para CardSemanal.xam
    /// </summary>
    public partial class CardSemanal : UserControl
    {
        private readonly IServiceProvider _sp;
        public AgendaViewModel vm { get; }
        public CardSemanal(AgendaViewModel agendaVm)
        {
            InitializeComponent();
            vm = agendaVm;
            DataContext = vm;
            Loaded += async (_, __) => await vm.InicializarAsync();
        }
    }
}
