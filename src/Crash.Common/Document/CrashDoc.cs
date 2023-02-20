using Crash.Client;
using Crash.Common.Tables;
using Crash.Events;

namespace Crash.Common.Document
{

	public sealed class CrashDoc : IEquatable<CrashDoc>, IDisposable
	{
		private readonly Guid id;

		#region Tables

		public readonly UserTable Users;

		public readonly ChangeTable CacheTable;

		public readonly CameraTable Cameras;

		#endregion

		#region Connectivity

		public CrashClient? LocalClient { get; internal set; }

		public CrashServer? LocalServer { get; internal set; }

		#endregion

		#region Queue

		public IdleQueue Queue { get; private set; }

		#endregion

		#region constructors

		public CrashDoc()
		{
			Users = new UserTable(this);
			CacheTable = new ChangeTable(this);
			Cameras = new CameraTable(this);
			Queue = new IdleQueue(this);
			id = Guid.NewGuid();
		}


		internal static CrashDoc CreateHeadless()
		{
			return new CrashDoc();
		}

		#endregion

		#region Methods
		public bool Equals(CrashDoc other)
		{
			if (null == other) return false;
			return this.id == other.id;
		}

		#endregion

		// Disposal

		public void Dispose()
		{
			LocalClient?.StopAsync();
			LocalServer?.Stop();
		}

	}

}
