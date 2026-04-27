using Microsoft.Extensions.DependencyInjection;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaPublisher(this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher, KafkaPublisher>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services)
    {
        services.AddHostedService<KafkaConsumer>();
        return services;
    }
}
