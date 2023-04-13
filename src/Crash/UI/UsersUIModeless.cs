using System.ComponentModel;

using Eto.Drawing;
using Eto.Forms;

using Rhino.UI;


namespace Crash.UI
{

	internal sealed class UsersForm : Form
	{
		private UsersViewModel ViewModel;


		private GridView m_grid;
		internal static Crash.UI.UsersForm? ActiveForm { get; set; }

		internal static void ShowForm()
		{
			var form = new UsersForm();
			form.Closed += OnFormClosed;
			form.Show();

			ActiveForm = form;
		}

		internal static void ToggleFormVisibility()
		{
			if (null == ActiveForm)
			{
				ShowForm();
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

		internal static void ReDraw()
		{
			if (null == ActiveForm)
			{
				ActiveForm = new UsersForm();
			}
			try
			{
				ActiveForm.m_grid.Invalidate(true);
				ActiveForm.Invalidate(true);
			}
			catch { }
		}

		/// <summary>
		/// FormClosed EventHandler
		/// </summary>
		private static void OnFormClosed(object sender, EventArgs e)
		{
			UsersForm.ActiveForm?.Dispose();
			UsersForm.ActiveForm = null;
		}

		private void ReDrawEvent(object sender, EventArgs e) => ReDraw();

		internal UsersForm()
		{
			Owner = RhinoEtoApp.MainWindow;
			ViewModel = new UsersViewModel();
			CreateForm();
		}

		private void CreateForm()
		{
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
				DataStore = ViewModel.Users.Cast<object>(),
				ShowHeader = true,
				Width = 270,
				Border = BorderType.Bezel,
				AllowEmptySelection = true,
				RowHeight = 24,
			};
			ViewModel.View = m_grid;
			m_grid.CellClick += ViewModel.CycleCameraSetting;
			m_grid.DataContextChanged += M_grid_DataContextChanged;

			// TODO : Implement Sorting
			// m_grid.ColumnHeaderClick += M_grid_ColumnHeaderClick;

			// Camera
			var ivc = new ImageViewCell();

			m_grid.Columns.Add(new GridColumn
			{
				DataCell = new ImageViewCell
				{
					Binding = ViewModel.ImageCellBinding,
				},
				Editable = false,
				HeaderText = "",
				Resizable = false,
				Sortable = false,
				Width = 36,
			});

			// Visible
			m_grid.Columns.Add(new GridColumn
			{
				DataCell = new CheckBoxCell
				{
					Binding = ViewModel.VisibleCellBinding,
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
			m_grid.Columns.Add(new GridColumn
			{
				DataCell = new TextBoxCell
				{
					Binding = ViewModel.TextCellBinding
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

		private void M_grid_DataContextChanged(object sender, EventArgs e)
		{
			;
		}

		private void Cell_Paint(object sender, CellPaintEventArgs e)
		{
			if (e.Item is not UsersViewModel.UserObject user) return;

			// e.Graphics.DrawEllipse(user.Color.ToEto(), new RectangleF(2, 2, 16, 16),);
			e.Graphics.FillEllipse(user.Colour.ToEto(), new RectangleF(2, 2, 16, 16));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			this.SavePosition();
			base.OnClosing(e);
		}

	}

}
