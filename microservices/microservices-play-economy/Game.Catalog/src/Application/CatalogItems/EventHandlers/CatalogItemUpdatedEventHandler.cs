using Game.Catalog.Contracts.Events;
using Game.Catalog.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Game.Catalog.Application.CatalogItems.EventHandlers;

internal class CatalogItemUpdatedEventHandler : INotificationHandler<CatalogItemUpdatedEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public CatalogItemUpdatedEventHandler(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(CatalogItemUpdatedEvent notification, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IPublishEndpoint publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        await publisher.Publish<ICatalogItemUpdatedEvent>(new
        {
            notification.Item.Id,
            notification.Item.Name,
            notification.Item.Description
        },
        cancellationToken);
    }
}