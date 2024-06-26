﻿using Common.Events;
using RabbitMQ.Client;

namespace Api.Notifications.Infrastructure.MessageBus;

internal sealed partial class ConsumerWorker
{
    private static readonly Dictionary<string, Type> _supportedMessages =
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => !t.IsAbstract)
            .Where(t => t.IsAssignableTo(typeof(DomainEvent)))
            .ToDictionary(t => t.Name, t => t);

    private void _setup()
    {
        _channel.ExchangeDeclare(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: typeof(EmailNotificationSentEvent).Name);

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: typeof(EmailNotificationFailedEvent).Name);

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: typeof(SMSNotificationSentEvent).Name);

        _channel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: typeof(SMSNotificationFailedEvent).Name);

        _channel.BasicQos(
            0,
            1, // Total message receive same time
            false); // [ false per consumer | true per channel ]
    }
}
