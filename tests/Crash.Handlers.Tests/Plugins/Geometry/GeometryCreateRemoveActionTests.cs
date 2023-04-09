using System.Collections;
using System.Collections.Generic;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.InternalEvents;
using Crash.Handlers.Plugins;
using Crash.Handlers.Plugins.Geometry.Create;

using Rhino;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class GeometryCreateRemoveActionTests
	{
		private readonly RhinoDoc _doc;
		private readonly CrashDoc _cdoc;

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_CanConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Add, createRecieveArgs, _cdoc);
			var createAction = new GeometryCreateAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_TryConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
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
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryRemoveArgs))]
		public void GeometryRemoveAction_TryConvert(object sender, CrashObjectEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();

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
					yield return new CrashObjectEventArgs(geom);
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
					yield return new CrashObjectEventArgs(geom, Guid.NewGuid());
				}
			}
		}

		private static Point3d RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new Point3d(x, y, z);
		}

	}

}
