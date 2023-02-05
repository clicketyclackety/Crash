using Crash.Utilities;
using System.Collections;


namespace Crash.Tables
{

    // Should this be async?
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

            OnUserAdded.Invoke(this, new UserEventArgs(user));

            return false;
        }

        public bool Add(string userName)
        {
            User user = new User(userName);

            return Add(user);
        }

        public void Remove(string userName)
        {
            if (_users.TryGetValue(userName, out User user))
            {
                OnUserAdded.Invoke(this, new UserEventArgs(user));
            }

            _users.Remove(userName);
        }

        public User? Get(string userName)
        {
            if (_users.ContainsKey(userName)) return _users[userName];
            return null;
        }

        public IEnumerator<User> GetEnumerator() => _users.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public event EventHandler<UserEventArgs> OnUserAdded;
        public event EventHandler<UserEventArgs> OnUserRemoved;

    }

    public sealed class UserEventArgs : EventArgs
    {
        public readonly User User;

        public UserEventArgs(User user)
        {
            User = user;
        }

    }

}
