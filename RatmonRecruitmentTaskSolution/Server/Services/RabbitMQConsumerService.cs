using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Server.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        string rabbitmqUsername = "admin";
        string rabbitmqPassword = "zaq1@WSX";
        string rabbitmqHostmane = "rrt.rabbitmq";
        int rabbitmqPort = 5672;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitmqHostmane,
                UserName = rabbitmqUsername,
                Password = rabbitmqPassword,
                Port = rabbitmqPort,
                AutomaticRecoveryEnabled = true
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "deviceDataQueue", durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            Trace.WriteLine(" [*] Waiting for logs.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {

                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var deserialized = JsonSerializer.Deserialize <(string Id, string Name, object Data)>(message);
                var parsed = JsonDocument.Parse(message).RootElement;

                var senderID = parsed.GetProperty("Sender").GetString();
                var senderName = parsed.GetProperty("Name").GetString();
                var timestamp = parsed.GetProperty("Timestamp").GetString();
                var data = parsed.GetProperty("Data").GetRawText();

                var test1 = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2>(data);
                var test2 = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2B>(data);
                var test3 = JsonSerializer.Deserialize<Shared.DeviceData_MOUSECOMBO>(data);
                var test4 = JsonSerializer.Deserialize<Shared.DeviceData_MAS2>(data);

                Console.WriteLine($"[{timestamp}] ({senderName}({senderID})) {data}");
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("deviceDataQueue", autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
