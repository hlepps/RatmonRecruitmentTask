using Microsoft.AspNetCore.Components;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace Server.Services
{
    public class RabbitMQConsumerService : BackgroundService, IDisposable
    {
        private readonly DeviceService DeviceService;
        private readonly DeviceDataService DeviceDataService;

        public RabbitMQConsumerService(DeviceService deviceService, DeviceDataService deviceDataService) : base()
        {
            this.DeviceService = deviceService;
            this.DeviceDataService = deviceDataService;
        }

        string rabbitmqUsername = "admin";
        string rabbitmqPassword = "zaq1@WSX";
        string rabbitmqHostmane = Program.IsRunningInContainer() ? "rrt.rabbitmq" : "localhost";
        int rabbitmqPort = 5672;

        ConnectionFactory factory;
        IConnection connection; 
        IChannel channel; 
        AsyncEventingBasicConsumer consumer;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            factory = new ConnectionFactory
            {
                HostName = rabbitmqHostmane,
                UserName = rabbitmqUsername,
                Password = rabbitmqPassword,
                Port = rabbitmqPort,
                AutomaticRecoveryEnabled = true
            };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "deviceDataQueue", durable: true, exclusive: false, autoDelete: false,
                arguments: null);


            consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var deserialized = JsonSerializer.Deserialize<(string Id, string Name, object Data)>(message);
                    var parsed = JsonDocument.Parse(message).RootElement;

                    var senderID = parsed.GetProperty("Sender").GetString();
                    var senderName = parsed.GetProperty("Name").GetString();
                    var timestamp = DateTime.Parse(parsed.GetProperty("Timestamp").GetString());
                    var data = parsed.GetProperty("Data").GetRawText();

                    DeviceType deviceType;
                    DeviceDataBase parsedData = DeviceDataParser.ParseJSON(data, out deviceType);

                    ThreadPool.QueueUserWorkItem(
                        new WaitCallback(delegate (object state)
                        {
                            ManageIncomingDataAsync(senderID, senderName, deviceType, timestamp, parsedData);
                            //Console.WriteLine($"Received: [{timestamp}] ({senderName}({senderID})) {parsedData}");
                        }), null);


                    //await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION WHILE CONSUMING: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync("deviceDataQueue", autoAck: true, consumer: consumer);

        }

        public override void Dispose()
        {
            base.Dispose();
            channel.Dispose();
            connection.Dispose();
        }

        async void ManageIncomingDataAsync(string senderId, string senderName, DeviceType type, DateTime timestamp, DeviceDataBase data)
        {
            if (await DeviceService.CheckIfDeviceIsRegisteredAsync(senderId))
            {
                await DeviceDataService.SaveDeviceDataAsync(senderId, timestamp, data);
            }
            else
            {
                await DeviceService.RegisterNewDeviceAsync(senderId, senderName, type);
            }

        }
    }
}
