using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Threading.Tasks;

namespace SISTEMA_DE_VENDAS_GENERICO.Data
{
    /// <summary>
    /// Classe responsável pela conexão e operações com o banco de dados
    /// Implementa padrão Singleton para garantir uma única instância de conexão
    /// </summary>
    public sealed class DatabaseConnection
    {
        #region Propriedades Privadas
        
        private static readonly object _lock = new object();
        private static DatabaseConnection _instance;
        private static string _connectionString;
        private static bool _connectionTested = false;
        
        #endregion

        #region Construtor Privado (Singleton)
        
        private DatabaseConnection() { }
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// Instância única da classe DatabaseConnection
        /// </summary>
        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new DatabaseConnection();
                    }
                }
                return _instance;
            }
        }
        
        #endregion

        #region Métodos Privados
        
        /// <summary>
        /// Obtém a string de conexão configurada no App.config
        /// Tenta primeiro LocalDB, depois SQL Server padrão
        /// </summary>
        /// <returns>String de conexão válida</returns>
        private static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                try
                {
                    // Tenta primeiro LocalDB
                    _connectionString = ConfigurationManager.ConnectionStrings["SistemaVendasConnection"]?.ConnectionString;
                    
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        // Se não encontrar, tenta SQL Server
                        _connectionString = ConfigurationManager.ConnectionStrings["SistemaVendasConnectionSQLServer"]?.ConnectionString;
                    }
                    
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        // String de conexão padrão como fallback
                        _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SistemaVendas;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
                    }
                }
                catch (Exception ex)
                {
                    LogError("Erro ao carregar string de conexão", ex);
                    _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SistemaVendas;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";
                }
            }
            return _connectionString;
        }

        /// <summary>
        /// Registra erros no log do sistema
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        /// <param name="exception">Exceção ocorrida</param>
        private static void LogError(string message, Exception exception = null)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERRO: {message}";
                if (exception != null)
                {
                    logMessage += $"\nDetalhes: {exception.Message}\nStackTrace: {exception.StackTrace}";
                }
                
                // Log no Event Viewer do Windows
                System.Diagnostics.EventLog.WriteEntry("Sistema de Vendas", logMessage, 
                    System.Diagnostics.EventLogEntryType.Error);
            }
            catch
            {
                // Se não conseguir logar, ignora silenciosamente
            }
        }
        
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Cria uma nova conexão com o banco de dados
        /// </summary>
        /// <returns>Conexão SQL configurada</returns>
        public static SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(GetConnectionString());
                return connection;
            }
            catch (Exception ex)
            {
                LogError("Erro ao criar conexão com banco de dados", ex);
                ShowUserError("Erro de Conexão", 
                    "Não foi possível conectar ao banco de dados.\n\n" +
                    "Verifique se:\n" +
                    "• O SQL Server está em execução\n" +
                    "• O banco 'SistemaVendas' existe\n" +
                    "• As configurações de conexão estão corretas");
                throw;
            }
        }

        /// <summary>
        /// Testa a conexão com o banco de dados
        /// </summary>
        /// <returns>True se a conexão foi bem-sucedida</returns>
        public static bool TestConnection()
        {
            if (_connectionTested)
                return true;

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    
                    // Testa uma consulta simples
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        command.ExecuteScalar();
                    }
                    
                    _connectionTested = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError("Falha no teste de conexão", ex);
                ShowUserError("Erro de Conexão", 
                    "Não foi possível conectar ao banco de dados.\n\n" +
                    "Detalhes técnicos:\n" + ex.Message + "\n\n" +
                    "Verifique se:\n" +
                    "• O SQL Server está em execução\n" +
                    "• O banco 'SistemaVendas' existe\n" +
                    "• As configurações de conexão estão corretas");
                return false;
            }
        }

        /// <summary>
        /// Executa uma consulta SELECT e retorna os dados
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parâmetros da consulta</param>
        /// <returns>DataTable com os resultados</returns>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("A consulta SQL não pode ser vazia", nameof(query));

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Timeout de 60 segundos para consultas complexas
                        command.CommandTimeout = 60;
                        
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        var dataTable = new DataTable();
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                        
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Erro ao executar consulta: {query}", ex);
                ShowUserError("Erro de Consulta", 
                    "Erro ao executar consulta no banco de dados.\n\n" +
                    "Se o problema persistir, contate o suporte técnico.");
                throw;
            }
        }

        /// <summary>
        /// Executa comandos INSERT, UPDATE ou DELETE
        /// </summary>
        /// <param name="query">Comando SQL</param>
        /// <param name="parameters">Parâmetros do comando</param>
        /// <returns>Número de linhas afetadas</returns>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("O comando SQL não pode ser vazio", nameof(query));

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 60;
                        
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Erro ao executar comando: {query}", ex);
                ShowUserError("Erro de Comando", 
                    "Erro ao executar operação no banco de dados.\n\n" +
                    "Se o problema persistir, contate o suporte técnico.");
                throw;
            }
        }

        /// <summary>
        /// Executa uma consulta que retorna um único valor
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parâmetros da consulta</param>
        /// <returns>Valor único retornado pela consulta</returns>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("A consulta SQL não pode ser vazia", nameof(query));

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 60;
                        
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Erro ao executar consulta escalar: {query}", ex);
                ShowUserError("Erro de Consulta", 
                    "Erro ao executar consulta no banco de dados.\n\n" +
                    "Se o problema persistir, contate o suporte técnico.");
                throw;
            }
        }

        /// <summary>
        /// Cria o banco de dados se ele não existir
        /// </summary>
        /// <returns>True se o banco foi criado ou já existe</returns>
        public static bool CreateDatabaseIfNotExists()
        {
            try
            {
                // String de conexão para o banco master
                string masterConnectionString = GetConnectionString()
                    .Replace("Initial Catalog=SistemaVendas", "Initial Catalog=master");
                
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();
                    
                    // Verifica se o banco existe
                    string checkDbQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = 'SistemaVendas'";
                    using (var command = new SqlCommand(checkDbQuery, connection))
                    {
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count == 0)
                        {
                            // Cria o banco
                            string createDbQuery = @"
                                CREATE DATABASE SistemaVendas
                                COLLATE SQL_Latin1_General_CP1_CI_AS";
                            
                            using (var createCommand = new SqlCommand(createDbQuery, connection))
                            {
                                createCommand.ExecuteNonQuery();
                            }
                            
                            ShowUserInfo("Banco Criado", 
                                "Banco de dados 'SistemaVendas' criado com sucesso!");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogError("Erro ao criar banco de dados", ex);
                ShowUserError("Erro de Criação", 
                    "Não foi possível criar o banco de dados.\n\n" +
                    "Detalhes: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Executa múltiplos comandos em uma transação
        /// </summary>
        /// <param name="commands">Lista de comandos SQL com parâmetros</param>
        /// <returns>True se todos os comandos foram executados com sucesso</returns>
        public static bool ExecuteTransaction(params (string query, SqlParameter[] parameters)[] commands)
        {
            if (commands == null || commands.Length == 0)
                throw new ArgumentException("Deve haver pelo menos um comando para executar", nameof(commands));

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var (query, parameters) in commands)
                        {
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.CommandTimeout = 60;
                                
                                if (parameters != null)
                                {
                                    command.Parameters.AddRange(parameters);
                                }
                                
                                command.ExecuteNonQuery();
                            }
                        }
                        
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogError("Erro na transação", ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Executa operações assíncronas no banco de dados
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parâmetros da consulta</param>
        /// <returns>DataTable com os resultados</returns>
        public static async Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[] parameters = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("A consulta SQL não pode ser vazia", nameof(query));

            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 60;
                        
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        var dataTable = new DataTable();
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                        
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Erro ao executar consulta assíncrona: {query}", ex);
                throw;
            }
        }
        
        #endregion

        #region Métodos de Interface com Usuário
        
        /// <summary>
        /// Exibe mensagem de erro para o usuário
        /// </summary>
        /// <param name="title">Título da mensagem</param>
        /// <param name="message">Conteúdo da mensagem</param>
        private static void ShowUserError(string title, string message)
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch
            {
                // Se não conseguir mostrar a mensagem, ignora
            }
        }

        /// <summary>
        /// Exibe mensagem informativa para o usuário
        /// </summary>
        /// <param name="title">Título da mensagem</param>
        /// <param name="message">Conteúdo da mensagem</param>
        private static void ShowUserInfo(string title, string message)
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch
            {
                // Se não conseguir mostrar a mensagem, ignora
            }
        }
        
        #endregion
    }
}