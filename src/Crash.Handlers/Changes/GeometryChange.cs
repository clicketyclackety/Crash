using System.Text.Json;

using Crash.Common.Events;
using Crash.Handlers.Changes;

using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Runtime;

namespace Crash.Common.Changes
{

	/// <summary>
	/// Local instance of a received Change.
	/// </summary>
	public sealed class GeometryChange : ICachedChange, IRhinoChange
	{
		IChange Change { get; set; }

		public GeometryBase Geometry { get; private set; }

		public Guid RhinoId { get; set; }

		public DateTime Stamp => Change.Stamp;

		public Guid Id => Change.Id;

		public string? Owner => Change.Owner;

		public string? Payload => Change.Payload;

		public int Action { get; set; }


		public GeometryChange()
		{

		}

		public GeometryChange(IChange cange)
		{
			Change = cange;
			var options = new SerializationOptions();
			GeometryBase? geometry = CommonObject.FromJSON(Change.Payload) as GeometryBase;
			if (null == geometry)
			{
				throw new JsonException("Could not deserialize Geometry");
			}

			Geometry = geometry;
		}

		public static GeometryChange CreateNew(string owner, GeometryBase geometry)
		{
			var options = new SerializationOptions();
			string? payload = geometry?.ToJSON(options);

			var Change = new Change(Guid.NewGuid(), owner, payload);
			var instance = new GeometryChange(Change) { Geometry = geometry };
			instance.Action = (int)(ChangeAction.Add | ChangeAction.Temporary);

			return instance;
		}


		public Action<CrashEventArgs> Draw { get; set; }

		public Action<CrashEventArgs> AddToDocument { get; set; }

		public Action<CrashEventArgs> RemoveFromDocument { get; set; }


	}
}
