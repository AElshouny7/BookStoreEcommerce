namespace BookStoreEcommerce.Services.Caching;

public static class CacheTtls
{
    public static readonly TimeSpan ProductsList = TimeSpan.FromMinutes(8);
    public static readonly TimeSpan ProductDetail = TimeSpan.FromMinutes(15);
}