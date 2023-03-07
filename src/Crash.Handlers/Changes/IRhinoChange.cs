using System;

namespace Crash.Handlers.Changes
{
	/// <summary>
	/// Interface for rhino object changes
	/// </summary>
	public interface IRhinoChange : IChange
	{
		/// <summary>
		/// The id of the referenced rhino object
		/// </summary>
		public Guid RhinoId { get; set; }
	}

}
