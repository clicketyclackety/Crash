using Crash.Utilities;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System;
using System.Collections.Generic;

namespace Crash.Commands
{

    public sealed class CrashPanelCommand : Command
    {

        public CrashPanelCommand()
        {
            Panels.RegisterPanel(PlugIn, typeof(Views.CrashPanel), "Crash", null);
            Instance = this;
        }

        public static CrashPanelCommand Instance { get; private set; }

        public override string EnglishName => "CrashPanel";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panel_id = Views.CrashPanel.PanelId;
            var visible = Panels.IsPanelVisible(panel_id);

            var prompt = (visible)
              ? "Crash panel is visible. New value"
              : "Crash panel is hidden. New value";

            var go = new GetOption();
            go.SetCommandPrompt(prompt);
            var hide_index = go.AddOption("Hide");
            var show_index = go.AddOption("Show");
            var toggle_index = go.AddOption("Toggle");
            go.Get();
            if (go.CommandResult() != Result.Success)
                return go.CommandResult();

            var option = go.Option();
            if (null == option)
                return Result.Failure;

            var index = option.Index;
            if (index == hide_index)
            {
                if (visible)
                    Panels.ClosePanel(panel_id);
            }
            else if (index == show_index)
            {
                if (!visible)
                    Panels.OpenPanel(panel_id);
            }
            else if (index == toggle_index)
            {
                if (visible)
                    Panels.ClosePanel(panel_id);
                else
                    Panels.OpenPanel(panel_id);
            }

            return Result.Success;
        }

    }

}
