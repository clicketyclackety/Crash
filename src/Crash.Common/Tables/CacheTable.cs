using System.Collections;

using Crash.Common.Document;

namespace Crash.Common.Tables
{

	/// <summary>Holds Temporary Changes only</summary>
	public sealed class ChangeTable : IEnumerable<IChange>
	{
		private readonly CrashDoc _crashDoc;

		// TODO : Should this be async? Or Concurrent?
		private readonly ConcurrentDictionary<Guid, IChange> _cache;

		// TODO : Move
		public bool IsInit { get; set; } = false;
		// TODO : Move
		public bool SomeoneIsDone { get; set; } = false;

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
				await Task.Run(() => _cache.TryRemove(cache.Id, out _));
			}

			await Task.Run(() => _cache.TryAdd(cache.Id, cache));
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
			change = default;

			if (_cache.TryGetValue(id, out IChange? cachedChange) &&
				cachedChange is T changeConverted)
			{
				if (cachedChange == default) return false;

				change = changeConverted;
				return true;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Method to get Changes
		/// </summary>
		/// <returns>returns a list of the Changes</returns>
		public IEnumerable<IChange> GetChanges() => _cache.Values;

		/// <inheritdoc/>
		public IEnumerator<IChange> GetEnumerator() => _cache.Values.GetEnumerator();
		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator<T>() => _cache.Values.Where(x => x is T).Cast<T>().GetEnumerator();
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => _cache.Values.GetEnumerator();

	}
}

