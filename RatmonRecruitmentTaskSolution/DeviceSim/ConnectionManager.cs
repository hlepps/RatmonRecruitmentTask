using MathNet.Numerics.Distributions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceBase
{
    public class ConnectionManager
    {
        string rabbitmqUsername = "admin";
        string rabbitmqPassword = "zaq1@WSX";
        string rabbitmqHostmane = "rrt.rabbitmq";
        int rabbitmqPort = 5672;

        Device device;
        
        public ConnectionManager(Device device)
        {
            this.device = device;
        }

        public async Task StartTransmitting(int dataFrequency)
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

            await channel.ExchangeDeclareAsync(exchange: "dataExchange", type: ExchangeType.Fanout, durable:true, autoDelete: false);


            while (true)
            {
                var message = device.GetDataMessage();
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(
                    exchange: "dataExchange",
                    routingKey: "data",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true, Expiration = "60000", ContentType="application/json"},
                    body:body
                    );

                Console.WriteLine($"Sent: {message}");
                Thread.Sleep(dataFrequency);
            }
        }
    }
}
