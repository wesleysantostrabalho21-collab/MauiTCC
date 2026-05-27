using MauiTCC.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Adicionado para suportar o .Any() e .Where() na memória
using System.Threading.Tasks;

namespace MauiTCC.Services
{
    public class DatabaseService
    {
        // Conexão assíncrona para não travar a interface do celular/computador (RNF01)
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database is not null)
                return;

            // Define o caminho do banco de dados no dispositivo
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

        // Recupera a lista de todos os pacientes cadastrados
        public async Task<List<Paciente>> GetPacientesAsync()
        {
            await Init();
            return await _database.Table<Paciente>().ToListAsync();
        }

        #endregion

        #region LOGICA DE USUÁRIOS E CONTROLE DE ACESSO

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

        // Salvar ou atualizar um agendamento (Resolve agendar e remarcar)
        public async Task<int> SalvarAgendamentoAsync(Agendamento agendamento)
        {
            await Init();
            if (agendamento.Id != 0)
            {
                return await _database.UpdateAsync(agendamento);
            }
            else
            {
                agendamento.Status = "Agendado";
                return await _database.InsertAsync(agendamento);
            }
        }

        // Deleta ou Cancela uma consulta (Para o botão cancelar)
        public async Task<int> CancelarAgendamentoAsync(Agendamento agendamento)
        {
            await Init();
            agendamento.Status = "Cancelado";
            return await _database.UpdateAsync(agendamento);
        }

        // Lista TODOS os agendamentos ativos (Ignorando os cancelados)
        public async Task<List<Agendamento>> GetAgendamentosAsync()
        {
            await Init();
            return await _database.Table<Agendamento>()
                                 .Where(a => a.Status != "Cancelado")
                                 .ToListAsync();
        }

        // Filtra a agenda por um Dentista específico
        public async Task<List<Agendamento>> GetAgendamentosPorDentistaAsync(int idDentista)
        {
            await Init();
            return await _database.Table<Agendamento>()
                                 .Where(a => a.IdDentista == idDentista && a.Status != "Cancelado")
                                 .ToListAsync();
        }

        // 🌟 ATUALIZADO COM VERIFICAÇÃO ULTRA SEGURA (Evita falhas de tabelas diferentes)
        public async Task<List<Agendamento>> GetAgendamentosPorPacienteAsync(int idPaciente, string nomePaciente)
        {
            await Init();

            // 1. Busca todos os agendamentos ativos da tabela para filtrar em memória (onde métodos de string funcionam perfeitamente)
            var todosAgendamentos = await _database.Table<Agendamento>()
                                                   .Where(a => a.Status != "Cancelado")
                                                   .ToListAsync();

            // 2. Compara pelo ID do Paciente ou se os nomes batem de alguma forma (ignorando maiúsculas/minúsculas)
            return todosAgendamentos.Where(a =>
                a.IdPaciente == idPaciente ||
                (!string.IsNullOrEmpty(a.NomePaciente) &&
                 (a.NomePaciente.Equals(nomePaciente, StringComparison.OrdinalIgnoreCase) ||
                  nomePaciente.Contains(a.NomePaciente, StringComparison.OrdinalIgnoreCase)))
            ).ToList();
        }

        // Filtra a agenda por uma Data específica
        public async Task<List<Agendamento>> GetAgendamentosPorDataAsync(DateTime data)
        {
            await Init();

            var dataDesejada = data.Date;
            var todos = await _database.Table<Agendamento>()
                                       .Where(a => a.Status != "Cancelado")
                                       .ToListAsync();

            return todos.Where(a => a.Data.Date == dataDesejada).ToList();
        }

        // Regra de Negócio (RNF): Verifica se o dentista ou a sala já estão ocupados naquele mesmo dia e horário
        public async Task<bool> VerificarDisponibilidadeAsync(DateTime data, string horario, string sala, int idDentista, int idAgendamentoAtual = 0)
        {
            await Init();
            var todos = await _database.Table<Agendamento>()
                                       .Where(a => a.Status != "Cancelado")
                                       .ToListAsync();

            bool conflito = todos.Any(a =>
                a.Id != idAgendamentoAtual &&
                a.Data.Date == data.Date &&
                a.Horario == horario &&
                (a.IdDentista == idDentista || a.SalaCadeira == sala)
            );

            return !conflito; // Retorna TRUE se estiver livre e FALSE se estiver ocupado
        }

        // Método auxiliar para carregar a lista de Dentistas nos Pickers da tela
        public async Task<List<Usuario>> GetTodosDentistasAsync()
        {
            await Init();
            return await _database.Table<Usuario>()
                                 .Where(u => u.Tipo == "Dentista")
                                 .ToListAsync();
        }

        // Lógica para buscar um prontuário específico
        public async Task<Prontuario> GetProntuarioAsync(int idPaciente)
        {
            await Init();
            return await _database.Table<Prontuario>()
                                 .Where(p => p.IdPaciente == idPaciente)
                                 .FirstOrDefaultAsync();
        }
        public async Task<int> SalvarProntuarioAsync(Prontuario prontuario)
        {
            await Init();

            // Se o IdProntuario for diferente de 0, significa que o registro já existe, então atualiza
            if (prontuario.IdProntuario != 0)
            {
                return await _database.UpdateAsync(prontuario);
            }
            // Caso contrário, é um prontuário novo sendo gerado para o paciente
            else
            {
                return await _database.InsertAsync(prontuario);
            }
        }

        #endregion
    }
}