using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.Models
{
    public partial class ClienteFormModel : ObservableObject
    {
        [ObservableProperty] private string nome = string.Empty;
        [ObservableProperty] private string telefone = string.Empty;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string? observacao;
        [ObservableProperty] private string? facebook;
        [ObservableProperty] private string? instagram;

        public CriancaFormModel? Crianca { get; set; }
    }
    public partial class CriancaFormModel : ObservableObject
    {
        [ObservableProperty] private string nome = string.Empty;
        [ObservableProperty] private int idade;
        [ObservableProperty] private IdadeUnidade unidade;
        [ObservableProperty] private Genero genero;
    }
}
