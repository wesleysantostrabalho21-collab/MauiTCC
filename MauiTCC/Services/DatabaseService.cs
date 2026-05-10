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
        // Importante: CreateTablesAsync cria a tabela APENAS se ela não existir
        await _database.CreateTablesAsync<Usuario, Endereco, Paciente, Dentista>();
        await _database.CreateTablesAsync<Agendamento, Atendimento, Prontuario, Recepcionista>();
        await _database.CreateTablesAsync<Financeiro, Consulta>();
    }


    // Lógica de Cadastro de Pacientes
    public async Task<int> SalvarPacienteAsync(Paciente paciente)
    {
        await Init();
        return await _database.InsertAsync(paciente);
    }

    // --- TESTE VERIFICAÇÃO ---
    public async Task<List<Paciente>> GetPacientesAsync()
    {
        await Init();
        return await _database.Table<Paciente>().ToListAsync();
    }
    // ---------------------------------------------------------------

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

    public async Task<bool> ValidarLoginAsync(string cpf, string senha)
    {
        await Init();
        // Procura um usuário que tenha o nome E a senha iguais aos digitados
        var usuario = await _database.Table<Paciente>()
                                     .Where(u => u.CPF == cpf && u.Senha == senha)
                                     .FirstOrDefaultAsync();

        return usuario != null; // Retorna true se achou, false se não achou
    }
}