using Microsoft.EntityFrameworkCore;

namespace BookStoreEcommerce.DBContext;

public class CustomerRepo(
    StoreDbContext _context
) : ICustomerRepo
{
    public async Task<List<CustomerSlim>> GetInactiveSinceAsync(DateTime cutoff, CancellationToken ct = default)
    {
        // var query =
        //     from user in _context.Users
        //     let lastOrderAt = _context.Orders
        //         .Where(o => o.UserId == user.Id)
        //         .Max(o => (DateTime?)o.OrderDate)
        //     let lastActive = (DateTime?)user.CreatedAt ?? lastOrderAt
        //     where lastActive == null || lastActive <= cutoff
        //     select new CustomerSlim
        //     {
        //         CustomerId = user.Id,
        //         Email = user.Email!,
        //         LastActive = (DateTime)lastActive!
        //     };

        var query =
            from user in _context.Users
            let lastOrderAt = _context.Orders
                .Where(o => o.UserId == user.Id)
                .Max(o => (DateTime?)o.OrderDate)
            let lastActive = ((DateTime?)user.LastActiveAt ?? lastOrderAt) ?? user.CreatedAt
            where lastActive <= cutoff
            select new CustomerSlim
            {
                CustomerId = user.Id,
                Email = user.Email!,
                LastActive = lastActive
            };

        return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task<DateTime?> GetLastActiveUtcAsync(int customerId, CancellationToken ct)
    {
        var userLast = await _context.Users
            .Where(u => u.Id == customerId)
            .Select(u => (DateTime?)u.LastActiveAt)
            .FirstOrDefaultAsync(ct);

        if (userLast is not null)
            return userLast;

        var lastOrder = await _context.Orders
            .Where(o => o.UserId == customerId)
            .MaxAsync(o => (DateTime?)o.OrderDate, ct);

        return lastOrder;

    }
}

