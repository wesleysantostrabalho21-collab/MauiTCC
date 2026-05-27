using SQLite;
using System;

namespace MauiTCC.Models
{
    public class Agendamento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int IdPaciente { get; set; }

        [Indexed, NotNull]
        public int IdDentista { get; set; }

        [NotNull]
        public string NomePaciente { get; set; } // Facilita a exibição rápida nos cards da agenda

        [NotNull]
        public string NomeDentista { get; set; }  // Facilita a exibição rápida nos cards da agenda

        [NotNull]
        public DateTime Data { get; set; }

        [NotNull]
        public string Horario { get; set; } // Formato "09:00", "14:30", etc.

        [NotNull]
        public string SalaCadeira { get; set; } // Ex: "Consultório A", "Cadeira 02"

        [NotNull]
        public string Status { get; set; } // "Agendado", "Remarcado", "Cancelado"
    }
}