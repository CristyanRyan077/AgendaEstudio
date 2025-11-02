using AgendaShared.DTOs;
using AgendaShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaWPF.Services
{
    public static class ClienteFormMap
    {
        public static ClienteCreateDto ToCreateDto(this ClienteFormModel f)
            => new ClienteCreateDto
            {
                Nome = f.Nome,
                Telefone = f.Telefone,
                Email = f.Email,
                Observacao = f.Observacao,
                Crianca = TemCriancaPreenchida(f)
                ? new CriancaCreateDto
                {
                    Genero = f.Crianca!.Genero,
                    Nome = f.Crianca!.Nome,
                    Idade = f.Crianca!.Idade,
                    IdadeUnidade = f.Crianca!.Unidade
                }
                : null
            };

        public static ClienteUpdateDto ToUpdateDto(this ClienteFormModel f, int id)
            => new ClienteUpdateDto
            {
                Nome = f.Nome,
                Telefone = f.Telefone,
                Email = f.Email,
                Observacao = f.Observacao,
                Crianca = TemCriancaPreenchida(f)
                ? new CriancaUpdateDto
                {
                    Genero = f.Crianca!.Genero,
                    Nome = f.Crianca!.Nome,
                    Idade = f.Crianca!.Idade,
                    IdadeUnidade = f.Crianca!.Unidade
                }
                : null
            };

            public static CriancaCreateDto ToCreateDto(this CriancaFormModel f) => new CriancaCreateDto
            {
                Nome = f.Nome,
                Genero = f.Genero,
                Idade = f.Idade,
                IdadeUnidade = f.Unidade
            };

        private static bool TemCriancaPreenchida(ClienteFormModel f)
            => f.Crianca is not null && !string.IsNullOrWhiteSpace(f.Crianca.Nome);
    }
}
