using SQLite;

namespace MauiTCC.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public string Nome { get; set; }
    [NotNull]
    public DateTime DataNascimento { get; set; }
    [Unique, NotNull]
    public string Cpf { get; set; }
    [NotNull]
    public string Senha { get; set; }
    public string Telefone { get; set; }
    [NotNull]
    public string TipoUsuario { get; set; }
}
