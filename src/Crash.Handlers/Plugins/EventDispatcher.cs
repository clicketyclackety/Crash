using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins.Initializers;

using Rhino;
using Rhino.Display;

namespace Crash.Handlers.Plugins
{

	// This needs updating
	public sealed class EventDispatcher
	{
		private readonly CrashDoc Doc;

		private readonly Dictionary<ChangeAction, List<IChangeCreateAction>> _createActions;
		private readonly Dictionary<string, List<IChangeRecieveAction>> _recieveActions;

		public EventDispatcher(CrashDoc doc)
		{
			Doc = doc;

			_createActions = new();
			_recieveActions = new();

			RegisterDefaultEvents();
			RegisterDefaultServerCalls();
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
		public void NotifyDispatcher(ChangeAction changeAction, object sender, EventArgs args, RhinoDoc doc)
		{
			if (!_createActions.TryGetValue(changeAction, out var actionChain)) return;
			var crashArgs = new CreateRecieveArgs(changeAction, args, doc);
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
							case ChangeAction.Add:
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
								tasks.Add(Doc.LocalClient.UnselectAsync(change.Id));
								break;
							case ChangeAction.Unlock:
								tasks.Add(Doc.LocalClient.SelectAsync(change.Id));
								break;

							case ChangeAction.Camera:
								tasks.Add(Doc.LocalClient.CameraChangeAsync(change));
								break;
						}
					}

					Task.WhenAll(tasks);

					return;
				}
			}
		}

		public void NotifyDispatcher(Change change)
		{
			if (!_recieveActions.TryGetValue(change.Type, out List<IChangeRecieveAction> recievers)) return;
			foreach (IChangeRecieveAction action in recievers)
			{
				action.OnRecieve(Doc, change);
			}
		}

		private void RegisterDefaultEvents()
		{
			// Object Events
			RhinoDoc.AddRhinoObject += (sender, args) => NotifyDispatcher(ChangeAction.Add, sender, args, args.TheObject.Document);
			RhinoDoc.UndeleteRhinoObject += (sender, args) => NotifyDispatcher(ChangeAction.Add, sender, args, args.TheObject.Document);
			RhinoDoc.DeleteRhinoObject += (sender, args) => NotifyDispatcher(ChangeAction.Remove, sender, args, args.TheObject.Document);

			RhinoDoc.BeforeTransformObjects += (sender, args) => NotifyDispatcher(ChangeAction.Transform, sender, args, args.Objects.First(o => o is not null).Document);

			RhinoDoc.DeselectObjects += (sender, args) => NotifyDispatcher(ChangeAction.Unlock, sender, args, args.Document);
			RhinoDoc.DeselectAllObjects += (sender, args) => NotifyDispatcher(ChangeAction.Unlock, sender, args, args.Document);
			RhinoDoc.SelectObjects += (sender, args) => NotifyDispatcher(ChangeAction.Lock, sender, args, args.Document);

			RhinoDoc.ModifyObjectAttributes += (sender, args) => NotifyDispatcher(ChangeAction.Update, sender, args, args.Document);
			RhinoDoc.UserStringChanged += (sender, args) => NotifyDispatcher(ChangeAction.Update, sender, args, args.Document);

			// Doc Events
			// RhinoDoc.UnitsChangedWithScaling += 

			// View Events
			RhinoView.Modified += (sender, args) => NotifyDispatcher(ChangeAction.Camera, sender, args, args.View.Document);
		}

		private void RegisterDefaultServerCalls()
		{
			Doc.LocalClient.OnAdd += (name, change) => NotifyDispatcher(change);

			// These are all missing a Change Type! It will need explicitly mentioning!
			Doc.LocalClient.OnDelete += (name, changeGuid) => NotifyDispatcher(DeleteChange(changeGuid, name));
			Doc.LocalClient.OnSelect += (name, changeGuid) => NotifyDispatcher(SelectChange(changeGuid, name));
			Doc.LocalClient.OnUnselect += (name, changeGuid) => NotifyDispatcher(UnSelectChange(changeGuid, name));

			// How does this get handled?
			Doc.LocalClient.OnDone += (name) => NotifyDispatcher(new Change(Guid.Empty, name, null));

			// This works better than I expected
			Doc.LocalClient.OnInitialize += (changes) =>
			{
				foreach (var change in changes)
				{
					NotifyDispatcher(change);
				}
			};
		}

		private Change DoneChange(string name)
			=> new Change()
			{
				Owner = name,
				Action = ChangeAction.None,
				Type = new DoneDefinition().ChangeName
			};

		private Change DeleteChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = new GeometryChange().Type,
				Action = ChangeAction.Remove
			};

		private Change SelectChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = new GeometryChange().Type,
				Action = ChangeAction.Lock
			};

		private Change UnSelectChange(Guid id, string name)
			=> new Change()
			{
				Id = id,
				Owner = name,
				Type = new GeometryChange().Type,
				Action = ChangeAction.Unlock
			};

	}

}
