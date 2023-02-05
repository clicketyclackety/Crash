using System.Collections.ObjectModel;
using System.ComponentModel;
using Crash.Document;
using Crash.Events;
using Crash.Properties;
using Eto.Drawing;
using Eto.Forms;

using Rhino.UI;


namespace Crash.UI
{

    public static class UserUIExtensions
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

        public static Image GetCameraImage(this User user)
        {
            return cameras[user.Camera];
        }

    }

    internal class UsersForm : Form
    {
        private GridView m_grid;
        private readonly ObservableCollection<User> m_users;
        internal static Crash.UI.UsersForm? ActiveForm { get; set; }

        internal static void ToggleFormVisibility()
        {
            if (null == ActiveForm)
            {
                var form = new UsersForm { Owner = RhinoEtoApp.MainWindow };
                form.Closed += OnFormClosed;
                form.Show();

                ActiveForm = form;
            }
            else
            {
                ActiveForm = null;
            }
        }

        internal static void CloseActiveForm()
        {
            ActiveForm?.Dispose();
        }

        internal static void ReDrawForm()
        {
            ActiveForm?.Invalidate(true);
        }

        /// <summary>
        /// FormClosed EventHandler
        /// </summary>
        private static void OnFormClosed(object sender, EventArgs e)
        {
            UsersForm.ActiveForm?.Dispose();
            UsersForm.ActiveForm = null;
        }

        // TODO : Make UI Respond to New Users
        public UsersForm()
        {
            if (CrashDoc.ActiveDoc is object && ClientManager.LocalClient is object)
            {
                m_users = new ObservableCollection<User>(CrashDoc.ActiveDoc.Users);
                CrashDoc.ActiveDoc.Users.OnUserAdded += (sender, userEventArgs) => ReDrawForm();
                CrashDoc.ActiveDoc.Users.OnUserRemoved += (sender, userEventArgs) => ReDrawForm();
                ClientManager.LocalClient.OnInitialize += (specks) => ReDrawForm();
            }

            Maximizable = false;
            Minimizable = false;
            Padding = new Padding(5);
            Resizable = false;
            ShowInTaskbar = false;
            Title = "Crash";
            WindowStyle = WindowStyle.Default;
            Width = 300;

            m_grid = new GridView
            {
                AllowMultipleSelection = false,
                DataStore = m_users,
                ShowHeader = true,
                Width = 270,
                Border = BorderType.Bezel,
                AllowEmptySelection = true,
                RowHeight = 24,
            };

            m_grid.CellClick += cycleCameraSetting;
            m_grid.ColumnHeaderClick += M_grid_ColumnHeaderClick;

            // Camera
            var imageCellBinding = Binding.Property<User, Image>(u => u.GetCameraImage());
            // imageCellBinding.Changed += RedrawView;
            var ivc = new ImageViewCell();

            m_grid.Columns.Add(new GridColumn
            {
                DataCell = new ImageViewCell
                {
                    Binding = imageCellBinding,
                },
                Editable = false,
                HeaderText = "",
                Resizable = false,
                Sortable = false,
                Width = 36,
            });

            // Visible
            var visibleCellBinding = Binding.Property<User, bool?>(u => u.Visible);
            visibleCellBinding.Changed += RedrawView;
            m_grid.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell
                {
                    Binding = visibleCellBinding,
                },
                AutoSize = true,
                Editable = true,
                HeaderText = "",
                Resizable = false,
                Sortable = false,
                Width = 24,
            });

            DrawableCell cell = new DrawableCell();
            cell.Paint += Cell_Paint;

            // Colours
            m_grid.Columns.Add(new GridColumn
            {
                DataCell = cell,
                AutoSize = true,
                Editable = false,
                HeaderText = "",
                Resizable = false,
                Sortable = false,
                Width = 24,
            });

            // User
            var textCellBinding = Binding.Property<User, string>(u => u.Name);
            textCellBinding.Changed += RedrawView;
            m_grid.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = textCellBinding
                },
                AutoSize = true,
                Editable = false,
                HeaderText = "Name",
                Resizable = false,
                Sortable = false,
            });

            var user_layout = new TableLayout
            {
                // Padding = new Padding(5, 10, 5, 5),
                // Spacing = new Size(5, 5),
                Rows = { new TableRow(null, m_grid, null) }
            };

            Content = new TableLayout
            {
                // Padding = new Padding(5),
                // Spacing = new Size(5, 5),
                Rows =
                {
                  new TableRow(user_layout)
                }
            };

        }

        private void Cell_Paint(object sender, DrawableCellPaintEventArgs e)
        {
            if (e.Item is not User user) return;

            // e.Graphics.DrawEllipse(user.Color.ToEto(), new RectangleF(2, 2, 16, 16),);
            e.Graphics.FillEllipse(user.Color.ToEto(), new RectangleF(2, 2, 16, 16));
        }

        private void M_grid_ColumnHeaderClick(object sender, GridColumnEventArgs e)
        {
            
        }

        private void cycleCameraSetting(object sender, GridCellMouseEventArgs e)
        {
            int row = e.Row;
            int col = e.Column;

            if (row < 0 || col != 0) return;
            if (!e.Buttons.HasFlag(MouseButtons.Primary)) return;

            if (e.Item is User user)
            {
                CameraState state = CycleState(user.Camera);
                if (state == CameraState.Follow)
                {
                    foreach (User userIter in CrashDoc.ActiveDoc.Users)
                    {
                        if (userIter.Camera == CameraState.Follow)
                        {
                            userIter.Camera = CameraState.Visible;
                        }
                    }

                    CameraCache.FollowCamera();
                }

                user.Camera = state;
            }

            m_grid.Invalidate(true);

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
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

        private void RedrawView(object sender, BindingChangedEventArgs e)
        {
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SavePosition();
            base.OnClosing(e);
        }

        protected void OnHelloButton()
        {
            MessageBox.Show(this, "Hello Rhino!", Title, MessageBoxButtons.OK);
        }

    }

}
