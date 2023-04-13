using Crash.Changes;
using Crash.Utils;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Crash.Handlers.Tests.Changes
{

	[TestFixture]
	public sealed class ChangeUtils_Tests
	{

		/// <summary>Test that nothing throws</summary>
		[Test]
		public void TryGetChangeId_NullInput()
		{
			Assert.That(ChangeUtils.TryGetChangeId(null, out Guid id), Is.False);
			Assert.That(id, Is.EqualTo(Guid.Empty));
		}

		/// <summary>Test that nothing throws</summary>
		[Test]
		public void TryGetChangeId_EmptyDictionary()
		{
			RhinoDoc doc = RhinoDoc.CreateHeadless(null);
			LineCurve lineCurve = new LineCurve(Point3d.Origin, new Point3d(100, 0, 0));
			Guid rhinoId = doc.Objects.Add(lineCurve);
			RhinoObject rhinoObject = doc.Objects.FindId(rhinoId);

			Assert.That(ChangeUtils.TryGetChangeId(rhinoObject, out Guid id), Is.False);
			Assert.That(id, Is.EqualTo(Guid.Empty));
		}

		/// <summary>Test that nothing throws</summary>
		[Test]
		public void SyncHost_NullInputs()
		{
			ChangeUtils.SyncHost(null, null);

			ChangeUtils.SyncHost(null, new ExampleRhinoChange());


			RhinoDoc doc = RhinoDoc.CreateHeadless(null);
			LineCurve lineCurve = new LineCurve(Point3d.Origin, new Point3d(100, 0, 0));
			Guid rhinoId = doc.Objects.Add(lineCurve);
			RhinoObject rhinoObject = doc.Objects.FindId(rhinoId);
			ChangeUtils.SyncHost(rhinoObject, null);
		}

		private sealed class ExampleRhinoChange : IChange
		{
			public DateTime Stamp => DateTime.Now;

			public Guid Id => Guid.NewGuid();

			public string Owner => nameof(ExampleRhinoChange);

			public string Payload => "";

			public string Type => nameof(ExampleRhinoChange);

			public ChangeAction Action { get; set; } = ChangeAction.Add;
		}

	}

}
