using MauiTCC.Models;
using SQLite;

namespace MauiTCC.Services;

public class DatabaseService
{
    // Conexão assíncrona para não travar a interface do celular (RNF01)
    private SQLiteAsyncConnection _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        // Define o caminho do banco de dados no dispositivo móvel
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "consultorio.db3");

        _database = new SQLiteAsyncConnection(databasePath);

        // Criação automática de todas as tabelas baseadas no Diagrama de Classes
        await _database.CreateTablesAsync(CreateFlags.None,
            typeof(Usuario), typeof(Endereco), typeof(Paciente), typeof(Dentista),
            typeof(Agendamento), typeof(Atendimento), typeof(Prontuario), typeof(Recepcionista), typeof(Financeiro), typeof(Consulta));
    }


    // Lógica de Cadastro de Pacientes
    public async Task<int> SalvarPacienteAsync(Paciente paciente)
    {
        await Init();
        return await _database.InsertAsync(paciente);
    }

    // Lógica de Consulta de Agenda 
    public async Task<List<Agendamento>> GetAgendamentosAsync()
    {
        await Init();
        return await _database.Table<Agendamento>().ToListAsync();
    }

    // Lógica para buscar um prontuário específico
    public async Task<Prontuario> GetProntuarioAsync(int idPaciente)
    {
        await Init();
        return await _database.Table<Prontuario>()
                             .Where(p => p.IdPaciente == idPaciente)
                             .FirstOrDefaultAsync();
    }
}