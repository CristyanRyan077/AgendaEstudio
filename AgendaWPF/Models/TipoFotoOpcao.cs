using AgendaShared;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public class TipoFotoOpcao : ObservableObject
    {
        public TipoEntrega Valor { get; }
        public string Nome { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public TipoFotoOpcao(TipoEntrega valor)
        {
            Valor = valor;
            Nome = valor.ToString(); // Ou uma lógica de formatação amigável
        }
    }
}
