namespace BookStoreEcommerce.Services;

public interface IUserActivityService
{
    Task TouchAsync(int userId, CancellationToken ct = default);
}