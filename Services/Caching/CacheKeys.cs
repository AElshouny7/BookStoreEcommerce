namespace BookStoreEcommerce.Services.Caching;

public static class CacheKeys
{
    public static string ProductById(int productId)
    {
        return $"products:by-id:{productId}";
    }

    public const string ProductListAll = "products:list:all";

    public static string ProductsByCategory(int categoryId)
    {
        return $"products:list:category:{categoryId}";
    }

    public const string ListKeysProductAllSet = "products:listkeys:all";
    public const string ListKeysProductByCategoryAllSet = "products:listkeys:categories";


}