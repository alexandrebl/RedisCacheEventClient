using RedisCacheEventClient.Library;
using RedisCacheEventClient.Utilities.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Utilitário de controle de eventos
    /// </summary>
    /// <typeparam name="T">tipo</typeparam>
    internal class ObserverManagerUtility<T> : IObserverManagerUtility<T> {

        /// <summary>
        /// Conexão
        /// </summary>
        private readonly ConnectionMultiplexer _connection;

        /// <summary>
        /// Evento
        /// </summary>
        public event Action<T, string> OnReceiveMessage;

        /// <summary>
        /// Evento
        /// </summary>
        public event Action<T> OnSendMessage;

        /// <summary>
        /// Evento de erro
        /// </summary>
        public event Action<Exception> OnErrorMessage;

        /// <summary>
        /// Evento de informação
        /// </summary>
        public event Action<string> OnInfoMessage;

        /// <summary>
        /// Evento de conexão
        /// </summary>
        public event Action<string, ConnectionFailedEventArgs> OnConnectionMessage;

        /// <summary>
        /// Objeto de sincronismo
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Método construtor
        /// </summary>
        public ObserverManagerUtility(string host) {
            //Define conexão
            ObserverConnectionManager.SetConnectionString(host);
            //Aber uma conexão
            _connection = ObserverConnectionManager.OpenConnection(true);

            //Evento de falha
            ObserverConnectionManager.OnFailed += delegate (ConnectionFailedEventArgs args) {
                //Dispara o evento
                OnConnectionHandle("Connection error", args);
            };

            //Evento de conexão restabelecida
            ObserverConnectionManager.OnRestored += delegate (ConnectionFailedEventArgs args) {
                //Dispara o evento
                OnConnectionHandle("Connection restored", args);
            };

            //Evento de erro
            ObserverConnectionManager.OnError += delegate (Exception exception) {
                //Dispara o evento
                OnErrorMessageHadle(new Exception($"Connection error. Exception: {exception}"));
            };

            //Evento de informação
            ObserverConnectionManager.OnInfo += OnInfoMessageHandle;
        }

        /// <summary>
        /// Indica se está conectado
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() {
            //Sincronismo
            lock (SyncObj) {
                //Indica se está conectado
                return _connection?.IsConnected ?? false;
            }
        }

        /// <summary>
        /// Assina a fila
        /// </summary>
        /// <param name="channel">canal</param>
        public void Subscribe(string channel) {
            //Obtem uma assinatura no canal
            var subscriber = GetSubscriber();

            try {
                //Assina o canal
                subscriber?.Subscribe(channel, (redisChannel, message) => {
                    //Converte o tipo do objeto
                    var obj = ObserverDataUtility<T>.RedisValueToObj(message);
                    //diapara o evento
                    OnReceiveMessageHadle(obj, channel);
                });

                //Desfaz a assinatura do canal
                subscriber?.Unsubscribe(channel, (redisChannel, value) => {
                    //Assina o canal
                    Subscribe(channel);
                });
            } catch (Exception ex) {
                //Dispara o evento
                OnErrorMessageHadle(ex);
                //Aguarda 1 segundo
                Task.Run(() => { Thread.Sleep(1000); });
                //Assina o canal
                Subscribe(channel);
            }
        }

        /// <summary>
        /// Publica mensagem
        /// </summary>
        /// <param name="channel">canal</param>
        /// <param name="obj">objeto</param>
        public void Publish(string channel, T obj) {
            //Indica se está conectado
            if (!IsConnected()) {
                //Dispara evento
                OnErrorMessageHadle(new Exception("Connection is not ready to publish"));
                return;
            }

            //Obtem um publicador
            var publisher = GetSubscriber();
            //Mensagem
            var message = ObserverDataUtility<T>.ObjToRedisValue(obj);

            //Publica
            publisher?.Publish(channel, message);
            //Dispara mensagem
            OnSendMessageHadle(obj);
        }

        /// <summary>
        /// Obtem um observador
        /// </summary>
        /// <returns>observador</returns>
        private ISubscriber GetSubscriber() {
            //Verifica se está conectado
            if (!IsConnected()) {
                //Dispara evento
                OnErrorMessageHadle(new Exception("Connection is not ready to subcribe"));
                //Retorno
                return null;
            }

            //Obtem um observador
            var subscriber = _connection.GetSubscriber();

            //Retorno
            return subscriber;
        }

        /// <summary>
        /// Mensagem recebida
        /// </summary>
        /// <param name="obj">objeto</param>
        /// <param name="channel">canal</param>
        protected virtual void OnReceiveMessageHadle(T obj, string channel) {
            //Dispara evento
            OnReceiveMessage?.Invoke(obj, channel);
        }

        /// <summary>
        /// Envio de mensagem
        /// </summary>
        /// <param name="obj">objeto</param>
        protected virtual void OnSendMessageHadle(T obj) {
            //Dispara evento
            OnSendMessage?.Invoke(obj);
        }

        /// <summary>
        /// Emensagem de erro
        /// </summary>
        /// <param name="ex">erro</param>
        protected virtual void OnErrorMessageHadle(Exception ex) {
            //Dispara evento
            OnErrorMessage?.Invoke(ex);
        }

        /// <summary>
        /// Mensagem de informação
        /// </summary>
        /// <param name="message">mensagem</param>
        protected virtual void OnInfoMessageHandle(string message) {
            //Dispara evento
            OnInfoMessage?.Invoke(message);
        }

        /// <summary>
        /// Evento de conexão
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="eventArgs">argumentos do evento</param>
        protected virtual void OnConnectionHandle(string mensagem, ConnectionFailedEventArgs eventArgs) {
            //Dispara evento
            OnConnectionMessage?.Invoke(mensagem, eventArgs);
        }
    }
}