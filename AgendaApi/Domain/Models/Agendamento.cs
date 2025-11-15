
using AgendaShared;
using AgendaShared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaApi.Models;

public partial class Agendamento
{
    public int Id { get; set; }

    //----------------------------------------------------//
    //            FKs
    //----------------------------------------------------//
    public int ClienteId { get; set; }
    public int? CriancaId { get; set; }
    public int PacoteId { get; set; }
    public int ServicoId { get; set; }
    //----------------------------------------------------//
    //            Colunas
    //----------------------------------------------------//
    public TimeSpan? Horario { get; set; }
    [MaxLength(100)] public string? Tema { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int? Mesversario { get; set; }
    [MaxLength(255)] public string? Observacao { get; set; }
    //----------------------------------------------------//
    //             Enums
    //----------------------------------------------------//
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Pendente;
    public TipoEntrega Entrega { get; set; } = TipoEntrega.Foto;


    //          Collections
    //----------------------------------------------------//
    public ICollection<AgendamentoProduto> AgendamentoProdutos { get; set; } = new List<AgendamentoProduto>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    public ICollection<AgendamentoEtapa> Etapas { get; } = new List<AgendamentoEtapa>();
    public ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    public ICollection<Pacote> Pacotes { get; set; } = new List<Pacote>();
    public ICollection<Crianca> Criancas { get; set; } = new List<Crianca>();
    //----------------------------------------------------//
    //       Propiedades de Navegação
    //----------------------------------------------------//
    public Cliente Cliente { get; set; } = null!;

    public Crianca? Crianca { get; set; }

    public Pacote Pacote { get; set; } = null!;

    public Servico Servico { get; set; } = null!;
    //----------------------------------------------------//

    [NotMapped]
    public bool EstaPago => (Pagamentos?.Sum(p => p.Valor) ?? 0m) >= Valor;
}
