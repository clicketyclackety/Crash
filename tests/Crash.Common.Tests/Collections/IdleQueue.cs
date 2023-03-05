using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Events;

namespace Crash.Common.Tests
{

	public sealed class IdleQueueTests
	{
		[Test]
		public void Count_ReturnsZero_WhenQueueIsEmpty()
		{
			// Arrange
			var queue = new IdleQueue(new CrashDoc());

			// Act
			var count = queue.Count;

			// Assert
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void Count_ReturnsCorrectCount_AfterEnqueueingItems()
		{
			int expectedCount = 3;

			// Arrange
			var queue = new IdleQueue(new CrashDoc());
			for (int i = 0; i < expectedCount; i++)
			{
				queue.AddAction(new IdleAction(null, null));
			}

			// Act
			var realCount = queue.Count;

			// Assert
			Assert.That(expectedCount, Is.EqualTo(realCount));
		}

		[Test]
		public void RunNextAction_DoesNothing_WhenQueueIsEmpty()
		{
			// Arrange
			var queue = new IdleQueue(new CrashDoc());

			// Act
			queue.RunNextAction();

			// Assert
			Assert.That(queue.Count, Is.EqualTo(0));
		}

		[Test]
		public void RunNextAction_InvokesAction_WhenQueueIsNotEmpty()
		{
			// Arrange
			var queue = new IdleQueue(new CrashDoc());
			IdleAction action = new IdleAction(DisposableCrashEvent, null);
			queue.AddAction(action);

			// Act
			queue.RunNextAction();

			// Assert
			Assert.IsTrue(action.Invoked);
		}

		[Test]
		public void RunNextAction_RemovesActionFromQueue_AfterInvoking()
		{
			int expectedCount = 3;

			// Arrange
			var queue = new IdleQueue(new CrashDoc());
			for (int i = 0; i < expectedCount; i++)
			{
				queue.AddAction(new IdleAction(DisposableCrashEvent, null));
			}

			// Act
			var realCount = queue.Count;

			// Act
			queue.RunNextAction();

			// Assert
			Assert.That(queue.Count, Is.EqualTo(expectedCount - 1));
		}

		[Test]
		public void RunNextAction_NoInvokeOnCompletedQueueEvent_WhenQueueIsEmpty()
		{
			// Arrange
			var queue = new IdleQueue(new CrashDoc());
			bool eventRaised = false;
			queue.OnCompletedQueue += (sender, args) => { eventRaised = true; };

			// Act
			queue.RunNextAction();

			// Assert
			Assert.IsFalse(eventRaised);
		}

		[Test]
		public void RunNextAction_InvokeOnCompletedQueueEvent()
		{
			// Arrange
			var queue = new IdleQueue(new CrashDoc());
			queue.AddAction(new IdleAction(DisposableCrashEvent, null));

			bool eventRaised = false;
			queue.OnCompletedQueue += (sender, args) => { eventRaised = true; };

			// Act
			queue.RunNextAction();

			// Assert
			Assert.IsTrue(eventRaised);
		}


		private void DisposableCrashEvent(CrashEventArgs args) { }
	}

}
