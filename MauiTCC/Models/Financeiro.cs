using SQLite;

namespace MauiTCC.Models;

public class Financeiro
{
    [PrimaryKey, AutoIncrement]
    public int IdFinanceiro { get; set; }
    [Indexed]
    public int IdUsuario { get; set; }
    public decimal ValorConsulta { get; set; }
    public decimal ValorProcedimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string StatusPagamento { get; set; }
}