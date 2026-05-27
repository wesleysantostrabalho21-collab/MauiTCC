using SQLite;
using System;

namespace MauiTCC.Models
{
    public class Prontuario
    {
        [PrimaryKey, AutoIncrement]
        public int IdProntuario { get; set; }

        [NotNull]
        public DateTime DataAbertura { get; set; }

        [NotNull]
        public string StatusGeral { get; set; }

        [Indexed]
        public int IdPaciente { get; set; }

        [Indexed]
        public int IdAtendimento { get; set; }

        // 🌟 Propriedades necessárias para conversar com a sua ProntuarioClinicoPage:
        public string NomePaciente { get; set; }
        public string HistoricoTratamentos { get; set; }
        public string ProcedimentosRealizados { get; set; }
        public string CaminhoAnexoDocumento { get; set; }
        public DateTime UltimaAtualizacao { get; set; } // O campo que estava faltando!
    }
}