using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.Models
{
    public partial class AgendamentoViewModel : ObservableObject
    {
        private readonly AgendamentoDto _agendamento;

        public AgendamentoViewModel(AgendamentoDto agendamento)
        {
            _agendamento = agendamento;
            
        }

        public int Id => _agendamento.Id;
        public DateTime Data => _agendamento.Data;
        public TimeSpan Horario => _agendamento.Horario;
        public string Status => _agendamento.Status.ToString();
        public decimal Valor => _agendamento.Valor;
        public string ClienteNome => _agendamento.Cliente?.Nome ?? "Desconhecido";
        public string ClienteTelefone => _agendamento.Cliente?.Telefone ?? "Sem telefone";

        public string HorarioFormatado => _agendamento.Horario.ToString(@"hh\:mm");

        // Se quiser expor o cliente inteiro resumido:
        public ClienteResumoDto? Cliente => _agendamento.Cliente;
    }
}
