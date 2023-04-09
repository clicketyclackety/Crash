using System.Text.Json;

using Crash.Handlers.Changes;

using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Runtime;

namespace Crash.Common.Changes
{

	/// <summary>
	/// Local instance of a received Change.
	/// </summary>
	public sealed class GeometryChange : IRhinoChange
	{
		public const string ChangeType = $"{nameof(Crash)}.{nameof(GeometryChange)}";

		public GeometryBase Geometry { get; private set; }

		public Guid RhinoId { get; set; }

		public DateTime Stamp { get; private set; }

		public Guid Id { get; internal set; }

		public string? Owner { get; private set; }

		public string? Payload { get; private set; }

		public string Type => ChangeType;

		public ChangeAction Action { get; set; }

		public GeometryChange() { }

		public GeometryChange(IChange change) : this()
		{
			var options = new SerializationOptions();
			GeometryBase? geometry = CommonObject.FromJSON(change.Payload) as GeometryBase;
			if (null == geometry)
			{
				throw new JsonException("Could not deserialize Geometry");
			}

			Geometry = geometry;
			Stamp = change.Stamp;
			Id = change.Id;
			Owner = change.Owner;
			Payload = change.Payload;
		}

		public static GeometryChange CreateNew(string owner, GeometryBase geometry)
		{
			var options = new SerializationOptions();
			string? payload = geometry?.ToJSON(options);

			var instance = new GeometryChange()
			{
				Geometry = geometry,
				Stamp = DateTime.UtcNow,
				Id = Guid.NewGuid(),
				Owner = owner,
				Payload = payload,
			};
			instance.Action = ChangeAction.Add;

			return instance;
		}

	}
}
