using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisCacheEventClient.Library {

    /// <summary>
    /// Gerenciador de conexão de cache
    /// </summary>
    public static class ObserverConnectionManager {

        /// <summary>
        /// String de conexão
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// Componente de conexão
        /// </summary>

        private static ConnectionMultiplexer _connection;

        /// <summary>
        /// Objeto de sincronismo
        /// </summary>
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Evento
        /// </summary>
        public static event Action<ConnectionFailedEventArgs> OnFailed;

        /// <summary>
        /// Evento
        /// </summary>
        public static event Action<ConnectionFailedEventArgs> OnRestored;

        /// <summary>
        /// Evento
        /// </summary>
        public static event Action<Exception> OnError;

        /// <summary>
        /// Evento
        /// </summary>
        public static event Action<string> OnInfo;

        /// <summary>
        /// Abre uma conexão
        /// </summary>
        /// <param name="isPersistence">indica se a conexão é persistente</param>
        /// <returns>Retorna uma conexão</returns>
        public static ConnectionMultiplexer OpenConnection(bool isPersistence) {
            //Verifica se a conexão existe
            if ((_connection != null) && (_connection.IsConnected)) return _connection;

            //Sincronismo
            lock (SyncObj) {
                try {
                    //Verifica se a conexão existe
                    if ((_connection == null) || (_connection.IsConnected)) {
                        //Abre a conexão
                        _connection = ConnectionMultiplexer.Connect(_connectionString);
                        //Evento de falha
                        _connection.ConnectionFailed += delegate (object sender, ConnectionFailedEventArgs args) {
                            //Dispara o evento
                            OnFailedHandle(args);
                            //Retenta a conexão
                            if (isPersistence) _connection = RetryOpenConnection();
                        };
                        //Evento de conexão reestabelecida
                        _connection.ConnectionRestored += delegate (object sender, ConnectionFailedEventArgs args) {
                            //Dispara o evento
                            OnRestoredHandle(args);
                        };
                    }
                } catch (Exception ex) {
                    //Dispara o erro
                    OnErrorHandle(ex);
                    //Retenta a conexão
                    if (isPersistence) return RetryOpenConnection();
                }
            }

            //Retorno
            return _connection;
        }

        /// <summary>
        /// Retenta a conexão
        /// </summary>
        /// <param name="waitMilliseconds">tempo de aguardo até a próxima retentativa</param>
        /// <returns>conexão</returns>
        private static ConnectionMultiplexer RetryOpenConnection(int waitMilliseconds = 1000) {
            //Envia mensagem
            OnInfoHandle($"Waitting {waitMilliseconds} ms for retry connection");
            //Efetua pausa
            Task.Run(() => {
                //Pausa
                Thread.Sleep(waitMilliseconds);
            }).Wait();
            //Evia mensagem
            OnInfoHandle("Retry connection");
            //Abre a conexão
            return OpenConnection(true);
        }

        /// <summary>
        /// Define a string de conexão
        /// </summary>
        /// <param name="connectionString">string de conexão</param>
        public static void SetConnectionString(string connectionString) {
            //Define conexão
            _connectionString = connectionString;
        }

        /// <summary>
        /// Evento de falha
        /// </summary>
        /// <param name="obj">objeto</param>
        private static void OnFailedHandle(ConnectionFailedEventArgs obj) {
            //Dispara evento
            OnFailed?.Invoke(obj);
        }

        /// <summary>
        /// Restaura dados
        /// </summary>
        /// <param name="obj">objeto</param>
        private static void OnRestoredHandle(ConnectionFailedEventArgs obj) {
            //Dispara evento
            OnRestored?.Invoke(obj);
        }

        /// <summary>
        /// Evento de erro
        /// </summary>
        /// <param name="ex">exceção</param>
        private static void OnErrorHandle(Exception ex) {
            //Dispara evento
            OnError?.Invoke(ex);
        }

        /// <summary>
        /// Evento de informação
        /// </summary>
        /// <param name="message">mensagem</param>
        private static void OnInfoHandle(string message) {
            //Dispara evento
            OnInfo?.Invoke(message);
        }
    }
}