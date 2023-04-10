using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Crash.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins;
using Crash.Handlers.Plugins.Geometry.Create;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class GeometrySelectUnselectActionTests
	{
		private readonly RhinoDoc _doc;
		private readonly CrashDoc _cdoc;

		[TestCaseSource(nameof(SelectArgs))]
		public void GeometrySelectAction_CanConvert(object sender, RhinoObjectSelectionEventArgs selectEventArgs)
		{
			var selectArgs = new CreateRecieveArgs(ChangeAction.Lock, selectEventArgs, _cdoc);
			var createAction = new GeometrySelectAction();
			Assert.That(createAction.CanConvert(sender, selectArgs), Is.True);
		}

		[TestCaseSource(nameof(SelectArgs))]
		public void GeometrySelectAction_TryConvert(object sender, RhinoObjectSelectionEventArgs selectEventArgs)
		{
			var selectArgs = new CreateRecieveArgs(ChangeAction.Lock, selectEventArgs, _cdoc);
			var createAction = new GeometrySelectAction();
			Assert.That(createAction.TryConvert(sender, selectArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(selectArgs.Action));
			}
		}

		[TestCaseSource(nameof(UnSelectArgs))]
		public void GeometryUnSelectAction_CanConvert(object sender, RhinoObjectSelectionEventArgs selectEventArgs)
		{
			var selectArgs = new CreateRecieveArgs(ChangeAction.Unlock, selectEventArgs, _cdoc);
			var createAction = new GeometryUnSelectAction();
			Assert.That(createAction.CanConvert(sender, selectArgs), Is.True);
		}

		[TestCaseSource(nameof(UnSelectArgs))]
		public void GeometryUnSelectAction_TryConvert(object sender, RhinoObjectSelectionEventArgs selectEventArgs)
		{
			var selectArgs = new CreateRecieveArgs(ChangeAction.Unlock, selectEventArgs, _cdoc);
			var createAction = new GeometryUnSelectAction();
			Assert.That(createAction.TryConvert(sender, selectArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(selectArgs.Action));
			}
		}


		public GeometrySelectUnselectActionTests()
		{
			//  Use the existing open docs
			_doc = RhinoDoc.CreateHeadless(null);
			_cdoc = new CrashDoc();

			RhinoDoc.SelectObjects += RhinoDoc_SelectObjects;
			RhinoDoc.DeselectObjects += RhinoDoc_DeselectObjects;
			// RhinoDoc.DeselectAllObjects

			for (int i = 0; i < 100; i++)
			{
				LineCurve lineCurve = new LineCurve(RandomPoint(), RandomPoint());
				Guid rhinoId = _doc.Objects.Add(lineCurve);
				_doc.Objects.Select(rhinoId, true, true);
			}

			RhinoDoc.SelectObjects -= RhinoDoc_SelectObjects;
			RhinoDoc.DeselectObjects -= RhinoDoc_DeselectObjects;
		}


		private void RhinoDoc_DeselectObjects(object sender, RhinoObjectSelectionEventArgs e)
		{
			_unSelectArgs.Add((sender, e));
		}

		private void RhinoDoc_SelectObjects(object sender, RhinoObjectSelectionEventArgs e)
		{
			_selectArgs.Add((sender, e));
		}

		private static List<(object, RhinoObjectSelectionEventArgs)> _selectArgs = new List<(object, RhinoObjectSelectionEventArgs)>();
		public static IEnumerable SelectArgs => _selectArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static List<(object, RhinoObjectSelectionEventArgs)> _unSelectArgs = new List<(object, RhinoObjectSelectionEventArgs)>();
		public static IEnumerable UnSelectArgs => _unSelectArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static Point3d RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new Point3d(x, y, z);
		}

	}

}
