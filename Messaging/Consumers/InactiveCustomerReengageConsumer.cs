using BookStoreEcommerce.Messaging.Contracts;
using MassTransit;

namespace BookStoreEcommerce.Messaging.Consumers;

public class InactiveCustomerReengageConsumer(
ILogger<InactiveCustomerReengageConsumer> _logger
) : IConsumer<InactiveCustomerReengageRequested>
{
    public async Task Consume(ConsumeContext<InactiveCustomerReengageRequested> context)
    {
        var message = context.Message;

        _logger.LogInformation("Re-engage customer {Id} ({Email}) at {Time}",
            message.CustomerId, message.Email ?? "<no-email>", message.TriggeredAt);

        await Task.CompletedTask;
    }
}
