using SQLite;

namespace MauiTCC.Models;

public class Endereco
{
    [PrimaryKey]
    public int IdUsuario { get; set; }
    [NotNull]
    public string Logradouro { get; set; }
    [NotNull]
    public string Numero { get; set; }
    [NotNull]
    public string Cep { get; set; }
    public string Cidade { get; set; }
}