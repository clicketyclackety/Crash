using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Crash.Changes;
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
	public sealed class ClientState
	{

		private CrashDoc _crashDoc;

		public ClientState(CrashDoc crashDoc)
		{
			this._crashDoc = crashDoc;
		}

		public void Init(IEnumerable<Change> Changes)
		{
			_crashDoc.LocalClient.OnInitialize -= Init;

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

		private async Task _HandleChangesAsync(IEnumerable<Change> Changes)
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

			ChangeAction _action = (ChangeAction)change.Action;
			if (_action.HasFlag(ChangeAction.Camera))
			{
				await _HandleCameraAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Add) && _action.HasFlag(ChangeAction.Temporary))
			{
				await _HandleTemporaryAddAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Add))
			{
				await _HandleAddsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Remove))
			{
				await _HandleRemoveAsync(change);
			}

			if (_action.HasFlag(ChangeAction.Lock))
			{
				await _HandleLockAsync(change);
			}
			else if (_action.HasFlag(ChangeAction.Unlock))
			{
				await HandleUnlockAsync(change);
			}

			if (_action.HasFlag(ChangeAction.Transform))
			{
				await _HandleTransformAsync(change);
			}

			await Task.CompletedTask;
		}

		private async Task _HandleLockAsync(Change change)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(_crashDoc);
			if (_crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Unlock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task HandleUnlockAsync(Change change)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(_crashDoc);
			if (_crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Lock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task _HandleTemporaryAddAsync(Change change)
		{
			GeometryChange geomChange = new GeometryChange(change);
			_crashDoc.CacheTable.AddToDocument(geomChange);

			await Task.CompletedTask;
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

		private void Bake(CrashEventArgs args)
		{
			if (args is not BakeArgs bakeArgs) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(bakeArgs.CrashDoc);

			Guid rhinoId = rhinoDoc.Objects.Add(bakeArgs.Geometry);
			var rhinoObject = rhinoDoc.Objects.FindId(rhinoId);
			Crash.Utils.ChangeUtils.SyncHost(rhinoObject, bakeArgs.Change);
		}

		private async Task _HandleAddsync(Change change)
		{
			GeometryChange localChange = new GeometryChange(change);

			BakeArgs bakeArgs = new BakeArgs(_crashDoc, localChange);
			IdleAction bakeAction = new IdleAction(Bake, bakeArgs);
			_crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private void Remove(CrashEventArgs args)
		{
			if (args is not DeleteArgs deleteArgs) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(deleteArgs.CrashDoc);
			if (!_crashDoc.CacheTable.TryGetValue(deleteArgs.ChangeId, out GeometryChange change)) return;

			var rhinoObject = rhinoDoc.Objects.FindId(change.RhinoId);
			rhinoDoc.Objects.Delete(rhinoObject, true, true);
		}

		private async Task _HandleRemoveAsync(Change change)
		{
			DeleteArgs removeArgs = new DeleteArgs(_crashDoc, change.Id);
			IdleAction deleteAction = new IdleAction(Remove, removeArgs);
			_crashDoc.Queue.AddAction(deleteAction);

			await Task.CompletedTask;
		}

		// TODO : Add in order ofDate?
		private async Task _HandleCameraAsync(Change change)
		{
			CameraChange cameraChange = new CameraChange(change);
			_crashDoc?.Cameras?.TryAddCamera(cameraChange);

			await Task.CompletedTask;
		}

	}

}
