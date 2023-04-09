using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins;
using Crash.Handlers.Plugins.Geometry.Create;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class GeometryCreateRemoveActionTests
	{
		private readonly RhinoDoc _doc;
		private readonly CrashDoc _cdoc;

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_CanConvert(object sender, RhinoObjectEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Add, createRecieveArgs, _cdoc);
			var createAction = new GeometryCreateAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryCreateArgs))]
		public void GeometryCreateAction_TryConvert(object sender, RhinoObjectEventArgs createRecieveArgs)
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
		public void GeometryRemoveAction_CanConvert(object sender, RhinoObjectEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();
			Assert.That(createAction.CanConvert(sender, createArgs), Is.True);
		}

		[TestCaseSource(nameof(GeometryRemoveArgs))]
		public void GeometryRemoveAction_TryConvert(object sender, RhinoObjectEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Remove, createRecieveArgs, _cdoc);
			var createAction = new GeometryRemoveAction();
			Assert.That(createAction.TryConvert(sender, createArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(createArgs.Action));
				Assert.That(change is GeometryChange, Is.True);
			}
		}


		public GeometryCreateRemoveActionTests()
		{
			//  Use the existing open docs
			_doc = RhinoDoc.CreateHeadless(null);
			_cdoc = new CrashDoc();

			RhinoDoc.AddRhinoObject += RhinoDoc_AddRhinoObject;
			RhinoDoc.DeleteRhinoObject += RhinoDoc_DeleteRhinoObject;
			for (int i = 0; i < 100; i++)
			{
				LineCurve lineCurve = new LineCurve(RandomPoint(), RandomPoint());
				_doc.Objects.Add(lineCurve);
			}

			RhinoDoc.AddRhinoObject -= RhinoDoc_AddRhinoObject;
			RhinoDoc.DeleteRhinoObject -= RhinoDoc_DeleteRhinoObject;
		}


		private void RhinoDoc_DeleteRhinoObject(object sender, RhinoObjectEventArgs e)
		{
			RemoveEventArgs.Add((sender, e));
		}

		private void RhinoDoc_AddRhinoObject(object sender, RhinoObjectEventArgs e)
		{
			CreateEventArgs.Add((sender, e));
		}

		private static List<(object, RhinoObjectEventArgs)> CreateEventArgs = new List<(object, RhinoObjectEventArgs)>();
		public static IEnumerable GeometryCreateArgs => CreateEventArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static List<(object, RhinoObjectEventArgs)> RemoveEventArgs = new List<(object, RhinoObjectEventArgs)>();
		public static IEnumerable GeometryRemoveArgs => RemoveEventArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static Point3d RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new Point3d(x, y, z);
		}

	}

}
