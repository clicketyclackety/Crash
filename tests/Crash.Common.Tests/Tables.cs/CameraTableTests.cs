using System.Collections;

using Crash.Common.Changes;
using Crash.Common.Collections;
using Crash.Common.Document;
using Crash.Common.View;
using Crash.Geometry;

namespace Crash.Common.Tables.Tests
{

	[TestFixture]
	public class CameraTableTests
	{

		[TestCaseSource(typeof(CameraChanges), nameof(CameraChanges.TestCases))]
		public void TestAddCamera(CameraChange change)
		{
			// Arrange
			CrashDoc crashDoc = new CrashDoc();
			CameraTable cameraTable = new CameraTable(crashDoc);
			cameraTable.TryAddCamera(change);

			// Act
			var users = cameraTable.GetActiveCameras().Keys.ToArray();
			var cameras = cameraTable.GetActiveCameras().Values.ToArray();

			// Assert
			Assert.That(users.Length, Is.EqualTo(1));
			Assert.That(cameras.Length, Is.EqualTo(1));
		}

		[TestCaseSource(typeof(CameraChanges), nameof(CameraChanges.TestCases))]
		public void TestGetCamera(CameraChange change)
		{
			// Arrange
			CrashDoc crashDoc = new CrashDoc();
			CameraTable cameraTable = new CameraTable(crashDoc);
			cameraTable.TryAddCamera(change);

			// Act
			Assert.IsTrue(cameraTable.TryGetCamera(new User(change.Owner),
						out FixedSizedQueue<Camera> cameras));

			// Assert
			Assert.That(cameras.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public void TestAddMoreThanMaxCameras()
		{
			// Arrange
			int overMax = (CameraTable.MAX_CAMERAS_IN_QUEUE + 5);
			CameraTable cameraTable = new CameraTable(new CrashDoc());

			string userName = "Jeff";
			User user = new User(userName);

			// Act
			for (int i = 0; i < overMax; i++)
			{
				Camera camera = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
				var change = CameraChange.CreateNew(camera, userName);
				Assert.IsTrue(cameraTable.TryAddCamera(change));
			}

			Assert.IsNotEmpty(cameraTable);

			// Assert
			Assert.IsTrue(cameraTable.TryGetCamera(user, out FixedSizedQueue<Camera> cameras));
			Assert.That(cameras.Count, Is.EqualTo(CameraTable.MAX_CAMERAS_IN_QUEUE));
		}

		[Test]
		public void TestGetActiveCameras()
		{
			CameraTable cameraTable = new CameraTable(new CrashDoc());
			string userName = "Jeff";

			// Act
			for (int i = 0; i < 5; i++)
			{
				Camera camera = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
				var change = CameraChange.CreateNew(camera, userName);
				Assert.IsTrue(cameraTable.TryAddCamera(change));
			}

			// Assert
			Assert.IsNotEmpty(cameraTable);
			Assert.That(cameraTable.GetActiveCameras().Count, Is.EqualTo(1));
		}

		public sealed class CameraChanges
		{
			public static IEnumerable TestCases
			{
				get
				{
					Camera camera1 = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera1, "Jenny");

					Camera camera2 = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera2, "Jack");

					Camera camera3 = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera3, "Jeff");

					Camera camera4 = new Camera(CPoint.Origin, new CPoint(1, 2, 3));
					yield return CameraChange.CreateNew(camera4, "Jerry");
				}
			}
		}

	}

}
