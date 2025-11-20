using AgendaShared.DTOs;
using AgendaWPF.Models;
using AgendaWPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AgendaWPF.Models.LembretesVM;

namespace AgendaWPF.ViewModels
{
    public partial class LembretesViewModel : ObservableObject
    {
        private readonly ILembreteService _service;

        [ObservableProperty] private ObservableCollection<LembretesVM> lembretes = new();
        [ObservableProperty] private LembretesVM? lembreteSelecionado;
        [ObservableProperty]
        private LembreteEditModel? lembreteEmEdicao;
        [ObservableProperty]
        private bool isNovo;

        // [ObservableProperty] private LembreteFormModel editando = new();

        // Cliente + agendamentos
        [ObservableProperty] private int? clienteIdBusca;              // para o campo ClienteId
        [ObservableProperty] private string? clienteNome;              // só leitura na tela

        // Filtros
        [ObservableProperty] private string? filtroTexto;
        [ObservableProperty] private bool mostrarSomentePendentes = true;
        [ObservableProperty] private DateTime? periodoInicio;
        [ObservableProperty] private DateTime? periodoFim;
        

        public LembretesViewModel(ILembreteService service)
        {
            _service = service;


            PeriodoInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            PeriodoFim = PeriodoInicio.Value.AddMonths(1).AddDays(-1); ;
        }
        partial void OnPeriodoInicioChanged(DateTime? value)
        {
           _ =  CarregarAsync();
        }
        partial void OnPeriodoFimChanged(DateTime? value)
        {
            _ = CarregarAsync();
        }
        partial void OnLembreteSelecionadoChanged(LembretesVM? oldValue, LembretesVM? newValue)
        {
            Cancelar();
        } 

        public async Task CarregarAsync()
        {
            try
            {
                var filtro = new LembreteQuery
                {
                    Inicio = PeriodoInicio,
                    Fim = PeriodoFim,
                    ClienteId = (ClienteIdBusca.HasValue && ClienteIdBusca.Value > 0)
                    ? ClienteIdBusca.Value
                    : null
                };

                var resultado = await _service.ListAsync(filtro);

                Lembretes = new ObservableCollection<LembretesVM>(resultado.Select(dto => new LembretesVM(dto)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar lembretes: " + ex);
              
            }
        }
        [RelayCommand]
        private void Novo()
        {
            IsNovo = true;

            LembreteEmEdicao = new LembreteEditModel
            {
                DataAlvo = DateTime.Today,
                Titulo = string.Empty,
                Descricao = string.Empty,
            };
        }
        [RelayCommand]
        private void Cancelar()
        {
            // Sai do modo novo/edição
            IsNovo = false;
            if (LembreteSelecionado != null)
            {
                // Recarrega os dados do selecionado pro editor
                LembreteEmEdicao = LembreteEditModel.FromVm(LembreteSelecionado);
            }
            else
            {
                LembreteEmEdicao = null;
            }
        }
        partial void OnLembreteSelecionadoChanged(LembretesVM? value)
        {
            if (IsNovo) return; // se estou criando novo, não sobrescrevo

            if (value is null)
            {
                LembreteEmEdicao = null;
            }
            else
            {
                LembreteEmEdicao = LembreteEditModel.FromVm(value);
            }
        }
        [RelayCommand]
        private async Task SalvarAsync()
        {
            if (LembreteEmEdicao is null) return;

            if (IsNovo)
            {
                var dto = LembreteEmEdicao.ToCreateDto();
                var criado = await _service.CreateAsync(dto);

                Lembretes.Add(new LembretesVM(criado));
                IsNovo = false;
            }
           
        }

    }
}
