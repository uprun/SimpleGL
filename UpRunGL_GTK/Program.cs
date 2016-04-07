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



		List<KeyValuePair<Gdk.Point, Gdk.Color>> pointsToDraw;
		int degrees = 0;
		System.Timers.Timer timer;
		public GdkApp() : base("Simple drawing")
		{
			timer = new System.Timers.Timer (200);
			timer.AutoReset = true;
			timer.Elapsed += (s, e) => {
				preparePointsToDraw (degrees);
				++degrees;
				Gtk.Application.Invoke(delegate {
					draw();
				});

			};

			pointsToDraw = new List<KeyValuePair<Point, Gdk.Color>> ();
			SetDefaultSize(400, 400);

			DeleteEvent+=delegate {Application.Quit(); };
			ShowAll();
			timer.Start ();
		}

		protected void preparePointsToDraw(double rotationDegrees=0.0)
		{
			UpRunGL.ZBuffer zbuffer;
			UpRunGL.Matrix3d projection;

			zbuffer = new UpRunGL.ZBuffer(400, 400, (x, y, r, g, b) => { 
				pointsToDraw.Add( 
					new KeyValuePair<Point, Gdk.Color> (
						new Gdk.Point(x, y),
						new Gdk.Color(r,g,b)));
				return true;
			});
			double halfSize = 25;
			UpRunGL.Triangle3d triangleFrontGreen = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize , -halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, -halfSize),
				new Color(0, 255, 0)
			);
			UpRunGL.Triangle3d triangleFrontRed = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(-halfSize, halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, -halfSize),
				new Color(255, 0, 0)
			);

			UpRunGL.Triangle3d triangleBackBlue = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize , -halfSize, halfSize),
				new UpRunGL.Point3d(halfSize, -halfSize, halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, halfSize),
				new Color(0, 0, 255)
			);
			UpRunGL.Triangle3d triangleBackRedPlusGreen = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize, -halfSize, halfSize),
				new UpRunGL.Point3d(-halfSize, halfSize, halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, halfSize),
				new Color(255, 255, 0)
			);
			UpRunGL.Triangle3d triangleLeftFirstRedPlusBlue = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(-halfSize, halfSize, -halfSize),
				new UpRunGL.Point3d(-halfSize, halfSize, halfSize),
				new Color(255, 0, 255)
			);
			UpRunGL.Triangle3d triangleLeftSecondRedPlusBlue = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(-halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(-halfSize, -halfSize, halfSize),
				new UpRunGL.Point3d(-halfSize, halfSize, halfSize),
				new Color(255, 0, 255)
			);
			UpRunGL.Triangle3d triangleRightFirstGreenPlusBlue = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, halfSize),
				new Color(0, 255, 255)
			);
			UpRunGL.Triangle3d triangleRightSecondGreenPlusBlue = new UpRunGL.Triangle3d (
				new UpRunGL.Point3d(halfSize, -halfSize, -halfSize),
				new UpRunGL.Point3d(halfSize, -halfSize, halfSize),
				new UpRunGL.Point3d(halfSize, halfSize, halfSize),
				new Color(0, 255, 255)
			);
			projection = new UpRunGL.Matrix3d ();
			//projection.InitAsProjectionCenteredMatrix (10.0);
			projection.InitAsProjectionParallelMatrix();


			UpRunGL.Matrix3d rotationMatrix = new UpRunGL.Matrix3d ();
			rotationMatrix.InitAsRotationMatrixOY ( (rotationDegrees / 180.0) * Math.PI );

			UpRunGL.Matrix3d shiftMatrix = new UpRunGL.Matrix3d ();
			shiftMatrix.InitAsShiftMatrix (0, 0, halfSize * 2 + 11);

			UpRunGL.Matrix3d localShift = new UpRunGL.Matrix3d ();
			localShift.InitAsShiftMatrix (200, 200, 0);

			var mulShiftRot = shiftMatrix.Multiply (rotationMatrix);
			var mulProjShiftRot = projection.Multiply (mulShiftRot);

			projection = localShift.Multiply (mulProjShiftRot);


			triangleFrontGreen.DrawToRastr (zbuffer, projection);
			triangleFrontRed.DrawToRastr (zbuffer, projection);
			triangleBackBlue.DrawToRastr (zbuffer, projection);
			triangleBackRedPlusGreen.DrawToRastr (zbuffer, projection);
			triangleLeftFirstRedPlusBlue.DrawToRastr (zbuffer, projection);
			triangleLeftSecondRedPlusBlue.DrawToRastr (zbuffer, projection);
			triangleRightFirstGreenPlusBlue.DrawToRastr (zbuffer, projection);
			triangleRightSecondGreenPlusBlue.DrawToRastr (zbuffer, projection);
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
			base.GdkWindow.Clear ();
			foreach (var point in pointsToDraw) 
			{
				gc.RgbFgColor = point.Value;

				base.GdkWindow.DrawPoint (gc, point.Key.X, point.Key.Y);	
			}
			pointsToDraw.Clear ();

		}
	}
}