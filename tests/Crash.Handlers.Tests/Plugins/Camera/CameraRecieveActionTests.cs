using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Crash.Changes;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Geometry;
using Crash.Handlers.Plugins.Camera.Recieve;

namespace Crash.Handlers.Tests.Plugins.Camera
{

	[TestFixture]
	public sealed class CameraRecieveActionTests
	{

		[TestCaseSource(nameof(CameraChanges))]
		public async Task GeometryCreateAction_CanConvert(Crash.Common.View.Camera camera)
		{
			string username = Path.GetRandomFileName().Replace(".", "");
			IChange change = CameraChange.CreateNew(camera, username);
			Change serverChange = new Change(change);

			CrashDoc crashDoc = new CrashDoc();

			CameraRecieveAction recieveAction = new CameraRecieveAction();

			Assert.That(crashDoc.Cameras, Is.Empty);
			await recieveAction.OnRecieveAsync(crashDoc, serverChange);
			Assert.That(crashDoc.Cameras, Is.Not.Empty);

			Assert.That(crashDoc.Cameras.TryGetCamera(new User(username), out var cameras), Is.True);
			Assert.That(cameras, Has.Count.EqualTo(1));
			Assert.That(cameras.FirstOrDefault(), Is.EqualTo(camera));
		}

		public static IEnumerable CameraChanges
		{
			get
			{
				for (int i = 0; i < 100; i++)
				{
					yield return new Crash.Common.View.Camera(RandomPoint(), RandomPoint());
				}
			}
		}

		private static CPoint RandomPoint()
		{
			double x = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double y = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			double z = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);

			return new CPoint(x, y, z);
		}

	}

}
