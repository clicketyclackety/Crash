using Crash.Handlers.Plugins.Initializers.Recieve;

using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins.Initializers
{

	/// <summary>Handles Done calls inside of Crash</summary>
	public sealed class DoneDefinition : IChangeDefinition
	{
		/// <inheritdoc/>
		public Type ChangeType => typeof(Change);

		/// <inheritdoc/>
		public string ChangeName => $"{nameof(Crash)}.Done";

		/// <inheritdoc/>
		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		/// <inheritdoc/>
		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }


		/// <summary>Default Constructor</summary>
		public DoneDefinition()
		{
			CreateActions = Array.Empty<IChangeCreateAction>();
			RecieveActions = new List<IChangeRecieveAction> { new DoneRecieve() };
		}


		/// <inheritdoc/>
		public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public BoundingBox GetBoundingBox(IChange change)
		{
			throw new NotImplementedException();
		}

	}

}
