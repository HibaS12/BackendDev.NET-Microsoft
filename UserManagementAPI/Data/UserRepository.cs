using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public class UserRepository
    {
        private static readonly List<User> _users = new List<User>();
        private static readonly object _lock = new object();

        public IEnumerable<User> GetAll(int page = 1, int pageSize = 20)
        {
            return _users.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public bool Add(User user)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Department) ||
                !new EmailAddressAttribute().IsValid(user.Email))
            {
                return false; // validation failed
            }

            // Prevent duplicate emails
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false; // duplicate email
            }

            lock (_lock)
            {
                user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(user);
            }
            return true;
        }

        public bool Update(User user)
        {
            var existing = GetById(user.Id);
            if (existing == null) return false;

            // Validation
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Department) ||
                !new EmailAddressAttribute().IsValid(user.Email))
            {
                return false; // validation failed
            }

            // Prevent duplicate emails (excluding the current user)
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) && u.Id != user.Id))
            {
                return false; // duplicate email
            }

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Email = user.Email;
            existing.Department = user.Department;
            return true;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null) return false;

            lock (_lock)
            {
                _users.Remove(user);
            }
            return true;
        }
    }
}