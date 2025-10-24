using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.DBContext
{
    public interface IUserRepo
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);
        User AddUser(User user);
        User? UpdateUser(User user);
        User? DeleteUser(int id);
        bool SaveChanges();
    }
}