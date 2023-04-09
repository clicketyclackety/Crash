using Crash.Common.Collections;

namespace Crash.Tests.Collections
{

	[TestFixture]
	public sealed class FixedSizedQueueTests
	{

		[Test]
		public void Enqueue_WhenQueueIsNotFull_AddsItemToQueue()
		{
			// Arrange
			var queue = new FixedSizedQueue<int>(3);

			// Act
			queue.Enqueue(1);
			queue.Enqueue(2);

			// Assert
			Assert.That(queue.Count, Is.EqualTo(2));
			CollectionAssert.AreEqual(new[] { 1, 2 }, queue);
		}

		[Test]
		public void Enqueue_WhenQueueIsFull_DiscardsOldestItem()
		{
			// Arrange
			var queue = new FixedSizedQueue<int>(3);

			// Act
			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);
			queue.Enqueue(4);

			// Assert
			Assert.That(queue.Count, Is.EqualTo(3));
			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, queue);
		}

	}

}
