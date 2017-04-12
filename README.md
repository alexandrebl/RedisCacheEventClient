# RedisCacheEventClient
Redis cache event client manager

/// <summary>
    /// Main class
    /// </summary>
    internal class Program {

        /// <summary>
        /// Main method
        /// </summary>
        private static void Main() {
            //Redis cache controller
            var redisCache = new RedisCacheEventClient.RedisEventManager<string>("127.0.0.1:6379", "chTest");

            //Received message event
            redisCache.ReceiveMessage += RedisCache_ReceiveMessage;

            //Subscribe channel
            redisCache.Subcribe();

            //Task to simulation publish messages
            Task.Factory.StartNew(() => {
                //Loop for send 1000 messages
                for (int index = 0; index < 1000; index++) {
                    //Message data
                    var message = $"{DateTime.UtcNow:o} => index value: {index}";
                    //Print message
                    Console.WriteLine($"Sent: {message}");

                    //Publish message
                    redisCache.Publish(message);

                    //Wait for 1 second
                    Thread.Sleep(1000);
                }
            });

            //Wait for press one Key
            Console.ReadKey();
        }

        /// <summary>
        /// Received message method
        /// </summary>
        /// <param name="obj">object received</param>
        private static void RedisCache_ReceiveMessage(string obj) {
            //Prin message
            Console.WriteLine($"Received: {obj}");
        }
    }
