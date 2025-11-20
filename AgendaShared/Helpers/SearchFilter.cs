using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaShared.Helpers
{
    public class AgendamentoSearchFilter
    {
        // Termo geral para nome ou telefone
        public string? SearchTerm { get; set; }

        // Lista de tipos de foto selecionados (vindo da checklist/radio button)
        // Se for um Radio Button, poderia ser apenas TipoFoto? TipoFotoSelecionado { get; set; }
        public List<TipoEntrega>? TiposDeFotoSelecionados { get; set; }

        // Campo para buscar por ID (Ex: "#123")
        public int? ClienteId { get; set; }
    }
}
