using Crash.Client;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;
using Crash.Events.Args;
using Crash.Handlers;

namespace Crash.Utilities
{

	/// <summary>
	/// The request manager
	/// </summary>
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

		private async Task _HandleChangeAsync(Change change)
		{
			if (null == change || string.IsNullOrEmpty(change.Owner)) return;

			_crashDoc.Users?.Add(change.Owner);
			string currentUser = _crashDoc.Users.CurrentUser.Name;

			ChangeAction _action = (ChangeAction)change.Action;
			if (_action.HasFlag(ChangeAction.Camera))
			{
				await _HandleCameraAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Add) && _action.HasFlag(ChangeAction.Temporary) && change.Owner != currentUser)
			{
				await _HandleTemporaryAddAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Add))
			{
				await _HandleAddAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Remove))
			{
				await _HandleRemoveAsync(change.Id);
			}

			if (_action.HasFlag(ChangeAction.Lock))
			{
				await _HandleLockAsync(change.Id);
			}
			else if (_action.HasFlag(ChangeAction.Unlock))
			{
				await HandleUnlockAsync(change.Id);
			}

			if (_action.HasFlag(ChangeAction.Transform))
			{
				await _HandleTransformAsync(change);
			}

			await Task.CompletedTask;
		}

		private async Task _HandleLockAsync(Guid changeId)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(_crashDoc);
			if (_crashDoc.CacheTable.TryGetValue(changeId, out GeometryChange cachedChange))
			{
				// Update Cache Table
				rDoc.Objects.Lock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task HandleUnlockAsync(Guid changeId)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(_crashDoc);
			if (_crashDoc.CacheTable.TryGetValue(changeId, out GeometryChange cachedChange))
			{
				// Update Cache Table
				rDoc.Objects.Unlock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task _HandleTemporaryAddAsync(Change change)
		{
			GeometryChange geomChange = new GeometryChange(change);

			BakeArgs bakeArgs = new BakeArgs(_crashDoc, geomChange);
			IdleAction bakeAction = new IdleAction(AddToCache, bakeArgs);
			_crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private void AddToCache(IdleArgs args)
		{
			if (args is not BakeArgs bakeArgs) return;
			args.CrashDoc.CacheTable.UpdateChangeAsync(bakeArgs.Change);
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

		private void Bake(IdleArgs args)
		{
			if (args is not BakeArgs bakeArgs) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(bakeArgs.CrashDoc);

			Guid rhinoId = rhinoDoc.Objects.Add(bakeArgs.Geometry);
			var rhinoObject = rhinoDoc.Objects.FindId(rhinoId);
			Crash.Utils.ChangeUtils.SyncHost(rhinoObject, bakeArgs.Change);

			bakeArgs.CrashDoc.CacheTable.UpdateChangeAsync(bakeArgs.Change);
		}

		private async Task _HandleAddAsync(Change change)
		{
			GeometryChange localChange = new GeometryChange(change);
			_HandleAddAsync(localChange);
		}

		private async Task _HandleAddAsync(GeometryChange geomChange)
		{
			BakeArgs bakeArgs = new BakeArgs(_crashDoc, geomChange);
			IdleAction bakeAction = new IdleAction(Bake, bakeArgs);
			_crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private void Remove(IdleArgs args)
		{
			if (args is not DeleteArgs deleteArgs) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(deleteArgs.CrashDoc);
			if (!_crashDoc.CacheTable.TryGetValue(deleteArgs.ChangeId, out GeometryChange change)) return;

			var rhinoObject = rhinoDoc.Objects.FindId(change.RhinoId);
			rhinoDoc.Objects.Delete(rhinoObject, true, true);
			_crashDoc.CacheTable.RemoveChange(change.Id);
		}

		private async Task _HandleRemoveAsync(Guid changeId)
		{
			DeleteArgs removeArgs = new DeleteArgs(_crashDoc, changeId);
			IdleAction deleteAction = new IdleAction(Remove, removeArgs);
			_crashDoc.Queue.AddAction(deleteAction);

			await Task.CompletedTask;
		}

		// TODO : Add in order of Date?
		private async Task _HandleCameraAsync(Change change)
		{
			CameraChange cameraChange = new CameraChange(change);
			_crashDoc?.Cameras?.TryAddCamera(cameraChange);

			await Task.CompletedTask;
		}

		private async Task _HandleDoneAsync(string user)
		{
			_crashDoc.CacheTable.SomeoneIsDone = true;

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
			}

			_crashDoc.Queue.OnCompletedQueue += Queue_OnCompletedQueue;
		}

		private void Queue_OnCompletedQueue(object sender, EventArgs e)
		{
			_crashDoc.Queue.OnCompletedQueue -= Queue_OnCompletedQueue;
			_crashDoc.CacheTable.SomeoneIsDone = false;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}

}
