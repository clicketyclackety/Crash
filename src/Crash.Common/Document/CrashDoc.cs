using System.Runtime.CompilerServices;

using Crash.Client;
using Crash.Common.Tables;
using Crash.Communications;
using Crash.Events;

[assembly: InternalsVisibleTo("Crash.Common.Tests")]
namespace Crash.Common.Document
{

	public sealed class CrashDoc : IEquatable<CrashDoc>, IDisposable
	{
		private readonly Guid _id;

		#region Tables

		public readonly UserTable Users;

		public readonly ChangeTable CacheTable;

		public readonly CameraTable Cameras;

		#endregion

		#region Connectivity

		public CrashClient? LocalClient { get; set; }

		public CrashServer? LocalServer { get; set; }

		#endregion

		#region Queue

		public IdleQueue Queue { get; private set; }

		#endregion

		#region constructors

		public CrashDoc()
		{
			_id = Guid.NewGuid();

			Users = new UserTable(this);
			CacheTable = new ChangeTable(this);
			Cameras = new CameraTable(this);

			Queue = new IdleQueue(this);
		}


		internal static CrashDoc CreateHeadless()
		{
			return new CrashDoc();
		}

		#endregion

		#region Methods
		public bool Equals(CrashDoc? other)
			=> other?.GetHashCode() == GetHashCode();

		public override bool Equals(object? obj)
		{
			if (obj is not CrashDoc other) return false;
			return Equals(other);
		}

		public override int GetHashCode() => _id.GetHashCode();

		#endregion

		// Disposal

		public void Dispose()
		{
			LocalClient?.StopAsync();
			LocalServer?.Stop();
		}

	}

}
