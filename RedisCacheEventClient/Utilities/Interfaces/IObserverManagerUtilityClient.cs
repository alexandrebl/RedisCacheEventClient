using System;
using System.Collections.Generic;

namespace RedisCacheEventClient.Utilities.Interfaces {

    /// <summary>
    /// Observer utility
    /// </summary>
    /// <typeparam name="T">tipo</typeparam>
    public interface IObserverManagerUtilityClient<T> {

        /// <summary>
        /// Evento
        /// </summary>
        event Action<T, string> OnReceived;

        /// <summary>
        /// Inicia a aplicação
        /// </summary>
        /// <param name="channels">canais</param>
        void Start(string[] channels);

        /// <summary>
        /// Lista de observers
        /// </summary>
        Dictionary<string, IObserverManagerUtility<T>> ObserverManagerUtilityDictionary { get; set; }
    }
}