using SQLite;

namespace MauiTCC.Models;

public class Paciente : Usuario // Herança necessária para ter Nome e CPF
{
    public string Convenio { get; set; }
    public string Telefone { get; set; } // Adicionado para bater com a tela
}