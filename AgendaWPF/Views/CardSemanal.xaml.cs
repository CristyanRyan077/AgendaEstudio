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
        private readonly IServiceProvider _sp;
        public AgendaViewModel vm { get; }
        public CardSemanal(AgendaViewModel agendaVm, IServiceProvider sp)
        {
            InitializeComponent();
            vm = agendaVm;
            DataContext = vm;
            _sp = sp;
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

            if (DataContext is AgendaViewModel vm)
            {
                GetAgendar();
            }
        }
    }
}
