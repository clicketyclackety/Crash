using System.Text.Json;

using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Runtime;

namespace Crash.Common.Changes
{

	/// <summary>A Change encapuslating Rhino Geometry</summary>
	public sealed class GeometryChange : IChange
	{
		/// <summary>The TYpe of this Change</summary>
		public const string ChangeType = $"{nameof(Crash)}.{nameof(GeometryChange)}";

		/// <summary>The Related Rhino Geometry</summary>
		public GeometryBase Geometry { get; private set; }

		/// <inheritdoc/>
		public DateTime Stamp { get; private set; }

		/// <inheritdoc/>
		public Guid Id { get; internal set; }

		/// <inheritdoc/>
		public string? Owner { get; private set; }

		/// <inheritdoc/>
		public string? Payload { get; private set; }

		/// <inheritdoc/>
		public string Type => ChangeType;
		/// <inheritdoc/>
		public ChangeAction Action { get; set; }

		/// <summary>Deserialization Constructor</summary>
		public GeometryChange() { }

		/// <summary>Inheritance Constructor</summary>
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
