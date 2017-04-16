using StackExchange.Redis;
using System;

namespace RedisCacheEventClient.Utilities.Interfaces {

    /// <summary>
    /// Utility event manager
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    internal interface IObserverManagerUtility<T> {

        /// <summary>
        /// Receive event
        /// </summary>
        event Action<T, string> OnReceiveMessage;

        /// <summary>
        /// Send message event
        /// </summary>
        event Action<T> OnSendMessage;

        /// <summary>
        /// Error event
        /// </summary>
        event Action<Exception> OnErrorMessage;

        /// <summary>
        /// Information event
        /// </summary>
        event Action<string> OnInfoMessage;

        /// <summary>
        /// Connection event
        /// </summary>
        event Action<string, ConnectionFailedEventArgs> OnConnectionMessage;

        /// <summary>
        /// flag that indicate is connected
        /// </summary>
        /// <returns>success flag</returns>
        bool IsConnected();

        /// <summary>
        /// Subscribe stream channel
        /// </summary>
        /// <param name="channel">stream channel</param>
        void Subscribe(string channel);

        /// <summary>
        /// Publish message
        /// </summary>
        /// <param name="channel">stream channel</param>
        /// <param name="obj">object</param>
        void Publish(string channel, T obj);
    }
}