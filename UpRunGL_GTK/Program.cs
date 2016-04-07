using System;
using Gdk;
using Gtk;
using System.Collections.Generic;
namespace UpRunGL_GTK
{
	public class GdkApp : Gtk.Window 
	{
		public static void Main()
		{
			Application.Init();
			new GdkApp();
			Application.Run();


		}

		UpRunGL.ZBuffer zbuffer;
		UpRunGL.Matrix3d projection;

		List<KeyValuePair<Gdk.Point, Gdk.Color>> pointsToDraw;
		public GdkApp() : base("Simple drawing")
		{
			pointsToDraw = new List<KeyValuePair<Point, Gdk.Color>> ();
			SetDefaultSize(400, 400);
			zbuffer = new UpRunGL.ZBuffer(400, 400, (x, y, r, g, b) => { 
				pointsToDraw.Add( 
					new KeyValuePair<Point, Gdk.Color> (
						new Gdk.Point(x, y),
						new Gdk.Color(r,g,b)));
				return true;
			});
			UpRunGL.Triangle3d triangleFrontGreen = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-200 , -200, 0),
				new UpRunGL.Point3d(200, -200, 0),
				new UpRunGL.Point3d(200, 200, 0),
				new Color(0, 255, 0)
			);
			UpRunGL.Triangle3d triangleFrontRed = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-200, -200, 0),
				new UpRunGL.Point3d(-200, 200, 0),
				new UpRunGL.Point3d(200, 200, 0),
				new Color(255, 0, 0)
			);
			projection = new UpRunGL.Matrix3d ();
			projection.InitAsProjectionCenteredMatrix (10.0);


			UpRunGL.Matrix3d rotationMatrix = new UpRunGL.Matrix3d ();
			rotationMatrix.InitAsRotationMatrixOY (0.1);

			UpRunGL.Matrix3d shiftMatrix = new UpRunGL.Matrix3d ();
			shiftMatrix.InitAsShiftMatrix (0, 0, 50);

			UpRunGL.Matrix3d localShift = new UpRunGL.Matrix3d ();
			localShift.InitAsShiftMatrix (200, 200, 0);

			var mulShiftRot = shiftMatrix.Multiply (rotationMatrix);
			var mulProjShiftRot = projection.Multiply (mulShiftRot);

			projection = localShift.Multiply (mulProjShiftRot);
			//projection = localShift.Multiply(projection);

			triangleFrontGreen.DrawToRastr (zbuffer, projection);
			triangleFrontRed.DrawToRastr (zbuffer, projection);
			DeleteEvent+=delegate {Application.Quit(); };
			ShowAll();
		}

		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			draw ();
			return base.OnButtonPressEvent (evnt);
		}

		protected override bool OnExposeEvent (EventExpose evnt)
		{
			bool ok = base.OnExposeEvent (evnt);
			draw ();
			return ok;
		}

		void draw()
		{
			Gdk.GC gc = new Gdk.GC ((Drawable)base.GdkWindow);
			foreach (var point in pointsToDraw) 
			{
				gc.RgbFgColor = point.Value;

				base.GdkWindow.DrawPoint (gc, point.Key.X, point.Key.Y);	
			}
			pointsToDraw.Clear ();

		}
	}
}