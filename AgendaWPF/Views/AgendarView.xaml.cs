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
        public AgendaViewModel viewmodel { get; }
        public AgendarView(AgendaViewModel vm)
        {
            InitializeComponent();
            viewmodel = vm;
            DataContext = vm;
        }
    }
}
