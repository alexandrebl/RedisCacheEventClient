using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisCacheEventClient.Library {

    /// <summary>
    /// Cache connection manager
    /// </summary>
    internal static class ObserverConnectionManager {

        /// <summary>
        /// connection String
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// Connection multiplexer
        /// </summary>

        private static ConnectionMultiplexer _connection;

        /// <summary>
        /// Sync object
        /// </summary>
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Fail event
        /// </summary>
        public static event Action<ConnectionFailedEventArgs> OnFailed;

        /// <summary>
        /// Restore Event
        /// </summary>
        public static event Action<ConnectionFailedEventArgs> OnRestored;

        /// <summary>
        /// Event error
        /// </summary>
        public static event Action<Exception> OnError;

        /// <summary>
        /// Event Info
        /// </summary>
        public static event Action<string> OnInfo;

        /// <summary>
        /// Open connection
        /// </summary>
        /// <param name="isPersistence">flag that define it is persistence connection</param>
        /// <returns>connection</returns>
        public static ConnectionMultiplexer OpenConnection(bool isPersistence) {
            //Verify if exists
            if ((_connection != null) && (_connection.IsConnected)) return _connection;

            //Sync context
            lock (SyncObj) {
                try {
                    //Verify if connection exists
                    if ((_connection == null) || (_connection.IsConnected)) {
                        //Open connection
                        _connection = ConnectionMultiplexer.Connect(_connectionString);
                        //Fail event
                        _connection.ConnectionFailed += delegate (object sender, ConnectionFailedEventArgs args) {
                            //Invoke event
                            OnFailedHandle(args);
                            //Retry connection
                            if (isPersistence) _connection = RetryOpenConnection();
                        };
                        //Connection restored
                        _connection.ConnectionRestored += delegate (object sender, ConnectionFailedEventArgs args) {
                            //Invoke event
                            OnRestoredHandle(args);
                        };
                    }
                } catch (Exception ex) {
                    //Invoke error
                    OnErrorHandle(ex);
                    //Retry connection
                    if (isPersistence) return RetryOpenConnection();
                }
            }

            //Return
            return _connection;
        }

        /// <summary>
        /// Retry connection
        /// </summary>
        /// <param name="waitMilliseconds">waitting for retry</param>
        /// <returns>connection</returns>
        private static ConnectionMultiplexer RetryOpenConnection(int waitMilliseconds = 1000) {
            //Send message
            OnInfoHandle($"Waitting {waitMilliseconds} ms for retry connection");
            //Wait
            Task.Run(() => {
                //Sleep
                Thread.Sleep(waitMilliseconds);
            }).Wait();
            //Send message
            OnInfoHandle("Retry connection");
            //Open connection
            return OpenConnection(true);
        }

        /// <summary>
        /// Define connection string
        /// </summary>
        /// <param name="connectionString">connection string</param>
        public static void SetConnectionString(string connectionString) {
            //Define connection
            _connectionString = connectionString;
        }

        /// <summary>
        /// Fail event
        /// </summary>
        /// <param name="obj">object</param>
        private static void OnFailedHandle(ConnectionFailedEventArgs obj) {
            //Invoke event
            OnFailed?.Invoke(obj);
        }

        /// <summary>
        /// Restore event
        /// </summary>
        /// <param name="obj">object</param>
        private static void OnRestoredHandle(ConnectionFailedEventArgs obj) {
            //Invoke event
            OnRestored?.Invoke(obj);
        }

        /// <summary>
        /// Error event
        /// </summary>
        /// <param name="ex">excception</param>
        private static void OnErrorHandle(Exception ex) {
            //Invoke event
            OnError?.Invoke(ex);
        }

        /// <summary>
        /// Information event
        /// </summary>
        /// <param name="message">message</param>
        private static void OnInfoHandle(string message) {
            //Invoke event
            OnInfo?.Invoke(message);
        }
    }
}