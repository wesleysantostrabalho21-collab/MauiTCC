using SQLite;

namespace MauiTCC.Models;

public class Consulta
{
    [PrimaryKey, AutoIncrement]
    public int IdConsulta { get; set; }
    [Indexed]
    public int IdAgendamento { get; set; }
    public decimal Valor { get; set; }
    [NotNull]
    public DateTime DataConsulta { get; set; }
    public string Status { get; set; }
}