using SQLite;

namespace MauiTCC.Models;

public class Paciente
{
    [PrimaryKey]
    public int IdUsuario { get; set; }
    public string Convenio { get; set; }
}