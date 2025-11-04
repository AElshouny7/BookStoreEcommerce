namespace BookStoreEcommerce.DBContext;

public interface ICustomerRepo
{
    // Users whose last activity is <= cutoffUtc (or null).
    Task<List<CustomerSlim>> GetInactiveSinceAsync(DateTime cutoffUtc, CancellationToken ct);

    // Last time this customer was active (or null if never).
    Task<DateTime?> GetLastActiveUtcAsync(int customerId, CancellationToken ct);
}

public sealed class CustomerSlim
{
    public int CustomerId { get; init; }
    public string? Email { get; init; }
    public DateTime LastActive { get; init; }
}