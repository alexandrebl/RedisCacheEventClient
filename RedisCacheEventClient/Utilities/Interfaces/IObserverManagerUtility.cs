using StackExchange.Redis;
using System;

namespace RedisCacheEventClient.Utilities.Interfaces {

    /// <summary>
    /// Utilitário de controle de eventos
    /// </summary>
    /// <typeparam name="T">tipo</typeparam>
    internal interface IObserverManagerUtility<T> {

        /// <summary>
        /// Evento
        /// </summary>
        event Action<T, string> OnReceiveMessage;

        /// <summary>
        /// Evento
        /// </summary>
        event Action<T> OnSendMessage;

        /// <summary>
        /// Evento de erro
        /// </summary>
        event Action<Exception> OnErrorMessage;

        /// <summary>
        /// Evento de informação
        /// </summary>
        event Action<string> OnInfoMessage;

        /// <summary>
        /// Evento de conexão
        /// </summary>
        event Action<string, ConnectionFailedEventArgs> OnConnectionMessage;

        /// <summary>
        /// Indica se está conectado
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// Assina a fila
        /// </summary>
        /// <param name="channel">canal</param>
        void Subscribe(string channel);

        /// <summary>
        /// Publica mensagem
        /// </summary>
        /// <param name="channel">canal</param>
        /// <param name="obj">objeto</param>
        void Publish(string channel, T obj);
    }
}