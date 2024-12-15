using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace prometeyapi.Core.Services.RabbitMQ;

public class Receive
{
    List<string> messages = new List<string>();

    ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

    public string[] Get(string queueName)
    {
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messages.Add(message);
            };
            channel.BasicConsume(queueName, false, consumer);
            channel.QueueDelete(queueName);
        }

        return messages.ToArray();
    }
}