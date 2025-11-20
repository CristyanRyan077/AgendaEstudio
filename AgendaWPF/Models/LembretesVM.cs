using AgendaShared;
using AgendaShared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Models
{
    public partial class LembretesVM : ObservableObject
    {
        private LembreteDto _dto;
        public int Id => _dto.Id;
        public int? ClienteId => _dto.ClienteId;
        public string? ClienteNome => _dto.ClienteNome;
        public int? AgendamentoId => _dto.AgendamentoId;

        public string? Descricao => _dto.Descricao;
        public string? NomeComId => $"{ClienteNome} (Id:{ClienteId})";
        public string? CaminhoImagem => _dto.CaminhoImagem;


        [ObservableProperty]
        private bool concluido;
        [ObservableProperty]
        private string titulo;

        // O 'DataAlvo' também deve ser editável, se necessário
        [ObservableProperty]
        private DateTime dataAlvo;

        public LembretesVM(LembreteDto dto)
        {
            _dto = dto;
            Titulo = dto.Titulo;
            DataAlvo = dto.DataAlvo;
            Concluido = dto.Status == LembreteStatus.Concluido;
        }
        public partial class LembreteEditModel : ObservableObject
        {
            [ObservableProperty] private int id;
            [ObservableProperty] private DateTime dataAlvo;
            [ObservableProperty] private string titulo = string.Empty;
            [ObservableProperty] private string descricao = string.Empty;
            [ObservableProperty] private bool concluido;
            [ObservableProperty] private string? clienteNome;
            [ObservableProperty] private string? caminhoImagem;
        

            public static LembreteEditModel FromVm(LembretesVM vm) =>
                new()
                {
                    Id = vm.Id,
                    DataAlvo = vm.DataAlvo,
                    Titulo = vm.Titulo,
                    Descricao = vm.Descricao ?? string.Empty,
                    Concluido = vm.Concluido,
                    ClienteNome = vm.ClienteNome,
                    CaminhoImagem = vm.CaminhoImagem,
                };
            public LembreteCreateDto ToCreateDto() => new()
            {
                DataAlvo = DataAlvo,
                Titulo = Titulo,
                Descricao = Descricao ?? string.Empty,
                Status = Concluido ? LembreteStatus.Concluido : LembreteStatus.Pendente,
                CaminhoImagem = CaminhoImagem
            };
        }



    }
}
