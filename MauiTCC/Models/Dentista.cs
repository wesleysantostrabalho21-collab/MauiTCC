using SQLite;

namespace MauiTCC.Models
{
    public class Dentista
    {
        [PrimaryKey]
        public int IdUsuario { get; set; }
        [Unique, NotNull]
        public string Cro { get; set; }
        [NotNull]
        public string Especialidade { get; set; }
    }
}