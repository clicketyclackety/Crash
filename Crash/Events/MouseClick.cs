using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crash.Utilities;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.UI;
using SpeckLib;

namespace Crash.Events
{
	/// <summary>
	/// Mouse events
	/// </summary>
	public class MouseEvent : MouseCallback
	{
		public static string selectedUser = string.Empty;

		public static Point3d tagLocation = Point3d.Unset;

		/// <summary>
		/// Active mouse Event
		/// </summary>
		private static MouseEvent active;

		/// <summary>
		/// Constructor that sets RegisterMouseEvents to true.
		/// </summary>
		private MouseEvent(bool active = true)
		{

			Enabled = active;
		}

		/// <summary>
		/// Method to intialize mouse event
		/// </summary>
		public static void CreateRegisterMouseEvent()
		{
			active = new MouseEvent();
		}

		/// <summary>
		/// Method to disable mouse events
		/// </summary>
		public static void DisableMouseEvent()
		{
			if (active is object)
				active.Enabled = false;
		}

		/// <summary>
		/// Method that is called on Mouse Up.
		/// Events can be added here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseCallbackEventArgs e)
		{

		}

		/// <summary>
		/// on mouse down events
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseCallbackEventArgs e)
		{
			
		}

		/// <summary>
		/// on mouse move events
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseCallbackEventArgs e)
		{
			Line line = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint);
			if (FindSelected(line))
				Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
			
		}

		private bool FindSelected(Line line)
        {
			IEnumerable<Speck> specks = LocalCache.Instance.GetSpecks();
			var enumer = specks.GetEnumerator();
			while (enumer.MoveNext())
			{
				Speck speck = enumer.Current;
				GeometryBase geom = speck.GetGeom();
				BoundingBox box = geom.GetBoundingBox(true);
				if(Intersection.LineBox(line, box, 100, out _))
                {
					selectedUser = speck.Owner;
					tagLocation = box.Center;
					return true;
                }
			}
			
			selectedUser = string.Empty;
			tagLocation = Point3d.Unset;
			return false;
		}
	}
}
