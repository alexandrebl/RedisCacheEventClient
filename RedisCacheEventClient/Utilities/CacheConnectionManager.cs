using StackExchange.Redis;
using System;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Gerenciador de conexão de cache
    /// </summary>
    public static class CacheConnectionManager {

        /// <summary>
        /// String de conexão
        /// </summary>
        private static string _connectionString = null;

        /// <summary>
        /// Componente de conexão
        /// </summary>
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            //verifica se a string de conexão
            if (string.IsNullOrEmpty(_connectionString)) throw new Exception("Cache connection string is empty");

            //Retorna uma conexão
            return ConnectionMultiplexer.Connect(_connectionString);
        });

        /// <summary>
        /// Abre uma conexão
        /// </summary>
        public static ConnectionMultiplexer Connection {
            get {
                //retorno
                return lazyConnection.Value;
            }
        }

        /// <summary>
        /// Define a string de conexão
        /// </summary>
        /// <param name="connectionString">string de conexão</param>
        public static void SetConnectionString(string connectionString) {
            //Define conexão
            _connectionString = connectionString;
        }
    }
}