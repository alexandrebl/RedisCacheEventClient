using RedisCacheEventClient.Utilities;
using RedisCacheEventClient.Utilities.Interfaces;
using System;

namespace RedisCacheEventClient {

    /// <summary>
    /// Redis event Manager
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    public class RedisEventManager<T> : IRedisEventManager<T> {

        /// <summary>
        /// Channel definition
        /// </summary>
        private readonly string _channel;

        /// <summary>
        /// Observer manager
        /// </summary>
        private IObserverManagerUtility<T> _observerManagerUtility;

        /// <summary>
        /// constructor method
        /// </summary>
        /// <param name="host">host and port</param>
        /// <param name="channel">channel</param>
        public RedisEventManager(string host, string channel) {
            //Set object
            _observerManagerUtility = new ObserverManagerUtility<T>(host);
            //Set channel
            _channel = channel;
            //Define evento
            _observerManagerUtility.OnReceiveMessage += OnReceiveMessage;
        }

        /// <summary>
        /// Message received
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="channel">channel message</param>
        private void OnReceiveMessage(T obj, string channel) {
            //Event invoker
            ReceiveMessage?.Invoke(obj);
        }

        /// <summary>
        /// Reception event
        /// </summary>
        public event Action<T> ReceiveMessage;

        /// <summary>
        /// Subscribe method
        /// </summary>
        public void Subcribe() {
            //Subscribe method
            _observerManagerUtility.Subscribe(_channel);
        }

        /// <summary>
        /// Publish method
        /// </summary>
        /// <param name="obj">object data</param>
        public void Publish(T obj) {
            //Publish data
            _observerManagerUtility.Publish(_channel, obj);
        }
    }
}