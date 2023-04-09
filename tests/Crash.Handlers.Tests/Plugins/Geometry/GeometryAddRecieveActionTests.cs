using System.Collections;
using System.IO;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Handlers.Plugins.Geometry.Recieve;

using Rhino;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Plugins.Geometry
{

	[TestFixture]
	public sealed class GeometryAddRecieveActionTests
	{

		[TestCaseSource(nameof(AddChanges))]
		public void TestGeometryAddRecieveAction(Change change)
		{
			CrashDoc crashDoc = new CrashDoc();
			RhinoDoc rhinoDoc = RhinoDoc.CreateHeadless(null);

			var addAction = new GeometryAddRecieveAction();
			addAction.OnRecieve(crashDoc, change);
			while (crashDoc.Queue.Count > 0)
			{
				crashDoc.Queue.RunNextAction();
			}

			// ChangeUtils.TryGetChangeId() ?

			// Assert that RhinoDoc had something added 
			Assert.That(rhinoDoc.Objects, Is.Not.Empty);
			Assert.That(crashDoc.CacheTable, Is.Empty);
		}

		public static IEnumerable AddChanges
		{
			get
			{
				for (int i = 0; i < 100; i++)
				{
					string owner = Path.GetRandomFileName().Replace(".", "");
					LineCurve lineCurve = new LineCurve(RandomPoint(), RandomPoint());
					IChange change = GeometryChange.CreateNew(owner, lineCurve);

					yield return new Change(change);
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
