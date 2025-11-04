namespace BookStoreEcommerce.Jobs;

public interface IReengageInactiveUsersJob
{
    Task Run(CancellationToken ct = default);
}

