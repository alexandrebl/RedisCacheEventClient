using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;

namespace RedisCacheEventClient.Utilities {

    /// <summary>
    /// Ubserver data processor
    /// </summary>
    internal static class ObserverDataUtility<T> {

        /// <summary>
        /// Convert object
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>value</returns>
        public static RedisValue ObjToRedisValue(T obj) {
            //Get value
            var redisValue = (obj is string) ? obj.ToString() : JsonConvert.SerializeObject(obj);

            //Return
            return redisValue;
        }

        /// <summary>
        /// Convert object
        /// </summary>
        /// <param name="redisValue">value</param>
        /// <returns>object</returns>
        public static T RedisValueToObj(RedisValue redisValue) {
            //Temporary variable
            var obj = default(T);

            //Verify if exists value
            if (!redisValue.HasValue) return obj;

            //Verify if is valid
            if (IsValidJson(redisValue))
                try {
                    //Deserialize
                    obj = JsonConvert.DeserializeObject<T>(redisValue);
                } catch (Exception) {
                    //Convert object
                    obj = (T)(object)(string)redisValue;
                } else
                //Convert object
                obj = (T)(object)(string)redisValue;

            //Return
            return obj;
        }

        /// <summary>
        /// Verify if is a valid json
        /// </summary>
        /// <param name="json">json data</param>
        /// <returns>success flag</returns>
        public static bool IsValidJson(string json) {
            try {
                //Parse data
                JObject.Parse(json);
                //Return
                return true;
            } catch (Exception) {
                //Return
                return false;
            }
        }
    }
}