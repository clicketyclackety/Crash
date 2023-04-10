using System.Collections;
using System.Collections.Generic;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.InternalEvents;
using Crash.Handlers.Plugins;
using Crash.Handlers.Plugins.Geometry.Create;

using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class GeometryCreateRemoveActionTests
	{

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_CanConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			CrashDoc _cdoc = new CrashDoc();
			var createArgs = new CreateRecieveArgs(ChangeAction.Add, createRecieveArgs, _cdoc);
			var createAction = new GeometryCreateAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_TryConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			CrashDoc _cdoc = new CrashDoc();
			var createArgs = new CreateRecieveArgs(ChangeAction.Add, createRecieveArgs, _cdoc);
			var createAction = new GeometryCreateAction();
			Assert.That(createAction.TryConvert(sender, createArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(createArgs.Action));
				Assert.That(change is GeometryChange, Is.True);
			}
		}

		[TestCaseSource(nameof(GeometryRemoveArgs))]
		public void GeometryRemoveAction_CanConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			CrashDoc _cdoc = new CrashDoc();
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryRemoveArgs))]
		public void GeometryRemoveAction_TryConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			CrashDoc _cdoc = new CrashDoc();
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();

			GeometryChange cache = GeometryChange.CreateNew("Test", createRecieveArgs.Geometry);
			cache.Id = createRecieveArgs.ChangeId;

			_cdoc.CacheTable.UpdateChangeAsync(cache);

			Assert.That(createAction.TryConvert(sender, createArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(createArgs.Action));
				Assert.That(change is Change, Is.True);
			}
		}


		public static IEnumerable GeometryCreateArgs
		{
			get
			{
				for (int i = 0; i < 10; i++)
				{
					LineCurve geom = NRhino.Random.Geometry.NLineCurve.Any();
					yield return new TestCaseData(new object(), new CrashObjectEventArgs(geom, Guid.Empty));
				}
			}
		}

		public static IEnumerable GeometryRemoveArgs
		{
			get
			{
				for (int i = 0; i < 10; i++)
				{
					LineCurve geom = NRhino.Random.Geometry.NLineCurve.Any();
					yield return new TestCaseData(new object(), new CrashObjectEventArgs(geom, Guid.NewGuid()));
				}
			}
		}

	}

}
