using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.Models
{
    public partial class ClienteViewModel : ObservableObject
    {
        private readonly ClienteDto _cliente;

        public ClienteViewModel(ClienteDto cliente)
        {
            _cliente = cliente;
        }

        public int Id => _cliente.Id;
        public string Nome => _cliente.Nome;
        public string Telefone => _cliente.Telefone;
        public string? Email => _cliente.Email;
        public string? Observacao => _cliente.Observacao;

        // Se quiser propriedades derivadas
        public string NomeComId => $"{_cliente.Nome} (ID: {_cliente.Id})";
    }
}
