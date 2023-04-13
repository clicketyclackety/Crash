using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Crash.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins;
using Crash.Handlers.Tests.Server;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class EventDispatcherTests
	{
		readonly RhinoDoc _doc;
		readonly CrashDoc _cDoc;

		public readonly IChangeDefinition TestChangeDefinition;
		public readonly EventDispatcher Dispatcher;

		public IChange Change(ChangeAction action) => new CustomChange(action);

		[SetUp]
		public void SetUp()
		{
			ServerUtils.StartDockerContainer();
		}

		public EventDispatcherTests()
		{
			_doc = RhinoDoc.CreateHeadless(null);
			_cDoc = new CrashDoc();
			TestChangeDefinition = new CustomDefinition();

			// Dispatcher = new EventDispatcher(_cDoc);
			// Dispatcher.RegisterDefinition(TestChangeDefinition);
			// registerCustomEvents();

			// Start Docker Container

			// Start Local CrashDoc
		}

		private void registerCustomEvents()
		{
			foreach (var changeAction in Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>())
			{
				CustomEvent += (sender, args) => Dispatcher.NotifyDispatcher(changeAction, sender, args, _doc);
			}
		}

		#region Custom Test Classes

		public sealed class CustomEventArgs : EventArgs
		{
			public readonly ChangeAction Value;
			public CustomEventArgs(ChangeAction changeAction)
			{
				Value = changeAction;
			}
		}

		public sealed class CustomDefinition : IChangeDefinition
		{
			public static BoundingBox TestBox => new BoundingBox(-100, -100, -100, 100, 100, 100);

			public Type ChangeType => typeof(CustomChange);

			public string ChangeName => $"Test.{nameof(CustomChange)}";

			public IEnumerable<IChangeCreateAction> CreateActions { get; set; }

			public IEnumerable<IChangeRecieveAction> RecieveActions { get; set; }

			public CustomDefinition()
			{
				CreateActions = getCreateActions();
				RecieveActions = getRecieveActions();
			}

			private IEnumerable<IChangeCreateAction> getCreateActions()
			{
				foreach (var changeAction in Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>())
				{
					yield return new Create(changeAction);
				}
			}

			private IEnumerable<IChangeRecieveAction> getRecieveActions()
			{
				foreach (var changeAction in Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>())
				{
					yield return new Recieve(changeAction);
				}
			}

			public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
			{
				Assert.That(drawArgs, Is.Not.Null);
				Assert.That(material, Is.Not.Null);
				Assert.That(change, Is.Not.Null);
				Assert.That(change, Is.TypeOf<CustomChange>());
			}

			public BoundingBox GetBoundingBox(IChange change)
			{
				Assert.That(change, Is.Not.Null);
				Assert.That(change, Is.TypeOf<CustomChange>());

				return TestBox;
			}

		}

		public sealed class Create : IChangeCreateAction
		{
			public ChangeAction Action { get; }

			public Create(ChangeAction action)
			{
				Action = action;
			}

			public bool CanConvert(object sender, CreateRecieveArgs crashArgs)
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(crashArgs, Is.Not.Null);

				Assert.That(crashArgs.Args is CustomEventArgs customArgs, Is.True);
				// customArgs.Value

				return true;
			}

			public bool TryConvert(object sender, CreateRecieveArgs crashArgs, out IEnumerable<IChange> changes)
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(crashArgs, Is.Not.Null);
				Assert.That(crashArgs.Args is CustomEventArgs customArgs, Is.True);

				changes = Array.Empty<IChange>();

				OnCreateEvent?.Invoke(sender, crashArgs);

				return true;
			}
		}

		public sealed class Recieve : IChangeRecieveAction
		{
			public ChangeAction Action { get; }

			public Recieve(ChangeAction action)
			{
				Action = action;
			}

			public async Task OnRecieveAsync(CrashDoc crashDoc, Change recievedChange)
			{
				Assert.That(crashDoc, Is.Not.Null);
				Assert.That(recievedChange, Is.Not.Null);

				// Assert Event
				OnRecieveEvent?.Invoke(recievedChange, null);
			}
		}

		public sealed class CustomChange : IChange
		{
			public const string TYPE = nameof(CustomChange);

			public DateTime Stamp { get; set; }

			public Guid Id { get; set; }

			public string Owner { get; set; }

			public string Payload { get; set; }

			public string Type { get; set; }

			public ChangeAction Action { get; set; }

			public CustomChange() { }

			public CustomChange(ChangeAction action)
			{
				Stamp = DateTime.Now;
				Id = Guid.NewGuid();
				Owner = "Test";
				Payload = "Payload Test";
				Action = action;
				Type = TYPE;
			}


		}

		#endregion

		#region Events

		public event EventHandler<CustomEventArgs> CustomEvent;
		public static event EventHandler OnRecieveEvent;
		public static event EventHandler OnCreateEvent;

		#endregion

		// These tests are twofold
		// 1. We're testing classes, interfaces etc.
		// 2. We're ensuring any pre-existing Definitions are superceeded

		[TestCaseSource(nameof(TestServerChanges))]
		public void TestRecieve(Change serverChange)
		{
			CrashDoc crashDoc = new CrashDoc();
			bool recieved = false;
			Change recievedChange = null;

			EventHandler _event = (sender, args) =>
			{
				recieved = true;
				Assert.That(sender is Change, Is.True);
				if (sender is Change sentChange)
				{
					recievedChange = sentChange;
				}
			};

			OnRecieveEvent += _event;

			// Assert
			Assert.That(recieved, Is.False);
			Dispatcher.NotifyDispatcherAsync(crashDoc, serverChange);
			Assert.That(recieved, Is.True);

			Assert.That(serverChange.Id, Is.EqualTo(recievedChange.Id));

			OnRecieveEvent -= _event;
		}

		[TestCaseSource(nameof(TestServerArgs))]
		public void TestCreate(CustomEventArgs args)
		{
			bool recieved = false;
			CustomEventArgs recievedArgs = null;

			EventHandler _event = (sender, args) =>
			{
				recieved = true;
				Assert.That(args is CustomEventArgs, Is.True);
				if (args is CustomEventArgs sendArgs)
				{
					recievedArgs = sendArgs;
				}
			};

			OnCreateEvent += _event;

			// Assert
			Assert.That(recieved, Is.False);
			CustomEvent?.Invoke(this, args);
			Assert.That(recieved, Is.True);

			Assert.That(recievedArgs.Value, Is.EqualTo(args.Value));

			OnCreateEvent -= _event;
		}

		[TestCaseSource(nameof(RandomChanges))]
		public void TestRandomRecieve(DateTime date, Guid id, string owner, string payload, string type, ChangeAction action)
		{
			IChange change = new CustomChange()
			{
				Stamp = date,
				Id = id,
				Owner = owner,
				Payload = payload,
				Type = type,
				Action = action,
			};

			// Dispatcher.NotifyDispatcher();
		}

		public static IEnumerable TestServerChanges
		{
			get
			{
				var owner = Path.GetRandomFileName().Replace(".", "");
				string payload = "Example Payload";

				foreach (var changeAction in Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>())
				{
					yield return new Change(Guid.NewGuid(), owner, payload)
					{
						Type = CustomChange.TYPE,
						Action = changeAction,
					};
				}
			}
		}

		public static IEnumerable TestServerArgs
		{
			get
			{
				foreach (var changeAction in Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>())
				{
					yield return new CustomEventArgs(changeAction);
				}
			}
		}

		public static IEnumerable RandomChanges
		{
			get
			{
				var changes = Enum.GetValues(typeof(ChangeAction)).Cast<ChangeAction>().ToArray();

				for (int i = 0; i < 100; i++)
				{
					var dateTime = new DateTime(TestContext.CurrentContext.Random.NextLong());
					var id = Guid.NewGuid();
					var owner = Path.GetRandomFileName().Replace(".", "");
					var payload = id.ToString(); // Should this be something useful?
					var type = CustomChange.TYPE;

					int changeIndex = TestContext.CurrentContext.Random.Next(0, changes.Length);
					var action = changes[changeIndex];

					yield return new TestCaseData(dateTime, id, owner, payload, type, action);
				}

			}
		}

	}

}
