using SQLite;

namespace MauiTCC.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string Senha { get; set; }
    public string Tipo { get; set; }
}