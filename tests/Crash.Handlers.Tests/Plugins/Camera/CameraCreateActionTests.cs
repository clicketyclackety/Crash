using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins;
using Crash.Handlers.Plugins.Camera.Create;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class CameraCreateActionTests
	{
		private readonly RhinoDoc _doc;
		private readonly CrashDoc _cdoc;

		[TestCaseSource(nameof(ViewArgs))]
		public void GeometryCreateAction_CanConvert(object sender, ViewEventArgs createRecieveArgs)
		{
			var cameraArgs = new CreateRecieveArgs(ChangeAction.Camera, createRecieveArgs, _cdoc);
			var createAction = new CameraCreateAction();
			Assert.That(createAction.CanConvert(sender, cameraArgs), Is.True);
		}

		[TestCaseSource(nameof(ViewArgs))]
		public void GeometryCreateAction_TryConvert(object sender, ViewEventArgs createRecieveArgs)
		{
			var createArgs = new CreateRecieveArgs(ChangeAction.Add, createRecieveArgs, _cdoc);
			var createAction = new CameraCreateAction();
			Assert.That(createAction.TryConvert(sender, createArgs, out IEnumerable<IChange> changes), Is.True);
			Assert.That(changes, Is.Not.Empty);
			foreach (var change in changes)
			{
				Assert.That(change.Action, Is.EqualTo(ChangeAction.Camera));
				Assert.That(change is CameraChange, Is.True);
			}
		}


		public CameraCreateActionTests()
		{
			//  Use the existing open docs
			_doc = RhinoDoc.CreateHeadless(null);
			_cdoc = new CrashDoc();

			RhinoView.Modified += RhinoView_Modified;

			for (int i = 0; i < 100; i++)
			{
				RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.SetCameraLocation(RandomPoint(), true);
			}

			RhinoView.Modified -= RhinoView_Modified;
		}

		private void RhinoView_Modified(object sender, ViewEventArgs e)
		{
			ViewEventArgs.Add((sender, e));
		}

		private static List<(object, ViewEventArgs)> ViewEventArgs = new List<(object, ViewEventArgs)>();
		public static IEnumerable ViewArgs => ViewEventArgs.Select(ea => new TestCaseData(ea.Item1, ea.Item2));

		private static Point3d RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new Point3d(x, y, z);
		}

	}

}
