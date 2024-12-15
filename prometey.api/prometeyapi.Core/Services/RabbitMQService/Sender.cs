using System.Text;
using RabbitMQ.Client;

namespace prometeyapi.Core.Services.RabbitMQ;

public class Sender
{
    ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

    public bool Send(string messageForSending, string queueName)
    {
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var message = $"{messageForSending}";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }

        return true;
    }
}