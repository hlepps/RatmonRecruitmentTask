using DeviceBase;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server.Services
{
    public class RabbitMQDeviceConfigService : BackgroundService, IDisposable
    {
        private readonly DeviceService DeviceService;
        private readonly DeviceDataService DeviceDataService;
        private const string QUEUE_NAME = "rpc_queue";

        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();

        string rabbitmqUsername = "admin";
        string rabbitmqPassword = "zaq1@WSX";
        string rabbitmqHostmane = Program.IsRunningInContainer() ? "rrt.rabbitmq" : "localhost";
        int rabbitmqPort = 5672;

        public RabbitMQDeviceConfigService(DeviceService deviceService, DeviceDataService deviceDataService) : base()
        {
            this.DeviceService = deviceService;
            this.DeviceDataService = deviceDataService;
        }

        ConnectionFactory factory;
        IConnection connection;
        IChannel channel;
        private string? replyQueueName;
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
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

            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            replyQueueName = queueDeclareResult.QueueName;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;

                if (false == string.IsNullOrEmpty(correlationId))
                {
                    if (_callbackMapper.TryRemove(correlationId, out var tcs))
                    {
                        var body = ea.Body.ToArray();
                        var response = Encoding.UTF8.GetString(body);
                        tcs.TrySetResult(response);
                    }
                }

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(replyQueueName, true, consumer);
        }

        public async Task<string> CallAsync(RpcConfigMessageType messageType, string message, string receiverId, CancellationToken cancellationToken = default)
        {
            if (channel is null)
            {
                throw new InvalidOperationException();
            }

            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueueName
            };

            var tcs = new TaskCompletionSource<string>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            var jsonMessage = JsonSerializer.Serialize(new RpcConfigMessage(){ Type=messageType, JsonMessage=message });
            Console.WriteLine($"Sent: {jsonMessage}");
            var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: $"{QUEUE_NAME}_{receiverId}",
                mandatory: true, basicProperties: props, body: messageBytes);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    _callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
        }
        public override void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (channel is not null)
            {
                await channel.CloseAsync();
            }

            if (connection is not null)
            {
                await connection.CloseAsync();
            }
        }
    }
}
