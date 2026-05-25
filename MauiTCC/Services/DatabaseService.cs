using MauiTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MauiTCC.Services
{
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

        #region LOGICA DE PACIENTES

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

        #endregion

        #region LOGICA DE USUÁRIOS E CONTROLE DE ACESSO (NOVO)

        /// <summary>
        /// Insere um usuário comum no sistema (Administrador ou Recepcionista).
        /// </summary>
        public async Task<int> SalvarUsuarioAsync(Usuario usuario)
        {
            await Init();
            return await _database.InsertAsync(usuario);
        }

        /// <summary>
        /// Cadastra o Usuário e os dados específicos do Dentista de forma vinculada.
        /// </summary>
        public async Task<bool> SalvarDentistaCompletoAsync(Usuario novoUsuario, Dentista novoDentista)
        {
            await Init();

            try
            {
                // Força o tipo correto antes de salvar na tabela geral
                novoUsuario.Tipo = "Dentista";

                // 1. Salva o usuário na tabela geral para gerar o ID de login
                int linhasAfetadas = await _database.InsertAsync(novoUsuario);

                if (linhasAfetadas > 0)
                {
                    // Vincula o IdUsuario do Dentista com o ID gerado pelo banco para o Usuário
                    novoDentista.IdUsuario = novoUsuario.Id;

                    // 2. Salva os dados do CRO e Especialidade na tabela Dentista
                    await _database.InsertAsync(novoDentista);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao salvar dentista completo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valida o login e retorna o Usuário completo com o seu "Tipo" de acesso.
        /// </summary>
        public async Task<Usuario> ValidarLoginComNivelAsync(string cpf, string senha)
        {
            await Init();
            try
            {
                // Busca na tabela geral de Usuários para dar acesso a Admin, Dentistas e Recepcionistas
                var usuario = await _database.Table<Usuario>()
                                             .Where(u => u.CPF == cpf && u.Senha == senha)
                                             .FirstOrDefaultAsync();
                return usuario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao validar login: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Método antigo ajustado caso alguma outra parte do seu código dependa dele.
        /// </summary>
        public async Task<bool> ValidarLoginAsync(string cpf, string senha)
        {
            var user = await ValidarLoginComNivelAsync(cpf, senha);
            return user != null;
        }

        #endregion

        #region LOGICA DE AGENDAMENTOS E PRONTUÁRIOS

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

        #endregion
    }
}