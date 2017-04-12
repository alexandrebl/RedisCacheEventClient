using RedisCacheEventClient.Utilities.Interfaces;
using System;
using System.Collections.Generic;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Observer utility
    /// </summary>
    /// <typeparam name="T">tipo</typeparam>
    public class ObserverManagerUtilityClient<T> : IObserverManagerUtilityClient<T> {

        /// <summary>
        /// Servidor redis
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// Evento
        /// </summary>
        public event Action<T, string> OnReceived;

        /// <summary>
        /// Lista de observers
        /// </summary>
        public Dictionary<string, IObserverManagerUtility<T>> ObserverManagerUtilityDictionary { get; set; }

        /// <summary>
        /// Método construtor
        /// </summary>
        /// <param name="host">servidor</param>
        /// <param name="logEntry">log de dados</param>
        public ObserverManagerUtilityClient(string[] channels, string host) {
            //Servidor
            _host = host;
            //Inicializa lista
            ObserverManagerUtilityDictionary = new Dictionary<string, IObserverManagerUtility<T>>();
            //Prepara a aplicação
            Initilize(channels);
        }

        /// <summary>
        /// Prepara a aplicação
        /// </summary>
        /// <param name="channels">canais</param>
        private void Initilize(string[] channels) {
            //Acessa a cada canal
            foreach (var channel in channels) {
                //Observador
                var observerManagerUtility = new ObserverManagerUtility<T>(_host);
                //Dispara evento
                observerManagerUtility.OnReceiveMessage += OnReceivedHadle;
                //Adiciona a lista
                ObserverManagerUtilityDictionary.Add(channel, observerManagerUtility);
            }
        }

        /// <summary>
        /// Evento de envio
        /// </summary>
        /// <param name="obj">objeto</param>
        /// <param name="channel">canais</param>
        protected virtual void OnReceivedHadle(T obj, string channel) {
            //Dispara evento
            OnReceived?.Invoke(obj, channel);
        }

        /// <summary>
        /// Inicia o processamento
        /// </summary>
        /// <param name="channels"></param>
        public void Start(string[] channels) {
            //Acessa a cada item
            foreach (var observerManagerUtility in ObserverManagerUtilityDictionary) {
                //Inicia o processamento
                observerManagerUtility.Value.Subscribe(observerManagerUtility.Key);
            }
        }
    }
}