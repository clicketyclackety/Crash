using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
	public sealed class ClientManager
	{
		public const string CrashPath = "Crash";
		public static string LastUrl = "http://localhost";
		public static string UrlAndPort => $"{LastUrl}{PortExt}";
		private static string PortExt => LastPort <= 0 ? string.Empty : $":{LastPort}";
		public static int LastPort = 5000;
		public static Uri ClientUri => new Uri($"{UrlAndPort}/{CrashPath}");

		private CrashDoc crashDoc;

		/// <summary>
		/// Method to load the client
		/// </summary>
		/// <param name="uri">the uri of the client</param>
		public async Task StartOrContinueLocalClient(CrashDoc crashDoc, Uri uri)
		{
			if (null == crashDoc) return;

			string userName = crashDoc?.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(userName))
			{
				throw new System.Exception("A User has not been assigned!");
			}

			CrashClient client = new CrashClient(userName, uri);
			crashDoc.LocalClient = client;

			client.OnInitialize += Init;

			// TODO : Check for successful connection
			await client.StartAsync();
		}

		/// <summary>
		/// Closes the local Client
		/// </summary>
		public async Task CloseLocalClient()
		{
			var client = crashDoc?.LocalClient;
			if (null == client) return;

			await client.StopAsync();

			client = null;
			crashDoc.Dispose();
		}

		// Move these?

		private void Init(IEnumerable<Change> Changes)
		{
			crashDoc.LocalClient.OnInitialize -= Init;

			Rhino.RhinoApp.WriteLine("Loading Changes ...");

			crashDoc.CacheTable.IsInit = true;
			try
			{
				_HandleChangesAsync(Changes);
			}
			catch
			{

			}
			finally
			{
				crashDoc.CacheTable.IsInit = false;
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

			crashDoc.Users?.Add(change.Owner);

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
			var rDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Unlock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task HandleUnlockAsync(Change change)
		{
			var rDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);
			if (crashDoc.CacheTable.TryGetValue(change.Id, out GeometryChange cachedChange))
			{
				rDoc.Objects.Lock(cachedChange.RhinoId, true);
			}

			await Task.CompletedTask;
		}

		private async Task _HandleTemporaryAddAsync(Change change)
		{
			GeometryChange geomChange = new GeometryChange(change);
			crashDoc.CacheTable.AddToDocument(geomChange);

			await Task.CompletedTask;
		}

		private async Task _HandleTransformAsync(Change change)
		{
			TransformChange transformChange = new TransformChange(change);
			if (!crashDoc.CacheTable.TryGetValue(change.Id, out ICachedChange cachedChange))
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

			BakeArgs bakeArgs = new BakeArgs(crashDoc, localChange);
			IdleAction bakeAction = new IdleAction(Bake, bakeArgs);
			crashDoc.Queue.AddAction(bakeAction);

			await Task.CompletedTask;
		}

		private void Remove(CrashEventArgs args)
		{
			if (args is not DeleteArgs deleteArgs) return;

			var rhinoDoc = CrashDocRegistry.GetRelatedDocument(deleteArgs.CrashDoc);
			if (!crashDoc.CacheTable.TryGetValue(deleteArgs.ChangeId, out GeometryChange change)) return;

			var rhinoObject = rhinoDoc.Objects.FindId(change.RhinoId);
			rhinoDoc.Objects.Delete(rhinoObject, true, true);
		}

		private async Task _HandleRemoveAsync(Change change)
		{
			DeleteArgs removeArgs = new DeleteArgs(crashDoc, change.Id);
			IdleAction deleteAction = new IdleAction(Remove, removeArgs);
			crashDoc.Queue.AddAction(deleteAction);

			await Task.CompletedTask;
		}

		// TODO : Add in order ofDate?
		private async Task _HandleCameraAsync(Change change)
		{
			CameraChange cameraChange = new CameraChange(change);
			crashDoc?.Cameras?.TryAddCamera(cameraChange);

			await Task.CompletedTask;
		}
	}

}
