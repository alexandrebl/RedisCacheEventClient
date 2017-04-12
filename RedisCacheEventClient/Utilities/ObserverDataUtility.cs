using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Utilitário de dados do observer
    /// </summary>
    public static class ObserverDataUtility<T> {

        /// <summary>
        /// Converte objeto
        /// </summary>
        /// <param name="obj">objeto</param>
        /// <returns>valor</returns>
        public static RedisValue ObjToRedisValue(T obj) {
            //Valor
            var redisValue = (obj is string) ? obj.ToString() : JsonConvert.SerializeObject(obj);

            //Retonro
            return redisValue;
        }

        /// <summary>
        /// Converte para objeto
        /// </summary>
        /// <param name="redisValue">valor</param>
        /// <returns>objeto</returns>
        public static T RedisValueToObj(RedisValue redisValue) {
            //Variável temporária
            var obj = default(T);

            //Verifica se há valor
            if (!redisValue.HasValue) return obj;

            //Verifica se é um json válido
            if (IsValidJson(redisValue))
                try {
                    //Deserializa
                    obj = JsonConvert.DeserializeObject<T>(redisValue);
                } catch (Exception) {
                    //Converte para objeto
                    obj = (T)(object)(string)redisValue;
                } else
                //Converte para objeto
                obj = (T)(object)(string)redisValue;

            //Retorno
            return obj;
        }

        /// <summary>
        /// Verifica se é um json válido
        /// </summary>
        /// <param name="json">dados no formato json</param>
        /// <returns>indica sucesso</returns>
        public static bool IsValidJson(string json) {
            try {
                //Efetua o parse
                JObject.Parse(json);
                //Retorno
                return true;
            } catch (Exception) {
                //Retorno de insucesso
                return false;
            }
        }
    }
}