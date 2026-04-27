using Microsoft.Extensions.DependencyInjection;
using TechfinChallenge.Messaging.Abstractions;

namespace TechfinChallenge.Messaging.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqPublisher>();
        services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<RabbitMqPublisher>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqPublisher>());
        return services;
    }

    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services)
    {
        services.AddHostedService<RabbitMqConsumer>();
        return services;
    }
}
