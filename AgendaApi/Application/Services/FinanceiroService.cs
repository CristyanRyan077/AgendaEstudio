using AgendaApi.Infra;
using AgendaApi.Models;
using AgendaShared;
using AgendaShared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AgendaApi.Application.Services
{
    public interface IFinanceiroService
    {

        Task<FinanceiroResumo> CalcularKpisAsync(FinanceiroFiltroRequest filtro);
        Task<List<RecebivelDTO>> ListarEmAbertoAsync(FinanceiroFiltroRequest filtro);
        Task<List<ServicoFaturamentoDTO>> ResumoPorServicoAsync(FinanceiroFiltroRequest filtro);
        public Task<List<ProdutoResumoVM>> ResumoPorProdutoAsync(FinanceiroFiltroRequest filtro);
    }
    public class FinanceiroService : IFinanceiroService
    {
        private readonly AgendaContext _db;
        public FinanceiroService(AgendaContext context)
        {
            _db = context;
        }
        private IQueryable<Agendamento> GetQueryBase(FinanceiroFiltroRequest filtro)
        {
            var baseQ = _db.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Servico)
                .AsNoTracking()
                .Where(a => a.Data >= filtro.Inicio && a.Data <= filtro.FimInclusivo); // Usa FimInclusivo

            if (filtro.ServicoId.HasValue) baseQ = baseQ.Where(a => a.ServicoId == filtro.ServicoId.Value);
            if (filtro.Status.HasValue) baseQ = baseQ.Where(a => a.Status == filtro.Status.Value);

            if (!string.IsNullOrWhiteSpace(filtro.ClienteNome))
            {
                var nome = filtro.ClienteNome.Trim().ToLower();
                // Nota: Se não houver Include(a => a.Cliente) no baseQ, isso pode dar erro.
                // Para segurança, certifique-se que o Include está presente OU mude a query para Left Join.
                baseQ = baseQ.Where(a => a.Cliente.Nome.ToLower().Contains(nome));
            }

            return baseQ;
        }
        public async Task<FinanceiroResumo> CalcularKpisAsync(FinanceiroFiltroRequest filtro)
        {
            var baseQ = GetQueryBase(filtro);
            var agds = await baseQ
                .Where(a => a.Status != StatusAgendamento.Cancelado)
                .Select(a => new { a.Id, a.Valor, a.Status })
                .ToListAsync();




            var agdIds = agds.Select(x => x.Id).ToList();

            // pagamentos de serviço (AgendamentoProdutoId == null) somente para os agendamentos do período
            var pagamentosServicoList = await _db.Pagamentos.AsNoTracking()
                .Where(p => p.AgendamentoProdutoId == null && agdIds.Contains(p.AgendamentoId))
                .GroupBy(p => p.AgendamentoId)
                .Select(g => new { AgendamentoId = g.Key, PagoServico = g.Sum(x => x.Valor) })
                .ToListAsync();

            var pagamentosServicoDict = pagamentosServicoList
                .ToDictionary(x => x.AgendamentoId, x => x.PagoServico);

            // Junta em memória: para cada agendamento pega o Pago (0 se não houver)
            var kpiRaw = agds
                .Select(a => new
                {
                    Valor = a.Valor, // se a.Valor for nullable no seu modelo, troque por (a.Valor ?? 0m)
                    Pago = pagamentosServicoDict.TryGetValue(a.Id, out var p) ? p : 0m,
                    a.Status
                })
                .ToList();

            // Cálculos em memória — sem EF tentando traduzir Math.Min/ternários
            var receita = kpiRaw.Sum(x => x.Valor);
            var recebido = kpiRaw.Sum(x => x.Pago);
            var aberto = kpiRaw.Sum(x => Math.Max(0m, x.Valor - x.Pago));
            var qtd = kpiRaw.Count;

            var concluidos = kpiRaw.Where(x => x.Status == StatusAgendamento.Concluido)
                                    .Select(x => Math.Min(x.Pago, x.Valor));
            var ticketMedio = qtd > 0 ? receita / qtd : 0m;


            var pagamentosPorItemQ =
                from p in _db.Set<Pagamento>().AsNoTracking()
                where p.AgendamentoProdutoId != null
                group p by p.AgendamentoProdutoId!.Value into g
                select new { AgendamentoProdutoId = g.Key, Pago = g.Sum(x => x.Valor) };
            var validAgdsQuery = baseQ.Where(a => a.Status != StatusAgendamento.Cancelado);

            // 2) Linhas de venda (join AgendamentoProdutos x agendamentos válidos x pagamentos por item)
            var itensQ =
                from ap in _db.AgendamentoProdutos.AsNoTracking()
                join a in validAgdsQuery on ap.AgendamentoId equals a.Id
                join pg in pagamentosPorItemQ on ap.Id equals pg.AgendamentoProdutoId into pgj
                from pg in pgj.DefaultIfEmpty()
                where !filtro.ProdutoId.HasValue || ap.ProdutoId == filtro.ProdutoId.Value
                select new
                {
                    Quantidade = ap.Quantidade,
                    ValorTotal = ap.Quantidade * ap.ValorUnitario, // usar expressão (propriedade calculada não traduz)
                    Pago = pg == null ? 0m : pg.Pago
                };

            // 3) Materializa as linhas e agrega em memória (estável no EF Core)
            var itens = await itensQ.ToListAsync();

            var receitaProdutos = itens.Sum(i => Math.Min(i.Pago, i.ValorTotal));
            var qtdProdutos = itens.Sum(i => i.Quantidade);
            var ticketMedioProd = qtdProdutos > 0 ? receitaProdutos / qtdProdutos : 0m;

            // ==============================
            // Retorno unificado
            // ==============================
            return new FinanceiroResumo
            {
                ReceitaBruta = receita,
                Recebido = recebido,
                EmAberto = Math.Max(0, aberto),
                QtdAgendamentos = qtd,
                TicketMedio = Math.Round(ticketMedio, 2),

                ReceitaProdutos = receitaProdutos,
                QtdProdutos = qtdProdutos,
                TicketMedioProdutos = Math.Round(ticketMedioProd, 2)
            };
            
        }
        public Task<List<RecebivelDTO>> ListarEmAbertoAsync(FinanceiroFiltroRequest filtro)
        {
            // O GetQueryBase já aplica todos os filtros de data, serviço, status e cliente.
            var baseQ = GetQueryBase(filtro);

            var valid = baseQ.Where(a => a.Status != StatusAgendamento.Cancelado);

            var q = from a in valid
                    join s in _db.Servicos.AsNoTracking() on a.ServicoId equals s.Id into sj
                    from s in sj.DefaultIfEmpty()
                    join c in _db.Clientes.AsNoTracking() on a.ClienteId equals c.Id into cj
                    from c in cj.DefaultIfEmpty()
                    let pago = a.Pagamentos.Where(p => p.Valor != 0)
                       .Sum(p => (decimal?)p.Valor) ?? 0m
                    where a.Valor > pago
                    orderby a.Data
                    select new RecebivelDTO
                    {
                        Id = a.Id,
                        Data = a.Data,
                        Cliente = c != null ? c.Nome : null,
                        Servico = s != null ? s.Nome : "—",
                        Valor = a.Valor,
                        ValorPago = pago,
                        Status = a.Status.ToString()
                    };

            return q.ToListAsync();
        }

        public Task<List<ServicoFaturamentoDTO>> ResumoPorServicoAsync(
         FinanceiroFiltroRequest filtro)
        {
            var valid = GetQueryBase(filtro)
            .Where(a => a.Status != StatusAgendamento.Cancelado);

            var query =
                from g in
                    (from a in valid
                     let pago = a.Pagamentos.Where(p => p.Valor != 0)
                       .Sum(p => (decimal?)p.Valor) ?? 0m
                     group new { a, pago } by a.ServicoId into grp
                     select new
                     {
                         ServicoId = grp.Key,
                         Receita = grp.Sum(x => x.pago),
                         Qtd = grp.Count(),
                         TicketMedio = grp.Average(x => x.pago)
                     })
                join s in _db.Servicos.AsNoTracking() on g.ServicoId equals s.Id into sj
                from s in sj.DefaultIfEmpty()
                orderby g.Receita descending
                select new ServicoFaturamentoDTO
                {
                    Servico = s != null ? s.Nome : "—",
                    Receita = g.Receita,
                    Qtd = g.Qtd,
                    TicketMedio = g.TicketMedio
                };

            return query.ToListAsync();
        }
        public async Task<List<ProdutoResumoVM>> ResumoPorProdutoAsync(FinanceiroFiltroRequest filtro)
        {
            // BaseQ com filtros de Cliente/Status/Período
            var agdsValid = GetQueryBase(filtro)
                                 .Where(a => a.Status != StatusAgendamento.Cancelado)
                                 .Select(a => a.Id); // Projeta apenas o ID do agendamento válido



            // Pagamentos por item (AgendamentoProdutoId) agregados no servidor
            var pagamentosPorItemQ =
                from p in _db.Pagamentos.AsNoTracking() // ou _db.Set<Agendamento.Pagamento>()
                where p.AgendamentoProdutoId != null
                group p by p.AgendamentoProdutoId!.Value into g
                select new { AgendamentoProdutoId = g.Key, Pago = g.Sum(x => x.Valor) };

            // “Linhas” (itens vendidos) já com valores necessários – ainda tudo no servidor
            var itensQ =
                from ap in _db.AgendamentoProdutos.AsNoTracking()
                join agdId in agdsValid on ap.AgendamentoId equals agdId
                join pg in pagamentosPorItemQ on ap.Id equals pg.AgendamentoProdutoId into pgj
                from pg in pgj.DefaultIfEmpty()
                where !filtro.ProdutoId.HasValue || ap.ProdutoId == filtro.ProdutoId.Value
                select new
                {
                    ap.ProdutoId,
                    Quantidade = ap.Quantidade,
                    ValorTotal = ap.Quantidade * ap.ValorUnitario,        // evita propriedade não mapeada
                    Pago = pg == null ? 0m : pg.Pago
                };

            // 🔻 materializa apenas as “linhas” (tamanho = nº de AgendamentoProduto no período/filtrado)
            var itens = await itensQ.ToListAsync();

            // Busca nomes dos produtos só para os IDs presentes
            var ids = itens.Select(i => i.ProdutoId).Distinct().ToList();
            var nomePorId = await _db.Produtos.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Id, p.Nome })
                .ToDictionaryAsync(x => x.Id, x => x.Nome);

            // Agrega em memória (super simples, sem esquisitices de tradução)
            var agregados = itens
                .GroupBy(i => i.ProdutoId)
                .Select(g =>
                {
                    var receita = g.Sum(i => Math.Min(i.Pago, i.ValorTotal));
                    var qtd = g.Sum(i => i.Quantidade);
                    return new ProdutoResumoVM
                    {
                        Produto = nomePorId.TryGetValue(g.Key, out var nome) ? nome : "—",
                        Receita = receita,
                        Qtd = qtd,
                        TicketMedio = qtd > 0 ? receita / qtd : 0m
                    };
                })
                .OrderByDescending(x => x.Receita)
                .ToList();

            return agregados;
        }
    }

}
