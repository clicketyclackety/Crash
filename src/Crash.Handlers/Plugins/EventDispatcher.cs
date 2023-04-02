using Crash.Common.Document;

using Rhino;
using Rhino.Display;

namespace Crash.Handlers.Plugins
{

	// This needs updating
	public sealed class EventDispatcher
	{
		private readonly CrashDoc Doc;

		private readonly Dictionary<ChangeAction, List<IChangeCreateAction>> _registeredEvents;
		private readonly Dictionary<string, IChangeDefinition> definitions;

		public EventDispatcher()
		{
			_registeredEvents = new();
			definitions = new();
			RegisterDefaultEvents();
		}

		public void RegisterAction(IChangeCreateAction changeCreate)
		{
			if (_registeredEvents.TryGetValue(changeCreate.Action, out var actions))
			{
				actions.Add(changeCreate);
			}
			else
			{
				_registeredEvents.Add(changeCreate.Action, new List<IChangeCreateAction> { changeCreate });
			}
		}

		// How can we prevent the same events being subscribed multiple times
		private void NotifyDispatcher(ChangeAction changeAction, object sender, EventArgs args, RhinoDoc doc)
		{
			if (!_registeredEvents.TryGetValue(changeAction, out var actionChain)) return;
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

		private void NotifyDispatcher(Change change)
		{
			if (!definitions.TryGetValue(change.Type, out IChangeDefinition definition)) return;
			foreach (IChangeRecieveAction action in definition.RecieveActions)
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
			Doc.LocalClient.OnDelete += (name, changeGuid) => NotifyDispatcher(new Change(changeGuid, name, null));
			Doc.LocalClient.OnSelect += (name, changeGuid) => NotifyDispatcher(new Change(changeGuid, name, null));
			Doc.LocalClient.OnUnselect += (name, changeGuid) => NotifyDispatcher(new Change(changeGuid, name, null));

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

	}

}
