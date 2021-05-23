using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Core.Command;
using RabbitMQ.Consumer.Core.Configuration;
using RabbitMQ.Consumer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static RabbitMQ.Consumer.Core.Command.RepescagemAtivaCommand;

namespace RabbitMQ.Consumer.Core
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var appSettings = FileUtils.ReadFileFromPath("AppSettings.json");
            var producerCoreConfiguration = JsonConvert.DeserializeObject<ConsumerCoreConfiguration>(appSettings);

            IList<ImportacaoRepescagemAtivaViewModel> listImportacao = new List<ImportacaoRepescagemAtivaViewModel>();

            var factory = new ConnectionFactory()
            {
                HostName = producerCoreConfiguration.RabbitMQHostName,
                VirtualHost = producerCoreConfiguration.RabbitMQVirtualHost,
                Password = producerCoreConfiguration.RabbitMQVPassword,
                UserName = producerCoreConfiguration.RabbitMQUserName
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "Q.Producer.Core",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Thread.Sleep(2000);
                var bodys = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(bodys);
                var objeto = JsonConvert.DeserializeObject<ImportacaoRepescagemAtivaViewModel>(message);
                listImportacao.Add(objeto);
                objeto.Id = 0;

                var result = RepescagemAtivaCommand.EnviarCPFsForRepescagemAtivaImportacao(objeto, producerCoreConfiguration.RepescagemAtivaWebApiUrl);

                if (result)
                {
                    Console.WriteLine("Recebido da Fila do RabbitMQ e Processado com sucesso.", message);
                }
                else
                {
                    Console.WriteLine("Recebido da Fila do RabbitMQ e NÃO IMPORTADO", message);
                }
            };
            channel.BasicConsume(queue: "Q.Producer.Core",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}