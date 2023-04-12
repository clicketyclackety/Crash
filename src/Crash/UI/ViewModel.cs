using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Crash.Common.Document;
using Crash.Common.Tables;
using Crash.Handlers;
using Crash.Properties;

using Eto.Drawing;
using Eto.Forms;

using Rhino.UI;

namespace Crash.UI
{
	internal sealed class UsersViewModel : INotifyPropertyChanged
	{
		public sealed class UserObject
		{
			public string Name { get; private set; }
			public System.Drawing.Color Colour { get; private set; }
			public CameraState Camera { get; set; }
			public bool Visible { get; set; }

			internal User CUser => new User(Name)
			{
				Camera = Camera,
				Visible = Visible,
			};

			internal UserObject(User user)
			{
				Name = user.Name;
				Colour = user.Color;
				Camera = user.Camera;
				Visible = user.Visible;
			}
		}

		internal ObservableCollection<UserObject> Users { get; set; }

		internal readonly IndirectBinding<Image> ImageCellBinding;

		internal readonly IndirectBinding<bool?> VisibleCellBinding;

		internal readonly IndirectBinding<string> TextCellBinding;

		internal GridView View;

		internal UsersViewModel()
		{
			RhinoDoc.ActiveDocumentChanged += (sender, args) => UsersForm.ReDraw();
			RhinoDoc.ActiveDocumentChanged += (sender, args) => UsersForm.ReDraw();

			ImageCellBinding = Binding.Property<UserObject, Image>(u => UsersViewModel.UserUIExtensions.GetCameraImage(u));
			// ImageCellBinding.Changed += ImageCellBinding_Changed;

			VisibleCellBinding = Binding.Property<UserObject, bool?>(u => u.Visible);
			VisibleCellBinding.Changed += VisibleCellBinding_Changed; ;

			TextCellBinding = Binding.Property<UserObject, string>(u => u.Name);
			// TextCellBinding.Changed += TextCellBinding_Changed;

			SetUsers();

			PropertyChanged += ViewModel_PropertyChanged;
			UserTable.OnUserRemoved += UserTable_OnUserChanged;
			UserTable.OnUserAdded += UserTable_OnUserChanged;
		}

		private void SetUsers()
		{
			if (CrashDocRegistry.ActiveDoc?.Users is IEnumerable<User> users)
			{
				Users = new ObservableCollection<UserObject>(users.Select(u => new UserObject(u)));
			}
			else
			{
				Users = new ObservableCollection<UserObject>();
			}
		}

		private void UserTable_OnUserChanged(object sender, UserEventArgs e)
		{
			try
			{
				SetUsers();
				UsersForm.ReDraw();
			}
			catch { }
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(Users)) return;
			UpdateCrashUserTable();
		}

		private void UpdateCrashUserTable()
		{
			UserTable userTable = CrashDocRegistry.ActiveDoc.Users;
			foreach (UsersViewModel.UserObject user in Users)
			{
				userTable.Update(user.CUser);
			}

			Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
			UsersForm.ReDraw();
		}

		private void VisibleCellBinding_Changed(object sender, BindingChangedEventArgs e)
		{
			NotifyPropertyChanged(nameof(Users));
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		internal void CycleCameraSetting(object sender, GridCellMouseEventArgs e)
		{
			int row = e.Row;
			int col = e.Column;

			if (row < 0 || col != 0) return;
			if (!e.Buttons.HasFlag(MouseButtons.Primary)) return;

			if (e.Item is UserObject user)
			{
				CameraState state = CycleState(user.Camera);

				if (state == CameraState.Follow)
				{
					for (int i = 0; i < Users.Count; i++)
					{
						UserObject currUser = Users[i];
						if (CameraState.Follow == currUser.Camera)
						{
							currUser.Camera = CameraState.Visible;
							CrashDocRegistry.ActiveDoc.Users.Update(currUser.CUser);
						}
					}

					user.Camera = state;
					UpdateCrashUserTable();
				}

				user.Camera = state;
			}

			NotifyPropertyChanged(nameof(Users));
		}

		private CameraState CycleState(CameraState state)
		{
			int stateCount = (int)state;
			stateCount++;

			if (stateCount >= Enum.GetValues(typeof(CameraState)).Length)
			{
				return CameraState.None;
			}

			return (CameraState)stateCount;
		}


		public event PropertyChangedEventHandler PropertyChanged;


		internal static class UserUIExtensions
		{

			private static Dictionary<CameraState, Image> cameras;

			static UserUIExtensions()
			{
				cameras = new Dictionary<CameraState, Image>
			{
				{ CameraState.None, Icons.CameraNone.ToEto() },
				{ CameraState.Visible, Icons.CameraVisible.ToEto() },
				{ CameraState.Follow, Icons.CameraFollow.ToEto() },
			};
			}

			internal static Image GetCameraImage(UserObject user)
			{
				return cameras[user.Camera];
			}

		}
	}


}
