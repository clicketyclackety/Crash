namespace Crash.Changes.Tests
{

	[Flags]
	enum CustomChangeActionNegative
	{
		Rejoin = -4,

		Destroy = -2,

		Rebuild = -1,

		// Crash Defaults
		Unset = ChangeAction.Unset,
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

			Assert.IsTrue(cAction.HasFlag(ChangeAction.Add));
			Assert.IsTrue(cAction.HasFlag(ChangeAction.Remove));
			Assert.IsTrue(cAction.HasFlag(ChangeAction.Update));
			Assert.IsTrue(cAction.HasFlag((ChangeAction)(-1)));
			Assert.IsTrue(cAction.HasFlag((ChangeAction)(-2)));
			Assert.IsTrue(cAction.HasFlag((ChangeAction)(-4)));
		}

		[Test]
		public void TestDeclaredNegative()
		{
			CustomChangeActionNegative cAction = CustomChangeActionNegative.Add | CustomChangeActionNegative.Remove | CustomChangeActionNegative.Update |
								   CustomChangeActionNegative.Rebuild | CustomChangeActionNegative.Destroy | CustomChangeActionNegative.Rejoin;

			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Add));
			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Remove));
			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Update));
			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Rebuild));
			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Destroy));
			Assert.IsTrue(cAction.HasFlag(CustomChangeActionNegative.Rejoin));
		}

	}
}
