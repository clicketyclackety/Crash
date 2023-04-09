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
	public sealed class GeometryTransformtActionTests
	{
		private readonly RhinoDoc _doc;
		private readonly CrashDoc _cdoc;

		[TestCaseSource(nameof(TransformArgs))]
		public void GeometrySelectAction_CanConvert(object sender, RhinoTransformObjectsEventArgs selectEventArgs)
		{
			var transformArgs = new CreateRecieveArgs(ChangeAction.Transform, selectEventArgs, _cdoc);
			var createAction = new GeometryTransformAction();
			Assert.That(createAction.CanConvert(sender, transformArgs), Is.True);
		}

		[TestCaseSource(nameof(TransformArgs))]
		public void GeometrySelectAction_TryConvert(object sender, RhinoTransformObjectsEventArgs selectEventArgs)
		{
			var transformArgs = new CreateRecieveArgs(ChangeAction.Transform, selectEventArgs, _cdoc);
			var createAction = new GeometryTransformAction();
			Assert.That(createAction.TryConvert(sender, transformArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(transformArgs.Action));
				Assert.That(change is TransformChange, Is.True);
			}
		}

		public GeometryTransformtActionTests()
		{
			//  Use the existing open docs
			_doc = RhinoDoc.CreateHeadless(null);
			_cdoc = new CrashDoc();

			RhinoDoc.BeforeTransformObjects += RhinoDoc_BeforeTransformObjects;
			// RhinoDoc.DeselectAllObjects

			for (int i = 0; i < 100; i++)
			{
				LineCurve lineCurve = new LineCurve(RandomPoint(), RandomPoint());
				Guid rhinoId = _doc.Objects.Add(lineCurve);
				_doc.Objects.Select(rhinoId);
				Assert.That(RhinoApp.RunScript("-Move 0,0,0 100,200,300", true), Is.True);
			}

			RhinoDoc.BeforeTransformObjects -= RhinoDoc_BeforeTransformObjects;
		}

		private void RhinoDoc_BeforeTransformObjects(object sender, RhinoTransformObjectsEventArgs e)
		{
			_transformArgs.Add((sender, e));
		}

		private static List<(object, RhinoTransformObjectsEventArgs)> _transformArgs = new List<(object, RhinoTransformObjectsEventArgs)>();
		public static IEnumerable TransformArgs => _transformArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static Point3d RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new Point3d(x, y, z);
		}

	}

}
