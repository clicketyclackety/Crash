using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.InternalEvents;
using Crash.Handlers.Plugins.Initializers;
using Crash.Utils;

using Rhino;
using Rhino.Display;

namespace Crash.Handlers.Plugins
{

	// This needs updating
	public sealed class EventDispatcher
	{

		private readonly Dictionary<ChangeAction, List<IChangeCreateAction>> _createActions;
		private readonly Dictionary<string, List<IChangeRecieveAction>> _recieveActions;

		public static EventDispatcher Instance;

		public EventDispatcher()
		{
			Instance = this;

			_createActions = new();
			_recieveActions = new();

			RegisterDefaultEvents();
		}

		public void RegisterDefinition(IChangeDefinition definition)
		{
			foreach (var create in definition.CreateActions)
			{
				if (_createActions.TryGetValue(create.Action, out var actions))
				{
					actions.Add(create);
				}
				else
				{
					_createActions.Add(create.Action, new List<IChangeCreateAction> { create });
				}
			}

			foreach (var recieve in definition.RecieveActions)
			{
				if (_recieveActions.TryGetValue(definition.ChangeName, out var recievers))
				{
					recievers.Add(recieve);
				}
				else
				{
					_recieveActions.Add(definition.ChangeName, new List<IChangeRecieveAction> { recieve });
				}
			}
		}

		// How can we prevent the same events being subscribed multiple times
		public async Task NotifyDispatcher(ChangeAction changeAction, object sender, EventArgs args, RhinoDoc doc)
		{
			if (!_createActions.TryGetValue(changeAction, out var actionChain)) return;
			var crashArgs = new CreateRecieveArgs(changeAction, args, doc);

			CrashDoc? Doc = CrashDocRegistry.GetRelatedDocument(doc);
			if (null == Doc) return;

			foreach (var action in actionChain)
			{
				if (!action.CanConvert(sender, crashArgs)) continue;
				if (action.TryConvert(sender, crashArgs, out var changes))
				{
					List<Task> tasks = new List<Task>(changes.Count());
					foreach (var ichange in changes)
					{
						Change change = new Change(ichange);
						switch (change.Action)
						{
							case ChangeAction.Add | ChangeAction.Temporary:
								tasks.Add(Doc.LocalClient.AddAsync(change));
								break;
							case ChangeAction.Remove:
								tasks.Add(Doc.LocalClient.DeleteAsync(change.Id));
								break;

							case ChangeAction.Transform:
								// tasks.Add(Doc.LocalClient.TransformAsync(change));
								break;

							case ChangeAction.Update:
								tasks.Add(Doc.LocalClient.UpdateAsync(change.Id, change));
								break;

							case ChangeAction.Lock:
								tasks.Add(Doc.LocalClient.SelectAsync(change.Id));
								break;
							case ChangeAction.Unlock:
								tasks.Add(Doc.LocalClient.UnselectAsync(change.Id));
								break;

							case ChangeAction.Camera:
								tasks.Add(Doc.LocalClient.CameraChangeAsync(change));
								break;
						}
					}

					await Task.WhenAll(tasks);

					return;
				}
			}
		}

		public async Task NotifyDispatcherAsync(CrashDoc Doc, Change change)
		{
			if (!_recieveActions.TryGetValue(change.Type, out List<IChangeRecieveAction>? recievers) ||
				null == recievers) return;

			await RegisterUserAsync(Doc, change);

			foreach (IChangeRecieveAction action in recievers)
			{
				if (action.Action != change.Action) continue;

				await action.OnRecieveAsync(Doc, change);
				return;
			}
		}

		private async Task RegisterUserAsync(CrashDoc doc, Change change)
		{
			doc.Users.Add(change.Owner);
		}

