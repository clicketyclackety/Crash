using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Events
{
    internal static class SelectItem
    {
        internal static void Event(object sender, RhinoObjectSelectionEventArgs e)
        {
            //Test if the selected item is selected in the database
            bool isOwned = false;

            //If you unselect, udpate the database
            if (!e.Selected)
            {

            }
            //if it gets selected and its already owned by someone, unselect the object
            if(e.Selected && isOwned)
                e.RhinoObjects.Select(x => x.Select(false));
            
            
          



        }

    }
}
