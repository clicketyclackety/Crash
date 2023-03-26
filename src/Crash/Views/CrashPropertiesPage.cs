﻿using System.Diagnostics;

using Eto.Drawing;
using Eto.Forms;

using Rhino.UI;

namespace Crash.Views
{
	internal class CrashPropertiesPage : OptionsDialogPage
	{
		private CrashPageControl m_page_control;

		public CrashPropertiesPage()
		  : base("Sample")
		{
		}

		public override bool OnActivate(bool active)
		{
			return (m_page_control == null || m_page_control.OnActivate(active));
		}

		public override bool OnApply()
		{
			return (m_page_control == null || m_page_control.OnApply());
		}

		public override void OnCancel()
		{
			if (m_page_control != null)
				m_page_control.OnCancel();
		}

		public override object PageControl
		{
			get
			{
				return (m_page_control ?? (m_page_control = new CrashPageControl()));
			}
		}

	}

	class CrashPageControl : Panel
	{
		public CrashPageControl()
		{
			var hello_button = new Button { Text = "Hello" };
			hello_button.Click += (sender, e) => OnHelloButton();

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.AddSeparateRow(hello_button, null);
			layout.Add(null);
			Content = layout;
		}

		public bool OnActivate(bool active)
		{
			Debug.WriteLine("SampleCsEtoOptionsDialogPage.OnActive(" + active + ")");
			return true;
		}

		public bool OnApply()
		{
			Debug.WriteLine("SampleCsEtoOptionsDialogPage.OnApply()");
			return true;
		}

		public void OnCancel()
		{
			Debug.WriteLine("SampleCsEtoOptionsDialogPage.OnCancel()");
		}

		/// <summary>
		/// Example of proper way to display a message box
		/// </summary>
		protected void OnHelloButton()
		{
			// Use the Rhino common message box and NOT the Eto MessageBox,
			// the Eto version expects a top level Eto Window as the owner for
			// the MessageBox and will cause problems when running on the Mac.
			// Since this panel is a child of some Rhino container it does not
			// have a top level Eto Window.
			Dialogs.ShowMessage("Hello Rhino!", "Sample");
		}

	}

}