		private void RegisterDefaultEvents()
		{
			// Object Events
			RhinoDoc.AddRhinoObject += (sender, args) =>
			{
				//TODO: Is Init? Where is that checked for?
				CrashDoc crashDoc = CrashDocRegistry.GetRelatedDocument(args.TheObject.Document);
				if (crashDoc is not null)
				{
					if (crashDoc.CacheTable.IsInit) return;
					if (crashDoc.CacheTable.SomeoneIsDone) return;
				}

				var crashArgs = new CrashObjectEventArgs(args.TheObject);
				NotifyDispatcher(ChangeAction.Add | ChangeAction.Temporary, sender, crashArgs, args.TheObject.Document);
			};

			RhinoDoc.UndeleteRhinoObject += (sender, args) =>
			{
				//TODO: Is Init? Where is that checked for?
				CrashDoc crashDoc = CrashDocRegistry.GetRelatedDocument(args.TheObject.Document);
				if (crashDoc is not null)
				{
					if (crashDoc.CacheTable.IsInit) return;
					if (crashDoc.CacheTable.SomeoneIsDone) return;
				}

				var crashArgs = new CrashObjectEventArgs(args.TheObject);
				NotifyDispatcher(ChangeAction.Add | ChangeAction.Temporary, sender, crashArgs, args.TheObject.Document);
			};

			RhinoDoc.DeleteRhinoObject += (sender, args) =>
			{
				args.TheObject.TryGetChangeId(out Guid changeId);
				if (changeId == Guid.Empty) return;

				var crashArgs = new CrashObjectEventArgs(args.TheObject.Geometry, args.TheObject.Id, changeId);
				NotifyDispatcher(ChangeAction.Remove, sender, crashArgs, args.TheObject.Document);
			};

			RhinoDoc.BeforeTransformObjects += (sender, args) =>
			{
				var crashArgs = new CrashTransformEventArgs(args.Transform.ToCrash(), args.Objects.Select(o => new CrashObject(o)), args.ObjectsWillBeCopied);
				RhinoDoc rhinoDoc = args.Objects.FirstOrDefault(o => o.Document is not null).Document;
				NotifyDispatcher(ChangeAction.Transform, sender, crashArgs, rhinoDoc);
			};

			RhinoDoc.DeselectObjects += (sender, args) =>
			{
				var crashArgs = new CrashSelectionEventArgs(args.Selected, args.RhinoObjects.Select(o => new CrashObject(o)));
				NotifyDispatcher(ChangeAction.Unlock, sender, crashArgs, args.Document);
			};

			RhinoDoc.DeselectAllObjects += (sender, args) =>
			{
				var crashArgs = new CrashSelectionEventArgs(false);
				NotifyDispatcher(ChangeAction.Unlock, sender, crashArgs, args.Document);
			};
			RhinoDoc.SelectObjects += (sender, args) =>
			{
				var crashArgs = new CrashSelectionEventArgs(args.Selected, args.RhinoObjects.Select(o => new CrashObject(o)));
				NotifyDispatcher(ChangeAction.Lock, sender, crashArgs, args.Document);
			};

			RhinoDoc.ModifyObjectAttributes += (sender, args) =>
			{
				// TODO : Create Wrapper
				NotifyDispatcher(ChangeAction.Update, sender, args, args.Document);
			};

			RhinoDoc.UserStringChanged += (sender, args) =>
			{
				// TODO : Create Wrapper
				NotifyDispatcher(ChangeAction.Update, sender, args, args.Document);
			};

			// Doc Events
			// RhinoDoc.UnitsChangedWithScaling += 

			// View Events
			RhinoView.Modified += (sender, args) =>
			{
				var crashArgs = new CrashViewArgs(args.View);
				NotifyDispatcher(ChangeAction.Camera, sender, crashArgs, args.View.Document);
			};
		}

#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
		public void RegisterDefaultServerCalls(CrashDoc Doc)
		{
			Doc.LocalClient.OnAdd += async (name, change) => await NotifyDispatcherAsync(Doc, change);
			Doc.LocalClient.OnDelete += async (name, changeGuid) => await NotifyDispatcherAsync(Doc, DeleteChange(changeGuid, name));

			Doc.LocalClient.OnSelect += async (name, changeGuid) => await NotifyDispatcherAsync(Doc, SelectChange(changeGuid, name));
			Doc.LocalClient.OnUnselect += async (name, changeGuid) => await NotifyDispatcherAsync(Doc, UnSelectChange(changeGuid, name));

			Doc.LocalClient.OnDone += async (name) => await NotifyDispatcherAsync(Doc, EventDispatcher.DoneChange(name));

			Doc.LocalClient.OnCameraChange += async (user, change) => await NotifyDispatcherAsync(Doc, change);

			// OnInit is called on reconnect as well?
			Action<Change[]> _event = null;
			_event = async (changes) =>
			{
				Doc.LocalClient.OnInitialize -= _event;
				foreach (var change in changes)
				{
					await NotifyDispatcherAsync(Doc, change);
				}
			};

			Doc.LocalClient.OnInitialize += _event;
		}
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates

		private static Change DoneChange(string name)
			=> new Change()
			{
				Owner = name,
				Action = ChangeAction.None,
				Type = new DoneDefinition().ChangeName,
				Id = Guid.NewGuid(),
				Stamp = DateTime.UtcNow,
			};

		private static Change DeleteChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = GeometryChange.ChangeType,
				Action = ChangeAction.Remove,
				Stamp = DateTime.Now,
				Payload = null,
			};

		private static Change SelectChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = GeometryChange.ChangeType,
				Action = ChangeAction.Lock,
				Stamp = DateTime.Now,
				Payload = null,
			};

		private static Change UnSelectChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = GeometryChange.ChangeType,
				Action = ChangeAction.Unlock,
				Stamp = DateTime.Now,
				Payload = null,
			};

	}

}
