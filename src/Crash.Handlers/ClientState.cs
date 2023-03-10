using Crash.Client;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers;

namespace Crash.Utilities
{

	/// <summary>
	/// The request manager
	/// </summary>
	// TODO : Improve all of this.
	public sealed class ClientState : IDisposable
	{

		private CrashDoc _crashDoc;
		private CrashClient _localClient;

		public ClientState(CrashDoc crashDoc, CrashClient crashClient)
		{
			this._crashDoc = crashDoc;
			this._localClient = crashClient;
			RegisterServerCallEvents();
		}

		public void Init(IEnumerable<Change> Changes)
		{
			Rhino.RhinoApp.WriteLine("Loading Changes ...");

			_crashDoc.CacheTable.IsInit = true;
			try
			{
				_HandleChangesAsync(Changes);
			}
			catch
			{

			}
			finally
			{
				_crashDoc.CacheTable.IsInit = false;
			}
		}

		private void RegisterServerCallEvents()
		{
			_localClient.OnAdd += (name, change) => _HandleChangeAsync(change);
			_localClient.OnDelete += (name, id) => _HandleRemoveAsync(id);
			_localClient.OnDelete += (name, id) => _HandleRemoveAsync(id);
			_localClient.OnDone += (name) => _HandleDoneAsync(name);
			// _localClient.OnUpdate += (name, id) => _HandleChangeAsync(name)
			_localClient.OnSelect += (name, id) => _HandleLockAsync(id);
			_localClient.OnUnselect += (name, id) => HandleUnlockAsync(id);
			_localClient.OnCameraChange += (name, change) => _HandleChangeAsync(change);
		}

		private int Sorter(Change change)
		{
			ChangeAction action = (ChangeAction)change.Action;
			if (action.HasFlag(ChangeAction.Transform))
			{
				return 2048;
			}
			else if (action.HasFlag(ChangeAction.Update))
			{
				return 1024;
			}
			else if (action.HasFlag(ChangeAction.Camera))
			{
				return 512;
			}
			else if (action.HasFlag(ChangeAction.Temporary))
			{
				return 2;
			}
			else if (action.HasFlag(ChangeAction.Lock) || action.HasFlag(ChangeAction.Unlock))
			{
				return 1;
			}
			else if (action.HasFlag(ChangeAction.Add))
			{
				return 0;
			}

			return 1;
		}

		Dictionary<int, Func<Task, IChange>> ChangeHandler;

		public async Task _HandleChangesAsync(IEnumerable<Change> Changes)
		{
			var enumer = Changes.OrderBy(c => Sorter(c)).GetEnumerator();
			while (enumer.MoveNext())
			{
				try
				{
					await _HandleChangeAsync(enumer.Current);
				}
				catch { }
			}
		}

		private async Task _HandleChangeAsync(IChange change)
		{
			ChangeAction action = (ChangeAction)change.Action;
			action &= ~ChangeAction.Temporary;

			Task task = action switch
			{
				ChangeAction.Remove => _HandleRemoveAsync(change.Id),
				ChangeAction.Add => _HandleAddAsync(change),

				ChangeAction.Lock => _HandleLockAsync(change.Id),
				ChangeAction.Unlock => HandleUnlockAsync(change.Id),

				ChangeAction.Camera => _HandleCameraAsync(change),

				_ => Task.CompletedTask,
			};

			await task;
		}

		private async Task _HandleTransformAsync(Change change)
		{
			TransformChange transformChange = new TransformChange(change);
			if (!_crashDoc.CacheTable.TryGetValue(change.Id, out ICachedChange cachedChange))
			{
				if (cachedChange is not GeometryChange geomChange) return;
				geomChange.Geometry.Transform(transformChange.Transform.ToRhino());
			}

			await Task.CompletedTask;
		}

		private async Task _HandleDoneAsync(string user)
		{
			var changes = _crashDoc.CacheTable.GetChanges();
			foreach (var change in changes)
			{
				ChangeAction action = (ChangeAction)change.Action;
				if (!action.HasFlag(ChangeAction.Temporary)) continue;

				// Not temporary anymore!
				action &= ~ChangeAction.Temporary;
				change.Action = (int)action;

				if (change is GeometryChange geomChange)
					await _HandleAddAsync(geomChange);

				_crashDoc.CacheTable.RemoveChange(change.Id);
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}

}
