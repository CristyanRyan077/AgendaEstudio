
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AgendaWPF.Models
{
    public partial class DiaCalendario : ObservableObject
    {
        public DateTime Data { get; set; }

        [ObservableProperty] private ObservableCollection<AgendamentoDto> agendamentos = new();
        public ObservableCollection<LembretesVM> Lembretes { get; } = new();

        [ObservableProperty] private bool selecionado;
        public bool TemLembretes => Lembretes.Any();
        public DiaCalendario()
        {
            // Sempre que mudar a coleção, avisa que TemLembretes mudou
            Lembretes.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(TemLembretes));
            };
        }

    }

}
