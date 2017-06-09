# RedisCacheEventClient
Redis cache event client manager

<strong>Develop branch</strong><br />
<img src="https://ci.appveyor.com/api/projects/status/github/alexandrebl/RedisCacheEventClient?branch=develop&svg=true" alt="Project Badge" with="300">

<strong>Master branch</strong><br />
<img src="https://ci.appveyor.com/api/projects/status/github/alexandrebl/RedisCacheEventClient?branch=master&svg=true" alt="Project Badge" with="300">

How to use:

Package console: Install-Package RedisCacheEventClient

```cs
using RedisCacheEventClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleExample {

    /// <summary>
    /// Main class
    /// </summary>
    internal class Program {

        /// <summary>
        /// Main method
        /// </summary>
        private static void Main() {
            //Redis cache controller
            var redisCache = new RedisEventManager<string>("127.0.0.1:6379", "chTest");

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
}
```
Other example using entity class:

```cs
using RedisCacheEventClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleExample {

    /// <summary>
    /// Main class
    /// </summary>
    internal class Program {

        /// <summary>
        /// Main method
        /// </summary>
        private static void Main() {
            //Redis cache controller
            var redisCache = new RedisEventManager<MessageData>("127.0.0.1:6379", "chTest");

            //Received message event
            redisCache.ReceiveMessage += RedisCache_ReceiveMessage;

            //Subscribe channel
            redisCache.Subcribe();

            //Task to simulation publish messages
            Task.Factory.StartNew(() => {
                //Loop for send 1000 messages
                for (int index = 0; index < 1000; index++) {
                    //Mensagem
                    var messageData = new MessageData {
                        Message = $"{DateTime.UtcNow:o} => index value: {index}"
                    };

                    //Print message
                    Console.WriteLine($"Sent => Key: {messageData.Key} | Mesage: {messageData.Message}");

                    //Publish message
                    redisCache.Publish(messageData);

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
        /// <param name="messageData">object received</param>
        private static void RedisCache_ReceiveMessage(MessageData messageData) {
            //Prin message
            Console.WriteLine($"Recv => Key: {messageData.Key} | Mesage: {messageData.Message}");
        }

        /// <summary>
        /// Message data
        /// </summary>
        public class MessageData {

            /// <summary>
            /// Constructor method
            /// </summary>
            public MessageData() {
                //Set key
                Key = Guid.NewGuid();
            }

            /// <summary>
            /// Message
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Key
            /// </summary>
            public Guid Key { get; set; }
        }
    }
}
```
