using Rhino.Commands;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Commands
{

    public sealed class ToggleUsersUI : Command
    {

        private Crash.UI.UsersUIModeless Form { get; set; }

        public override string EnglishName => "ToggleCrashUI";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (null == Form)
            {
                Form = new UsersUIModeless { Owner = RhinoEtoApp.MainWindow };
                Form.Closed += OnFormClosed;
                Form.Show();
            }
            return Result.Success;
        }

        /// <summary>
        /// FormClosed EventHandler
        /// </summary>
        private void OnFormClosed(object sender, EventArgs e)
        {
            Form.Dispose();
            Form = null;
        }
    }
}
