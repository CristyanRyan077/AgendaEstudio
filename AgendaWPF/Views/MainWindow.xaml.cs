using AgendaWPF.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgendaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _sp;
        public MainViewModel viewmodel { get; }
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            viewmodel = vm;
            DataContext = viewmodel;

        }
    }
}