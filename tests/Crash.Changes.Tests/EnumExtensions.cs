namespace Crash.Changes.Tests
{

	[Flags]
	enum CustomChangeActionNegative
	{
		Rejoin = -4,

		Destroy = -2,

		Rebuild = -1,

		// Crash Defaults
		Unset = ChangeAction.None,
		Add = ChangeAction.Add,
		Remove = ChangeAction.Remove,
		Update = ChangeAction.Update,
		Transform = ChangeAction.Transform,
		Lock = ChangeAction.Lock,
		Unlock = ChangeAction.Unlock,
		Temporary = ChangeAction.Temporary,
	}


	[TestFixture]
	public sealed class EnumExtensions
	{

		[Test]
		public void TestImplicitNegatives()
		{
			ChangeAction cAction = ChangeAction.Add | ChangeAction.Remove | ChangeAction.Update |
								   (ChangeAction)(-1) | (ChangeAction)(-2) | (ChangeAction)(-4);

			Assert.That(cAction.HasFlag(ChangeAction.Add), Is.True);
			Assert.That(cAction.HasFlag(ChangeAction.Remove), Is.True);
			Assert.That(cAction.HasFlag(ChangeAction.Update), Is.True);
			Assert.That(cAction.HasFlag((ChangeAction)(-1)), Is.True);
			Assert.That(cAction.HasFlag((ChangeAction)(-2)), Is.True);
			Assert.That(cAction.HasFlag((ChangeAction)(-4)), Is.True);
		}

		[Test]
		public void TestDeclaredNegative()
		{
			CustomChangeActionNegative cAction = CustomChangeActionNegative.Add | CustomChangeActionNegative.Remove | CustomChangeActionNegative.Update |
								   CustomChangeActionNegative.Rebuild | CustomChangeActionNegative.Destroy | CustomChangeActionNegative.Rejoin;

			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Add), Is.True);
			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Remove), Is.True);
			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Update), Is.True);
			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Rebuild), Is.True);
			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Destroy), Is.True);
			Assert.That(cAction.HasFlag(CustomChangeActionNegative.Rejoin), Is.True);
		}

	}
}
