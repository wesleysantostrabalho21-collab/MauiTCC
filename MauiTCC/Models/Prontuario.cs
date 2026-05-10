using SQLite;

namespace MauiTCC.Models;

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
}