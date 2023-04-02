using Rhino.Display;
using Rhino.Geometry;

namespace Crash.Handlers.Plugins
{

	/// <summary>Describes a Change</summary>
	public interface IChangeDefinition
	{

		/// <summary>The Type of </summary>
		Type ChangeType { get; }

		/// <summary></summary>
		string ChangeName { get; }

		/// <summary></summary>
		void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change);

		/// <summary></summary>
		BoundingBox GetBoundingBox(IChange change);

		// These will be registered somewhere and Crash will perform a fall through to find the first conversion candidate
		// They will be index by Action too.
		/// <summary></summary>
		IEnumerable<IChangeCreateAction> CreateActions { get; }

		// These will be registered somewhere, and when Crash recieves a Change, and then perform the conversion
		// It will then be indexed by name
		/// <summary></summary>
		IEnumerable<IChangeRecieveAction> RecieveActions { get; }

	}

}
