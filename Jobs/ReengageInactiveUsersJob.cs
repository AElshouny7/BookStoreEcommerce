using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Messaging.Contracts;
using MassTransit;

namespace BookStoreEcommerce.Jobs;

public class ReengageInactiveUsersJob(
ICustomerRepo _customers,
IPublishEndpoint _publisher,
ILogger<ReengageInactiveUsersJob> _logger
) : IReengageInactiveUsersJob
{
    public async Task Run(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-7);
        var inactive = await _customers.GetInactiveSinceAsync(cutoff, ct);

        _logger.LogInformation("Found {Count} inactive customers since {Cutoff}", inactive.Count, cutoff);

        foreach (var customer in inactive)
        {
            await _publisher.Publish(new InactiveCustomerReengageRequested(
                customer.CustomerId, customer.Email, DateTime.UtcNow
            ), ct);
        }
    }
}