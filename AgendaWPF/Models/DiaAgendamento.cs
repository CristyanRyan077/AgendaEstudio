using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public partial class DiaAgendamento : ObservableObject
    {
        private static readonly CultureInfo ptBR = new("pt-BR");
        [ObservableProperty] public DateTime data;

        [ObservableProperty] public string nome; // "Segunda", "Terça", ...

        partial void OnDataChanged(DateTime oldValue, DateTime newValue)
        {

            var n = newValue.ToString("dddd", ptBR);
 
            Nome = char.ToUpper(n[0], ptBR) + n.Substring(1).Replace("-feira", "").Trim();
        }
        [ObservableProperty]
        private ObservableCollection<AgendamentoVM> agendamentos = new();
        public ObservableCollection<LembretesVM> Lembretes { get; } = new();

        public bool TemLembretes => Lembretes.Any();

        public DiaAgendamento()
        {
            // Sempre que mudar a coleção, avisa que TemLembretes mudou
            Lembretes.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(TemLembretes));
            };
        }
    }
}
