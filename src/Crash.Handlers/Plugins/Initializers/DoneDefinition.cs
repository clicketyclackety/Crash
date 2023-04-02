using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Initializers
{

	public sealed class DoneDefinition : IChangeDefinition
	{
		public Type ChangeType => typeof(Change);

		public string ChangeName => $"{nameof(Crash)}.Done";

		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }


		public DoneDefinition()
		{
			CreateActions = Array.Empty<IChangeCreateAction>();
			RecieveActions = new List<IChangeRecieveAction> { new DoneRecieve() };
		}


		public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
		{
			throw new NotImplementedException();
		}

		public BoundingBox GetBoundingBox(IChange change)
		{
			throw new NotImplementedException();
		}

	}

}
