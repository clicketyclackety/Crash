using System.Collections;

using Crash.Common.Document;

namespace Crash.Common.Tables
{

	public sealed class ChangeTable : IEnumerable<IChange>
	{
		private readonly CrashDoc _crashDoc;

		// TODO : Should this be async? Or Concurrent?
		private ConcurrentDictionary<Guid, IChange> _cache { get; set; }

		public bool IsInit = false;
		public bool SomeoneIsDone = false;

		/// <summary>
		/// Local cache constructor subscribing to RhinoApp_Idle
		/// </summary>
		public ChangeTable(CrashDoc hostDoc)
		{
			_cache = new ConcurrentDictionary<Guid, IChange>();
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
		public async Task UpdateChangeAsync(IChange cache)
		{
			if (cache == null) return;

			if (_cache.ContainsKey(cache.Id))
			{
				_cache.TryRemove(cache.Id, out _);
			}

			_cache.TryAdd(cache.Id, cache);
		}

		/// <summary>
		/// Remove a Change for the cache
		/// </summary>
		/// <param name="Change">the Change to remove</param>
		public void RemoveChange(Guid ChangeId)
		{
			_cache.TryRemove(ChangeId, out _);
		}

		/// <summary>
		/// Remove multiple Changes from the cache
		/// </summary>
		/// <param name="Changes">the Changes to remove</param>
		public void RemoveChanges(IEnumerable<IChange> Changes)
		{
			if (null == Changes) return;

			var enumer = Changes.GetEnumerator();
			while (enumer.MoveNext())
			{
				RemoveChange(enumer.Current.Id);
			}
		}

		public bool TryGetValue<T>(Guid id, out T change) where T : IChange
		{
			if (_cache.TryGetValue(id, out IChange cachedChange) &&
				cachedChange is T changeConverted)
			{
				change = changeConverted;
				return true;
			}

			change = default;
			return false;
		}

		#endregion

		/// <summary>
		/// Method to get Changes
		/// </summary>
		/// <returns>returns a list of the Changes</returns>
		public IEnumerable<IChange> GetChanges() => _cache.Values;

		public IEnumerator<IChange> GetEnumerator() => _cache.Values.GetEnumerator();

		public IEnumerator<T> GetEnumerator<T>() => _cache.Values.Where(x => x is T).Cast<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _cache.Values.GetEnumerator();
	}
}

