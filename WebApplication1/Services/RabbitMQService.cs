using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Pojos;

namespace WebApplication1.Services
{
    public class RabbitMQService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private ConnectionFactory factory;
        private VaultClientService vaultClient;

        public RabbitMQService(ILoggerFactory loggerFactory, VaultClientService vaultClient)
        {
            this._logger = loggerFactory.CreateLogger<RabbitMQService>();
            this.vaultClient = vaultClient;
            ConnectRabbitMQ();
        }

        private void ConnectRabbitMQ()
        {
            // Get Credentials from RabbitMQ
            Task<Credentials> task = vaultClient.GetCredentials();

            factory = new ConnectionFactory { HostName = "localhost", UserName = task.Result.UserName, Password = task.Result.Password };
            //factory = new ConnectionFactory { HostName = "10.0.0.9", UserName = "mezz", Password = "P@ssw0rd" };

            factory.AutomaticRecoveryEnabled = true;
            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("bridge.cloud.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("bridge.cloud.loyalty", false, false, false, null);
            _channel.QueueBind("bridge.cloud.loyalty", "bridge.cloud.exchange", "bridge.cloud.loyalty.*", null);
            _channel.BasicQos(0, 1, false);


            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        }


        private void HandleMessage(string content)
        {
            // we just print this message   

            //_logger.LogInformation($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {

            // Get new Credentials if problen on it
            Task<Credentials> task = vaultClient.GetCredentials();
            factory.UserName = task.Result.UserName;
            factory.Password = task.Result.Password;
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {

        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                    // received message  
                    var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                    // handle the received message  
                    HandleMessage(content);
                try
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }

            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("bridge.cloud.loyalty", false, consumer);

            return Task.CompletedTask;
        }

    }
}
