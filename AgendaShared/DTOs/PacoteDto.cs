
namespace AgendaApi.Domain.DTOs
{
    public class PacoteDto
    {
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class PacoteCreateDto
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class PacoteUpdateDto
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}