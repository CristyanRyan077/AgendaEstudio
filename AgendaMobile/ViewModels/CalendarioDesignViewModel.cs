using AgendaMobile.Helpers;
using AgendaMobile.Models;
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.ViewModels
{
    internal class CalendarioDesignViewModel
    {
        public ObservableCollection<ClienteViewModel> Clientes { get; } = new();
        public ObservableCollection<AgendamentoViewModel> Agendamentos { get; } = new();

        public ObservableCollection<AgendamentoPorDia> Dias;
        public ObservableCollection<AgendamentoDto> AgendamentosSelecionados = new();
        public CalendarioDesignViewModel()
        {
        }
    }
}
