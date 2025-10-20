using System.Collections.Generic;

namespace AgendaApi.Domain.DTOs
{
    public class ServicoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool PossuiCrianca { get; set; }
        public IEnumerable<PacoteDto>? Pacotes { get; set; }
    }

    public class ServicoCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public bool PossuiCrianca { get; set; } = true;
    }

    public class ServicoUpdateDto
    {
        public string Nome { get; set; } = string.Empty;
        public bool PossuiCrianca { get; set; } = true;
    }
}