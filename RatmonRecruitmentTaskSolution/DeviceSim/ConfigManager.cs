using DeviceSim;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeviceBase
{
    public class ConfigManager : IDisposable
    {
        string configFileName = null;
        Device device;
        Task rpcTask;
        public ConfigManager(Device device)
        {
            this.device = device;
            configFileName = Environment.GetEnvironmentVariable("CONFIG_FILENAME") + ".json";
            if (configFileName is null) throw new Exception("Environment variable CONFIG_FILENAME not specified");

            if (!File.Exists(configFileName))
            {
                GenerateConfigFile();
            }
            else
            {
                string json = File.ReadAllText(configFileName);
                try
                {
                    switch (device.deviceType)
                    {
                        case Shared.DeviceType.MOUSE2:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSE2>(json);
                            break;
                        case Shared.DeviceType.MOUSE2B:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSE2B>(json);
                            break;
                        case Shared.DeviceType.MOUSECOMBO:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSECOMBO>(json);
                            break;
                        case Shared.DeviceType.MAS2:
                            device.Config = JsonSerializer.Deserialize<Config_MAS2>(json);
                            break;
                    }
                }
                catch (Exception e)
                {
                    GenerateConfigFile();
                }
                if (device.Config is null) throw new Exception("Unable to deserialize config.json");
                Console.WriteLine($"ID:{device.Config.UniqueId}");
            }

            rpcTask = Task.Run(StartRpc);
            rpcTask.Wait();
        }

        ConnectionFactory factory;
        IConnection connection;
        IChannel channel;
        const string QUEUE_NAME = "rpc_queue";
        string rabbitmqUsername = "admin";
        string rabbitmqPassword = "zaq1@WSX";
        string rabbitmqHostmane = Program.IsRunningInContainer() ? "rrt.rabbitmq" : "localhost";
        int rabbitmqPort = 5672;
        async Task StartRpc()
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

            await channel.QueueDeclareAsync(queue: $"{QUEUE_NAME}_{device.Config.UniqueId}", durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            Console.WriteLine("Started listening");
            consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs ea) =>
            {
                Console.WriteLine("Received message");
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
                IChannel ch = cons.Channel;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId
                };

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received: {message}");
                    RpcConfigMessage deserializedMessage = new RpcConfigMessage();
                    using var document = JsonDocument.Parse(message);
                    deserializedMessage.Type = (RpcConfigMessageType)document.RootElement.GetProperty("Type").GetInt32();
                    deserializedMessage.JsonMessage = document.RootElement.GetProperty("JsonMessage").GetString();
                    switch (deserializedMessage!.Type)
                    {
                        case RpcConfigMessageType.GET_CONFIG:
                            switch (device.deviceType)
                            {
                                case Shared.DeviceType.MOUSE2:
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSE2);
                                    break;
                                case Shared.DeviceType.MOUSE2B:
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSE2B);
                                    break;
                                case Shared.DeviceType.MOUSECOMBO:
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSECOMBO);
                                    break;
                                case Shared.DeviceType.MAS2:
                                    response = JsonSerializer.Serialize(device.Config as Config_MAS2);
                                    break;
                            }
                            break;
                        case RpcConfigMessageType.UPDATE_CONFIG:
                            switch (device.deviceType)
                            {
                                case Shared.DeviceType.MOUSE2:
                                    device.Config = JsonSerializer.Deserialize<Config_MOUSE2>(deserializedMessage!.JsonMessage)!;
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSE2);
                                    break;
                                case Shared.DeviceType.MOUSE2B:
                                    device.Config = JsonSerializer.Deserialize<Config_MOUSE2B>(deserializedMessage!.JsonMessage)!;
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSE2B);
                                    break;
                                case Shared.DeviceType.MOUSECOMBO:
                                    device.Config = JsonSerializer.Deserialize<Config_MOUSECOMBO>(deserializedMessage!.JsonMessage)!;
                                    response = JsonSerializer.Serialize(device.Config as Config_MOUSECOMBO);
                                    break;
                                case Shared.DeviceType.MAS2:
                                    device.Config = JsonSerializer.Deserialize<Config_MAS2>(deserializedMessage!.JsonMessage)!;
                                    response = JsonSerializer.Serialize(device.Config as Config_MAS2);
                                    break;
                            }
                            SaveConfigToFile();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($" [!] Unknown Message: {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await ch.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync($"{QUEUE_NAME}_{device.Config.UniqueId}", false, consumer);
        }

        void GenerateConfigFile()
        {
            switch (device.deviceType)
            {
                case Shared.DeviceType.MOUSE2:
                    device.Config = new Config_MOUSE2() { AlarmThreshold = new Random().NextDouble() * 10 };
                    device.Config.Name = device.deviceType.ToString();
                    device.Config.UniqueId = Guid.CreateVersion7().ToString();
                    break;
                case Shared.DeviceType.MOUSE2B:
                    device.Config = new Config_MOUSE2B() { AlarmThreshold = new Random().NextDouble() * 10, CableLength = new Random().NextDouble() * 100 };
                    device.Config.Name = device.deviceType.ToString();
                    device.Config.UniqueId = Guid.CreateVersion7().ToString();
                    break;
                case Shared.DeviceType.MOUSECOMBO:
                    device.Config = new Config_MOUSECOMBO() { AlarmThreshold = new Random().NextDouble() * 10 };
                    device.Config.Name = device.deviceType.ToString();
                    device.Config.UniqueId = Guid.CreateVersion7().ToString();
                    break;
                case Shared.DeviceType.MAS2:
                    device.Config = new Config_MAS2() { TemperatureThreshold = new Random().NextDouble() * 100, HumidityThreshold = new Random().NextDouble() * 100 };
                    device.Config.Name = device.deviceType.ToString();
                    device.Config.UniqueId = Guid.CreateVersion7().ToString();
                    break;
            }

            SaveConfigToFile();
        }

        void SaveConfigToFile()
        {
            string config = "";
            switch (device.deviceType)
            {
                case Shared.DeviceType.MOUSE2:
                    config = JsonSerializer.Serialize<Config_MOUSE2>(device.Config as Config_MOUSE2);
                    break;
                case Shared.DeviceType.MOUSE2B:
                    config = JsonSerializer.Serialize<Config_MOUSE2B>(device.Config as Config_MOUSE2B);
                    break;
                case Shared.DeviceType.MOUSECOMBO:
                    config = JsonSerializer.Serialize<Config_MOUSECOMBO>(device.Config as Config_MOUSECOMBO);
                    break;
                case Shared.DeviceType.MAS2:
                    config = JsonSerializer.Serialize<Config_MAS2>(device.Config as Config_MAS2);
                    break;
            }
            File.WriteAllText(configFileName, config);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
