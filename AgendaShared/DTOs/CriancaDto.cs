using AgendaApi.Domain.Models;

namespace AgendaApi.Domain.DTOs
{
    public class CriancaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Genero Genero { get; set; }
        public int Idade { get; set; }
        public IdadeUnidade IdadeUnidade { get; set; } = IdadeUnidade.Meses;
        public string? IdadeFormatada => $"{Idade} {IdadeUnidade}";
        public int ClienteId { get; set; }
    }

    public class CriancaCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public Genero Genero { get; set; }
        public int Idade { get; set; }
        public IdadeUnidade IdadeUnidade { get; set; } = IdadeUnidade.Meses;
        public int ClienteId { get; set; }
    }

    public class CriancaUpdateDto
    {
        public string Nome { get; set; } = string.Empty;
        public Genero Genero { get; set; }
        public int Idade { get; set; }
        public IdadeUnidade IdadeUnidade { get; set; } = IdadeUnidade.Meses;
    }
}
