namespace Crash.Common.Tables.Tests
{

	/*
	 * 
	[TestFixture]
	public class ChangeTableTests
	{
		private ChangeTable _changeTable;

		[SetUp]
		public void SetUp()
		{
			var doc = new CrashDoc();
			_changeTable = new ChangeTable(doc);
		}

		[Test]
		public void UpdateChangeAsync_UpdatesCache_WhenCacheContainsChange()
		{
			// Arrange
			var id = Guid.NewGuid();
			var initialChange = new TextChange(id, "Hello", "World");
			_changeTable.UpdateChangeAsync(initialChange).Wait();

			var updatedChange = new TextChange(id, "Hello", "Everyone");

			// Act
			_changeTable.UpdateChangeAsync(updatedChange).Wait();
			var result = _changeTable.TryGetValue<TextChange>(id, out var retrievedChange);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual("Everyone", retrievedChange.NewValue);
		}

		[Test]
		public void UpdateChangeAsync_AddsChange_WhenCacheDoesNotContainChange()
		{
			// Arrange
			var id = Guid.NewGuid();
			var change = new TextChange(id, "Hello", "World");

			// Act
			_changeTable.UpdateChangeAsync(change).Wait();
			var result = _changeTable.TryGetValue<TextChange>(id, out var retrievedChange);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual("World", retrievedChange.NewValue);
		}

		[Test]
		public void RemoveChange_RemovesChangeFromCache()
		{
			// Arrange
			var id = Guid.NewGuid();
			var change = new TextChange(id, "Hello", "World");
			_changeTable.UpdateChangeAsync(change).Wait();

			// Act
			_changeTable.RemoveChange(id);
			var result = _changeTable.TryGetValue<TextChange>(id, out var retrievedChange);

			// Assert
			Assert.IsFalse(result);
			Assert.IsNull(retrievedChange);
		}

		[Test]
		public void RemoveChanges_RemovesChangesFromCache()
		{
			// Arrange
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var change1 = new TextChange(id1, "Hello", "World");
			var change2 = new TextChange(id2, "Goodbye", "World");
			_changeTable.UpdateChangeAsync(change1).Wait();
			_changeTable.UpdateChangeAsync(change2).Wait();

			// Act
			_changeTable.RemoveChanges(new[] { change1, change2 });
			var result1 = _changeTable.TryGetValue<TextChange>(id1, out var retrievedChange1);
			var result2 = _changeTable.TryGetValue<TextChange>(id2, out var retrievedChange2);

			// Assert
			Assert.IsFalse(result1);
			Assert.IsNull(retrievedChange1);
			Assert.IsFalse(result2);
			Assert.IsNull(retrievedChange2);
		}

		[Test]
		public void AddToDocument_InvokesEventHandler()
		{
			// Arrange
			var id = Guid.NewGuid();
			var change = new TextChange(id, "Hello", "World");
			var eventHandlerInvoked = false;

			change.AddToDocument = (args) => eventHandlerInvoked = true;

			// Act
			_changeTable.AddToDocument(change);

			// Assert
			Assert.IsTrue(eventHandlerInvoked);
		}

		[Test]
		public void RemoveFromDocument_InvokesEventHandler()
		{
			// Arrange
			var id = Guid.NewGuid();
			var change = new TextChange(id, "Hello", "World");
		}
	}

	*/

}
