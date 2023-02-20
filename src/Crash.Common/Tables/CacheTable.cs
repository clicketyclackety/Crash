using System.Collections;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;

namespace Crash.Common.Tables
{

	public sealed class ChangeTable : IEnumerable<ICachedChange>
	{
		private CrashDoc _crashDoc;

		// TODO : Should this be async? Or Concurrent?
		private ConcurrentDictionary<Guid, ICachedChange> _cache { get; set; }


		public bool IsInit = false;
		public bool SomeoneIsDone = false;

		/// <summary>
		/// Local cache constructor subscribing to RhinoApp_Idle
		/// </summary>
		public ChangeTable(CrashDoc hostDoc)
		{
			_cache = new ConcurrentDictionary<Guid, ICachedChange>();
			_crashDoc = hostDoc;
		}

		internal void Clear()
		{
			_cache?.Clear();
		}

		#region ConcurrentDictionary Methods

		/// <summary>
		/// Method to update a Change
		/// </summary>
		/// <param name="cache">the Changes</param>
		/// <returns>returns the update task</returns>
		public async Task UpdateChange(ICachedChange cache)
		{
			if (cache == null) return;

			if (_cache.ContainsKey(cache.Id))
			{
				_cache.TryRemove(cache.Id, out _);
			}

			_cache.TryAdd(cache.Id, cache);
			if (string.IsNullOrEmpty(cache.Owner))
				return;
		}

		/// <summary>
		/// Remove a Change for the cache
		/// </summary>
		/// <param name="Change">the Change to remove</param>
		internal void RemoveChange(Guid ChangeId)
		{
			_cache.TryRemove(ChangeId, out _);
		}

		/// <summary>
		/// Remove multiple Changes from the cache
		/// </summary>
		/// <param name="Changes">the Changes to remove</param>
		internal void RemoveChanges(IEnumerable<global::IChange> Changes)
		{
			if (null == Changes) return;

			var enumer = Changes.GetEnumerator();
			while (enumer.MoveNext())
			{
				RemoveChange(enumer.Current.Id);
			}
		}

		#endregion

		public void AddToDocument(ICachedChange cachedChange)
		{
			if (cachedChange.AddToDocument is not object) return;

			var args = new CrashEventArgs(_crashDoc);
			cachedChange.AddToDocument.Invoke(args);
		}

		public void RemoveFromDocument(ICachedChange cachedChange)
		{
			if (cachedChange.AddToDocument is not object) return;

			var args = new CrashEventArgs(_crashDoc);
			cachedChange.RemoveFromDocument.Invoke(args);
		}


		/// <summary>
		/// Method to get Changes
		/// </summary>
		/// <returns>returns a list of the Changes</returns>
		public IEnumerable<ICachedChange> GetChanges() => _cache.Values;

		public IEnumerator<ICachedChange> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

}
