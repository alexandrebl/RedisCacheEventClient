using System;

namespace RedisCacheEventClient {

    /// <summary>
    /// Redis event Manager
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    public interface IRedisEventManager<T> {

        /// <summary>
        /// Reception event
        /// </summary>
        event Action<T> ReceiveMessage;

        /// <summary>
        /// Subscribe method
        /// </summary>
        void Subcribe();

        /// <summary>
        /// Publish method
        /// </summary>
        /// <param name="obj">object data</param>
        void Publish(T obj);
    }
}