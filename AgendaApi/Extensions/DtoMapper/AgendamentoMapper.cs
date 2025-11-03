using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Extensions.DtoMapper
{
    public static class AgendamentoMapper
    {
        public static Agendamento ToEntity(this AgendamentoCreateDto dto) =>
         new Agendamento
         {
             ClienteId = dto.ClienteId,
             CriancaId = dto.CriancaId,
             ServicoId = dto.ServicoId,
             PacoteId = dto.PacoteId,
             Valor = dto.Valor,
             Data = dto.Data,
             Mesversario = dto.Mesversario,
             Horario = dto.Horario,
             Tema = dto.Tema,
             Status = dto.Status,
             Entrega = dto.Tipo
         };

        public static AgendamentoDto ToDto(this Agendamento entity) =>
        new AgendamentoDto
        {
            Id = entity.Id,
            ClienteId = entity.ClienteId,
            CriancaId = entity.CriancaId,
            ServicoId = entity.ServicoId,
            PacoteId = entity.PacoteId,
            Valor = entity.Valor,
            Data = entity.Data,
            Horario = entity.Horario,
            Mesversario = entity.Mesversario,
            Tema = entity.Tema,
            Tipo = entity.Entrega,
            Status = entity.Status,
            Pagamentos = entity.Pagamentos?.Select(p => p.ToDto()).ToList(),
            Etapas = entity.Etapas?.Select(e => e.ToDto()).ToList(),

            Cliente = entity.Cliente == null ? null : new ClienteResumoDto
            {
                Id = entity.Cliente.Id,
                Nome = entity.Cliente.Nome,
                Telefone = entity.Cliente.Telefone
            },
            Servico = entity.Servico == null ? null : new ServicoResumoDto
            {
                Id = entity.Servico.Id,
                Nome = entity.Servico.Nome,
            },
            Pacote = entity.Pacote == null ? null : new PacoteResumoDto
            {
                Id = entity.Pacote.Id,
                Nome = entity.Pacote.Nome,
                Valor = entity.Pacote.Valor
            },
            Crianca = entity.Crianca == null ? null : new CriancaResumoDto
            {
                Id = entity.Crianca.Id,
                Genero = entity.Crianca.Genero,
                Nome = entity.Crianca.Nome,
                Idade = entity.Crianca.Idade,
                IdadeUnidade = entity.Crianca.IdadeUnidade
            }
        };
        public static void UpdateEntity(this Agendamento ag, AgendamentoUpdateDto dto)
        {
            ag.ClienteId = dto.ClienteId;
            ag.CriancaId = dto.CriancaId;
            ag.ServicoId = dto.ServicoId;
            ag.PacoteId = dto.PacoteId;
            ag.Data = dto.Data;
            ag.Valor = dto.Valor;
            ag.Status = dto.Status;

            if (dto.Horario.HasValue) ag.Horario = dto.Horario.Value;
            if (!string.IsNullOrEmpty(dto.Tema)) ag.Tema = dto.Tema;

        }
        
    }
}
