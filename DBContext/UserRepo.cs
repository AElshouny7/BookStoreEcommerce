using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public class UserRepo : IUserRepo
    {
        private readonly StoreDbContext _context;

        public UserRepo(StoreDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}