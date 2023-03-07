using System.Text.Json;

using Crash.Common.Events;
using Crash.Handlers.Changes;

using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Runtime;

namespace Crash.Common.Changes
{

	/// <summary>
	/// Local instance of a received change of geometry
	/// </summary>
	public sealed class GeometryChange : ICachedChange, IRhinoChange
	{
		/// <inheritdoc />
		IChange Change { get; set; }

		/// <summary>
		/// The rhino geometry of the change
		/// </summary>
		public GeometryBase Geometry { get; private set; }

		/// <summary>
		/// The rhino object id
		/// </summary>
		public Guid RhinoId { get; set; }

		/// <summary>
		/// The date time stamp
		/// </summary>
		public DateTime Stamp => Change.Stamp;

		/// <summary>
		/// The unique ID od the change
		/// </summary>
		public Guid Id => Change.Id;

		/// <summary>
		/// The owner of the change
		/// </summary>
		public string? Owner => Change.Owner;

		/// <summary>
		/// The payload, the geometry that changed serialized as string
		/// </summary>
		public string? Payload => Change.Payload;

		/// <summary>
		/// The action describing what changed
		/// </summary>
		public int Action { get; set; }

		/// <summary>
		/// Empty constructor
		/// </summary>
		public GeometryChange()
		{
			Draw = PerformDraw;
			RemoveFromDocument = PerformRemoveFromDocument;
			AddToDocument = PerformAddToDocument;
		}

		/// <summary>
		/// Constructor of a geometry change from an IChange
		/// </summary>
		/// <param name="change">the IChange describing the change</param>
		/// <exception cref="JsonException">If the geometry could not be De-serialized</exception>
		public GeometryChange(IChange change) : this()
		{
			Change = change;
			Action = change.Action;
			var options = new SerializationOptions();
			GeometryBase? geometry = CommonObject.FromJSON(Change.Payload) as GeometryBase;
			if (null == geometry)
			{
				throw new JsonException("Could not deserialize Geometry");
			}

			Geometry = geometry;

			Draw = PerformDraw;
			RemoveFromDocument = PerformRemoveFromDocument;
			AddToDocument = PerformAddToDocument;
		}

		/// <summary>
		/// Method to generate a Geometry change from a GeometryBase object
		/// </summary>
		/// <param name="owner">the owner of the change</param>
		/// <param name="geometry">the geometry that changed</param>
		/// <returns></returns>
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

		private void PerformAddToDocument(CrashEventArgs e)
		{
			// TODO : Implement this
		}

		private void PerformRemoveFromDocument(CrashEventArgs e)
		{
			// TODO : Implement this
		}

		private void PerformDraw(CrashEventArgs e)
		{
			// TODO : Implement this
		}

	}
}
