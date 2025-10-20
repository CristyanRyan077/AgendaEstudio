using AgendaApi.Domain.Models;
using AgendaApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AgendaApi.Infra;

public partial class AgendaContext : DbContext
{
    public AgendaContext()
    {
    }

    public AgendaContext(DbContextOptions<AgendaContext> options)
        : base(options)
    {
    }

    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<AgendamentoEtapa> AgendamentoEtapas { get; set; }
    public DbSet<AgendamentoProduto> AgendamentoProdutos { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Crianca> Criancas { get; set; }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Pacote> Pacotes { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AgendaLocal;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasIndex(e => e.ClienteId, "IX_Agendamentos_ClienteId");
            entity.HasIndex(e => e.CriancaId, "IX_Agendamentos_CriancaId");
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            // Relação entre Agendamento e Cliente
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Agendamentos)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação entre Agendamento e Criança 
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Crianca)
                .WithMany()
                .HasForeignKey(a => a.CriancaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação entre Agendamento e Serviço
            modelBuilder.Entity<Agendamento>()
               .HasOne(a => a.Servico)
               .WithMany()
               .IsRequired()
               .HasForeignKey(a => a.ServicoId)
               .OnDelete(DeleteBehavior.Restrict);
            // Relação entre Agendamento e Pacote
            modelBuilder.Entity<Agendamento>()
             .HasOne(a => a.Pacote)
             .WithMany()
             .IsRequired()
             .HasForeignKey(a => a.PacoteId)
             .OnDelete(DeleteBehavior.Restrict);


        });

        modelBuilder.Entity<AgendamentoProduto>(entity =>
        {
            entity.HasIndex(e => e.AgendamentoId, "IX_AgendamentoProdutos_AgendamentoId");

            entity.HasIndex(e => e.ProdutoId, "IX_AgendamentoProdutos_ProdutoId");

            entity.Property(e => e.ValorUnitario).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Agendamento).WithMany(p => p.AgendamentoProdutos).HasForeignKey(d => d.AgendamentoId);

            entity.HasOne(d => d.Produto).WithMany(p => p.AgendamentoProdutos).HasForeignKey(d => d.ProdutoId);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.TotalPagoHistorico).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPagoMesAtual).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Crianca>(entity =>
        {
            entity.HasIndex(e => e.ClienteId, "IX_Criancas_ClienteId");

            entity.HasOne(c => c.Cliente).WithMany(p => p.Criancas).HasForeignKey(c => c.ClienteId).OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Pacote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Pacotes");

            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Servico).WithMany(p => p.Pacotes)
                .HasForeignKey(d => d.ServicoId)
                .HasConstraintName("FK_Pacotes_Servicos");
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasIndex(e => e.AgendamentoId, "IX_Pagamentos_AgendamentoId");

            entity.HasIndex(e => e.DataPagamento, "IX_Pagamentos_DataPagamento");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Observacao).HasMaxLength(500);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Agendamento).WithMany(p => p.Pagamentos).HasForeignKey(d => d.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.AgendamentoId);
            entity.HasIndex(x => x.DataPagamento);
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.Property(e => e.Nome).HasMaxLength(50);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Servico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Servicos");

            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.PossuiCrianca).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
