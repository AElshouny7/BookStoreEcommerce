using BookStoreEcommerce.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BookStoreEcommerce.Services;

public class UserActivityService(
    StoreDbContext _context
) : IUserActivityService
{
    public async Task TouchAsync(int userId, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
            return;

        var now = DateTime.UtcNow;

        if (user.LastActiveAt == null || (now - user.LastActiveAt) > TimeSpan.FromMinutes(5))
        {
            user.LastActiveAt = now;
            await _context.SaveChangesAsync(ct);
        }

    }
}