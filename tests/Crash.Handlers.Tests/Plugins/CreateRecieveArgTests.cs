using Crash.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Handlers.Plugins;

using Rhino;

namespace Crash.Handlers.Tests.Plugins
{

	[TestFixture]
	public sealed class CreateRecieveArgTests
	{

		[Test]
		public void ValidityTests()
		{
			var doc = new CrashDoc();
			var eargs = new CrashEventArgs(doc);
			var args = new CreateRecieveArgs(ChangeAction.Add, eargs, doc);
		}

		[Test]
		public void Creation_BadInputs()
		{
			Assert.Throws<ArgumentNullException>(() => new CreateRecieveArgs((ChangeAction)(-1), null, (CrashDoc)null));
			Assert.Throws<ArgumentNullException>(() => new CreateRecieveArgs((ChangeAction)(-1), null, (RhinoDoc)null));
		}

	}

}
