using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public class UserRepo(StoreDbContext context) : IUserRepo
    {
        private readonly StoreDbContext _context = context;

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            return user;
        }

        public User? UpdateUser(User user)
        {
            _context.Users.Update(user);
            return user;
        }

        public User? DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                return user;
            }
            return null;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}