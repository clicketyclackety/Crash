using System.Collections;


namespace Crash.Tables
{

    // TODO : Add User Added/Removed Events for UI to hook into
    public sealed class UserTable : IEnumerable<User>
    {

        private readonly Dictionary<string, User> _users;

        public static User CurrentUser;

        public UserTable()
        {
            _users = new Dictionary<string, User>();
        }

        public bool Add(User user)
        {
            // TODO : Add logic preventing add of current user?

            if (!_users.ContainsKey(user.Name))
            {
                _users[user.Name] = user;
                return true;
            }

            return false;
        }

        public bool Add(string userName)
        {
            User user = new User(userName);
            return Add(user);
        }

        public void Remove(string userName)
        {
            _users.Remove(userName);
        }

        public User? Get(string userName)
        {
            if (_users.ContainsKey(userName)) return _users[userName];
            return null;
        }

        public IEnumerator<User> GetEnumerator() => _users.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
