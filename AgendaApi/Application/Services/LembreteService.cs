using AgendaApi.Domain.Models;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Infra;
using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AgendaApi.Application.Services
{
    public interface ILembreteService
    {
        Task<LembreteDto> CreateAsync(LembreteCreateDto dto, CancellationToken ct = default);
        Task<LembreteDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<LembreteDto>> ListAsync(LembreteQuery filtro, CancellationToken ct = default);

        Task<LembreteDto?> UpdateAsync(int id, LembreteCreateDto dto, CancellationToken ct = default);

        Task ConcluirAsync(int id, CancellationToken ct = default);
        Task MarcarIgnoradoAsync(int id, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
    public class LembreteService : ILembreteService
    {
        private readonly AgendaContext _context;
        public LembreteService(AgendaContext context) => _context = context;

        private IQueryable<Lembrete> GetQueryBase(LembreteQuery filtro)
        {
            var query = _context.Lembretes
         .AsNoTracking()
         .AsQueryable();

            // 1) Intervalo de datas
            if (filtro.Inicio.HasValue)
            {
                var ini = filtro.Inicio.Value.Date; // zera hora
                query = query.Where(l => l.DataAlvo >= ini);
            }

            if (filtro.Fim.HasValue)
            {
                // Se quiser incluir o dia inteiro de Fim:
                var fimExclusivo = filtro.Fim.Value.Date.AddDays(1);
                query = query.Where(l => l.DataAlvo < fimExclusivo);
            }

            // 2) Filtro por IDs
            if (filtro.AgendamentoId.HasValue)
                query = query.Where(a => a.AgendamentoId == filtro.AgendamentoId.Value);

            if (filtro.ClienteId.HasValue)
                query = query.Where(a => a.ClienteId == filtro.ClienteId.Value);

            // 3) Status
            if (filtro.Status.HasValue)
                query = query.Where(a => a.Status == filtro.Status.Value);

            return query;
        }
        public async Task<LembreteDto> CreateAsync(LembreteCreateDto dto, CancellationToken ct = default)
        {
            var entity = dto.ToEntity();
            entity.CreatedAt = DateTime.UtcNow;
            entity.Status = dto.Status == default ? LembreteStatus.Pendente : dto.Status;

            _context.Lembretes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
        }
        public async Task<LembreteDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Lembretes
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, ct);

            return entity?.ToDto();
        }
        public async Task<IReadOnlyList<LembreteDto>> ListAsync(LembreteQuery filtro, CancellationToken ct = default)
        {
            var query = GetQueryBase(filtro).Include(q => q.Cliente);
            var countAntes = await query.CountAsync(ct);
            Console.WriteLine($"[LembreteService] Count após filtros = {countAntes}");
            var itens = await query
                .OrderBy(l => l.DataAlvo)
                .ThenBy(l => l.Id)
                .Select(l => l.ToDto())
                .ToListAsync(ct);

            return itens;
        }
        public async Task<LembreteDto?> UpdateAsync(int id, LembreteCreateDto dto, CancellationToken ct = default)
        {
            var entity = await _context.Lembretes.FirstOrDefaultAsync(l => l.Id == id, ct);
            if (entity is null) return null;

            // Atualiza campos permitidos
            entity.Titulo = dto.Titulo;
            entity.Descricao = dto.Descricao;
            entity.DataAlvo = dto.DataAlvo;
            entity.ClienteId = dto.ClienteId;
            entity.AgendamentoId = dto.AgendamentoId;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
        }
        public async Task ConcluirAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Lembretes.FirstOrDefaultAsync(l => l.Id == id, ct);
            if (entity is null) return;

            entity.Status = LembreteStatus.Concluido;
            entity.ConcluidoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        public async Task MarcarIgnoradoAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Lembretes.FirstOrDefaultAsync(l => l.Id == id, ct);
            if (entity is null) return;

            entity.Status = LembreteStatus.Ignorado;
            entity.ConcluidoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await  _context.Lembretes.FirstOrDefaultAsync(l => l.Id == id, ct);
            if (entity is null) return;

            _context.Lembretes.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

    }
}
