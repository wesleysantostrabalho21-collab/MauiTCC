using SQLite;

namespace MauiTCC.Models
{
    public class Agendamento
    {
        [PrimaryKey, AutoIncrement]
        public int IdAgendamento { get; set; }
        [NotNull]
        public DateTime DataHora { get; set; }
        public string Situacao { get; set; }
        public string TipoServico { get; set; }
        [Indexed]
        public int IdPaciente { get; set; }
        [Indexed]
        public int IdDentista { get; set; }
    }
}