using Crash.Common.Changes;
using Crash.Geometry;
using Crash.Handlers.InternalEvents;

namespace Crash.Handlers.Plugins.Camera.Create
{

	/// <summary>Creates a Camera from a View Event</summary>
	internal sealed class CameraCreateAction : IChangeCreateAction
	{
		/// <inheritdoc/>
		public ChangeAction Action => ChangeAction.Camera;

		DateTime lastSentTime;
		CPoint lastLocation;
		CPoint lastTarget;
		static TimeSpan maxPerSecond = TimeSpan.FromMilliseconds(250);

		/// <summary>Default Constructor</summary>
		internal CameraCreateAction()
		{
			lastSentTime = DateTime.MinValue;
			lastLocation = CPoint.None;
			lastTarget = CPoint.None;
		}

		private double DistanceBetween(CPoint p1, CPoint p2)
		{
			// https://www.mathsisfun.com/algebra/distance-2-points.html
			double dist = Math.Sqrt(
							Math.Pow(p1.X - p2.X, 2) +
							Math.Pow(p1.Y - p2.Y, 2) +
							Math.Pow(p1.Z - p2.Z, 2)
						  );

			return dist;
		}

		/// <inheritdoc/>
		public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
		{
			if (crashArgs.Args is not CrashViewArgs viewArgs) return false;
			DateTime now = DateTime.UtcNow;
			TimeSpan timeSinceLastSent = now - lastSentTime;
			if (timeSinceLastSent < maxPerSecond)
			{
				return false;
			}

			if (DistanceBetween(viewArgs.Location, lastLocation) < 10 &&
				DistanceBetween(viewArgs.Target, lastTarget) < 10)
			{
				return false;
			}

			lastLocation = viewArgs.Location;
			lastTarget = viewArgs.Target;
			lastSentTime = DateTime.UtcNow;

			return true;
		}

		/// <inheritdoc/>
		public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
		{
			changes = Array.Empty<IChange>();
			if (crashArgs.Args is not CrashViewArgs viewArgs)
			{
				changes = null;
				return false;
			}

			var userName = crashArgs.Doc.Users.CurrentUser.Name;

			var camera = new Common.View.Camera(viewArgs.Location, viewArgs.Target);

			changes = new List<IChange> { CameraChange.CreateNew(camera, userName) };

			return true;
		}

	}

}
