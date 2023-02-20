using System.Collections;

using Crash.Common.Document;


namespace Crash.Common.Tables
{

	// Should this be async?
	public sealed class UserTable : IEnumerable<User>
	{

		private readonly Dictionary<string, User> _users;

		private CrashDoc crashDoc;

		public User CurrentUser { get; set; }

		public UserTable(CrashDoc hostDoc)
		{
			_users = new Dictionary<string, User>();
			crashDoc = hostDoc;
		}

		public bool Add(User user)
		{
			// TODO : Add logic preventing add of current user?

			if (!_users.ContainsKey(user.Name))
			{
				_users.Add(user.Name, user);
				return true;
			}

			OnUserAdded?.Invoke(this, new UserEventArgs(user));

			return false;
		}

		public bool Add(string userName)
		{
			var user = new User(userName);

			return Add(user);
		}

		public void Remove(string userName)
		{
			if (_users.TryGetValue(userName, out var user))
			{
				OnUserAdded?.Invoke(this, new UserEventArgs(user));
			}

			_users.Remove(userName);
		}

		public User Get(string userName)
		{
			string lowerUserName = userName.ToLower();
			if (_users.ContainsKey(lowerUserName)) return _users[lowerUserName];
			return default(User);
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
