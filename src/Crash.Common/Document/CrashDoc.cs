using System.Runtime.CompilerServices;

using Crash.Client;
using Crash.Common.Tables;
using Crash.Communications;
using Crash.Events;

[assembly: InternalsVisibleTo("Crash.Common.Tests")]
namespace Crash.Common.Document
{

	/// <summary>The Crash Document</summary>
	public sealed class CrashDoc : IEquatable<CrashDoc>, IDisposable
	{
		public readonly Guid Id;

		#region Tables
		/// <summary>The Users Table for the Crash Doc</summary>
		public readonly UserTable Users;
		/// <summary>The Changes Table for the Crash Doc</summary>
		public readonly ChangeTable CacheTable;
		/// <summary>The Camera Table for the crash Doc</summary>
		public readonly CameraTable Cameras;
		#endregion

		#region Connectivity
		/// <summary>The Local Client for the Crash Doc</summary>
		public CrashClient? LocalClient { get; set; }
		/// <summary>The Local Server for the Crash Doc</summary>
		public CrashServer? LocalServer { get; set; }
		#endregion

		#region Queue
		/// <summary>The Idle Queue for the Crash Document</summary>
		public IdleQueue Queue { get; private set; }

		#endregion

		#region constructors

		/// <summary>Constructs a Crash Doc</summary>
		public CrashDoc()
		{
			Id = Guid.NewGuid();

			Users = new UserTable(this);
			CacheTable = new ChangeTable(this);
			Cameras = new CameraTable(this);

			Queue = new IdleQueue(this);
		}

		#endregion

		#region Methods
		/// <inheritdoc/>
		public bool Equals(CrashDoc? other)
			=> other?.GetHashCode() == GetHashCode();

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			if (obj is not CrashDoc other) return false;
			return Equals(other);
		}

		/// <inheritdoc/>
		public override int GetHashCode() => Id.GetHashCode();

		#endregion

		/// <inheritdoc/>
		public void Dispose()
		{
			LocalClient?.StopAsync();
			LocalServer?.Stop();
		}

	}

}
