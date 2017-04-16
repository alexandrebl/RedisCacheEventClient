using RedisCacheEventClient.Library;
using RedisCacheEventClient.Utilities.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Utility event manager
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    internal class ObserverManagerUtility<T> : IObserverManagerUtility<T> {

        /// <summary>
        /// Connection
        /// </summary>
        private readonly ConnectionMultiplexer _connection;

        /// <summary>
        /// Receive event
        /// </summary>
        public event Action<T, string> OnReceiveMessage;

        /// <summary>
        /// Send message event
        /// </summary>
        public event Action<T> OnSendMessage;

        /// <summary>
        /// Error event
        /// </summary>
        public event Action<Exception> OnErrorMessage;

        /// <summary>
        /// Information event
        /// </summary>
        public event Action<string> OnInfoMessage;

        /// <summary>
        /// Connection event
        /// </summary>
        public event Action<string, ConnectionFailedEventArgs> OnConnectionMessage;

        /// <summary>
        /// Sync object
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="host">server conection string</param>
        public ObserverManagerUtility(string host) {
            //Define connection
            ObserverConnectionManager.SetConnectionString(host);
            //Open connection
            _connection = ObserverConnectionManager.OpenConnection(true);

            //Connection fail
            ObserverConnectionManager.OnFailed += delegate (ConnectionFailedEventArgs args) {
                //Invoke event
                OnConnectionHandle("Connection error", args);
            };

            //Connection Restored
            ObserverConnectionManager.OnRestored += delegate (ConnectionFailedEventArgs args) {
                //Invoke event
                OnConnectionHandle("Connection restored", args);
            };

            //Error event
            ObserverConnectionManager.OnError += delegate (Exception exception) {
                //Invoke event
                OnErrorMessageHadle(new Exception($"Connection error. Exception: {exception}"));
            };

            //Information event
            ObserverConnectionManager.OnInfo += OnInfoMessageHandle;
        }

        /// <summary>
        /// flag that indicate is connected
        /// </summary>
        /// <returns>success flag</returns>
        public bool IsConnected() {
            //Sync context
            lock (SyncObj) {
                //Return flag data
                return _connection?.IsConnected ?? false;
            }
        }

        /// <summary>
        /// Subscribe stream channel
        /// </summary>
        /// <param name="channel">stream channel</param>
        public void Subscribe(string channel) {
            //Get channel subscriber
            var subscriber = GetSubscriber();

            try {
                //Subscribe channel
                subscriber?.Subscribe(channel, (redisChannel, message) => {
                    //Convert object
                    var obj = ObserverDataUtility<T>.RedisValueToObj(message);
                    //Invoke event
                    OnReceiveMessageHadle(obj, channel);
                });

                //Unsubscribe channel
                subscriber?.Unsubscribe(channel, (redisChannel, value) => {
                    //Subscribe channel
                    Subscribe(channel);
                });
            } catch (Exception ex) {
                //Invoke event
                OnErrorMessageHadle(ex);
                //Waitting for 1 second
                Task.Run(() => { Thread.Sleep(1000); });
                //Subscribe channel
                Subscribe(channel);
            }
        }

        /// <summary>
        /// Publish message
        /// </summary>
        /// <param name="channel">stream channel</param>
        /// <param name="obj">object</param>
        public void Publish(string channel, T obj) {
            //Flag
            if (!IsConnected()) {
                //Invoke event
                OnErrorMessageHadle(new Exception("Connection is not ready to publish"));
                return;
            }

            //Get subscriber
            var publisher = GetSubscriber();
            //Get message data
            var message = ObserverDataUtility<T>.ObjToRedisValue(obj);

            //Publish
            publisher?.Publish(channel, message);
            //Invoke event
            OnSendMessageHadle(obj);
        }

        /// <summary>
        /// Get subscriber
        /// </summary>
        /// <returns>observador</returns>
        private ISubscriber GetSubscriber() {
            //Verify if is connected
            if (!IsConnected()) {
                //Invoke event
                OnErrorMessageHadle(new Exception("Connection is not ready to subcribe"));
                //Return
                return null;
            }

            //Get subscriber
            var subscriber = _connection.GetSubscriber();

            //Return
            return subscriber;
        }

        /// <summary>
        /// Receive messaage
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="channel">channel</param>
        protected virtual void OnReceiveMessageHadle(T obj, string channel) {
            //Invoke event
            OnReceiveMessage?.Invoke(obj, channel);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="obj">object</param>
        protected virtual void OnSendMessageHadle(T obj) {
            //Invoke event
            OnSendMessage?.Invoke(obj);
        }

        /// <summary>
        /// Emensagem de erro
        /// </summary>
        /// <param name="ex">erro</param>
        protected virtual void OnErrorMessageHadle(Exception ex) {
            //Invoke event
            OnErrorMessage?.Invoke(ex);
        }

        /// <summary>
        /// Information message
        /// </summary>
        /// <param name="message">mensagem</param>
        protected virtual void OnInfoMessageHandle(string message) {
            //Invoke event
            OnInfoMessage?.Invoke(message);
        }

        /// <summary>
        /// Connection event
        /// </summary>
        /// <param name="mensagem">message</param>
        /// <param name="eventArgs">event arguments</param>
        protected virtual void OnConnectionHandle(string mensagem, ConnectionFailedEventArgs eventArgs) {
            //Invoke event
            OnConnectionMessage?.Invoke(mensagem, eventArgs);
        }
    }
}