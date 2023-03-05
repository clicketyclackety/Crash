using System;

namespace Crash.Handlers.Changes
{

	public interface IRhinoChange : IChange
	{
		public Guid RhinoId { get; set; }
	}

}
