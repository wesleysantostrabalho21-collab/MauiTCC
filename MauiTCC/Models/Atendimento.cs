using SQLite;

namespace MauiTCC.Models;

public class Atendimento
{
    [PrimaryKey, AutoIncrement]
    public int IdAtendimento { get; set; }
    [NotNull]
    public DateTime DataAtendimento { get; set; }
    [NotNull]
    public string Diagnostico { get; set; }
    public string Observacoes { get; set; }
    [NotNull]
    public string ProcedimentoRealizado { get; set; }
    [Indexed]
    public int IdAgendamento { get; set; }
}