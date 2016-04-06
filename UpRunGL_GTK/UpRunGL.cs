﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UpRunGL_GTK
{
	public  class Color 
	{
		public byte Red { get; set;}
		public byte Green { get; set;}
		public byte Blue { get; set;}
		public Color(byte red, byte green, byte blue)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;

			
		}

	}
	
	public class UpRunGL
	{
		public class Point3d
		{
			public double[] mas;
			public Point3d()
			{
				mas = new double[4];
				for (int i = 0; i < 4; i++)
				{
					mas[i] = new double();
					mas[i] = 0;
				}
				mas[3] = 1;
			}
			public Point3d(double x, double y, double z)
			{
				mas = new double[4];
				for (int i = 0; i < 4; i++)
				{
					mas[i] = new double();
					mas[i] = 0;
				}
				mas[3] = 1;
				X = x;
				Y = y;
				Z = z;
			}
			public void NormalizeByOne()
			{
				for (int i = 0; i < 3; i++)
					mas[i] /= mas[3];
				mas[3] = 1;
			}
			public void InitFrom(Point3d source)
			{
				for (int i = 0; i < 4; i++)
					mas[i] = source.mas[i];
			}
			public void InitFrom(double[] mas2)
			{
				if (mas2.Length != 4)
					throw new Exception(String.Format("Point3d failed method :InitFrom , length of mas2 should be 4 instead of {0}", mas2.Length));
				for (int i = 0; i < 4; i++)
					mas[i] = mas2[i];
			}
			public void ToZero()
			{
				for (int i = 0; i < 4; i++)
					mas[i] = 0;
			}
			public Point3d Clone()
			{
				Point3d buf = new Point3d();
				buf.InitFrom(this);
				return buf;
			}
			public double X
			{
				get
				{
					return mas[0];
				}
				set
				{
					mas[0] = value;
				}
			}
			public double Y
			{
				get
				{
					return mas[1];
				}
				set
				{
					mas[1] = value;
				}
			}
			public double Z
			{
				get
				{
					return mas[2];
				}
				set
				{
					mas[2] = value;
				}
			}
			public double One
			{
				get
				{
					return mas[3];
				}
				set
				{
					mas[3] = value;
				}
			}
		}
		public class Matrix3d
		{
			public double[][] mas;
			public Matrix3d()
			{
				mas = new double[4][];
				for (int i = 0; i < 4; i++)
					mas[i] = new double[4];
				for (int i = 0; i < 4; i++)
					for (int j = 0; j < 4; j++)
					{
						mas[i][j] = new double();
						mas[i][j] = 0;
					}
			}
			public void InitFrom(Matrix3d source)
			{
				for (int i = 0; i < 4; i++)
					for (int j = 0; j < 4; j++)
					{
						mas[i][j] = source.mas[i][j];
					}
			}
			public void ToZero()
			{
				for (int i = 0; i < 4; i++)
					for (int j = 0; j < 4; j++)
					{
						mas[i][j] = 0;
					}
			}
			public void Ones()
			{
				for (int i = 0; i < 4; i++)
					mas[i][i] = 1;
			}
			public void Clone()
			{
				Matrix3d buf = new Matrix3d();
				buf.InitFrom(this);
			}
			public Matrix3d Multiply(Matrix3d B)
			{
				Matrix3d buf = new Matrix3d();
				for (int i = 0; i < 4; i++)
					for (int k = 0; k < 4; k++)
						for (int j = 0; j < 4; j++)
							buf.mas[i][k] += mas[i][j] * B.mas[j][k];
				return buf;
			}
			public Point3d Multiply(Point3d b)
			{
				Point3d buf = new Point3d();
				for (int i = 0; i < 4; i++)
					for (int j = 0; j < 4; j++)
						buf.mas[i] += mas[i][j] * b.mas[j];
				return buf;
			}
			public void InitAsRotationMatrixOX(double Rad)
			{
				double cs, sn;
				sn = Math.Sin(Rad);
				cs = Math.Cos(Rad);
				ToZero();
				Ones();
				mas[1][1] = cs;
				mas[2][2] = cs;
				mas[1][2] = sn;
				mas[2][1] = -sn;
			}
			public void InitAsRotationMatrixOY(double Rad)
			{
				double cs, sn;
				sn = Math.Sin(Rad);
				cs = Math.Cos(Rad);
				ToZero();
				Ones();
				mas[0][0] = cs;
				mas[2][2] = cs;
				mas[0][2] = sn;
				mas[2][0] = -sn;
			}
			public void InitAsRotationMatrixOZ(double Rad)
			{
				double cs, sn;
				sn = Math.Sin(Rad);
				cs = Math.Cos(Rad);
				ToZero();
				Ones();
				mas[0][0] = cs;
				mas[1][1] = cs;
				mas[0][1] = sn;
				mas[1][0] = -sn;
			}
			public void InitAsScaleMatrix(double sX, double sY, double sZ)
			{
				ToZero();
				mas[3][3] = 1;
				mas[0][0] = sX;
				mas[1][1] = sY;
				mas[2][2] = sZ;
			}
			public void InitAsShiftMatrix(double dX, double dY, double dZ)
			{
				ToZero();
				Ones();
				mas[0][3] = dX;
				mas[1][3] = dY;
				mas[2][3] = dZ;
			}
			public void InitAsProjectionCenteredMatrix(double dist)
			{
				ToZero();
				Ones();
				mas[2][2] = 0;
				mas[3][2] = 1 / dist;
			}
			public void InitAsProjectionParallelMatrix()
			{
				ToZero();
				Ones();
				mas[2][2] = 0;
			}
		}
		public class ZBuffer
		{
			//public Bitmap bmprastr;
			public double[][] mas;

			private Func<int, int, byte, byte, byte, bool> drawColorPoint;
			int width,height;
			public int Height
			{
				get
				{
					return height;
				}
			}
			public int Width
			{
				get
				{
					return width;
				}
			}
			public ZBuffer(int width, int height, Func<int, int, byte, byte, byte, bool> drawColorPoint)
			{
				this.drawColorPoint = drawColorPoint;
				this.width = width;
				this.height = height;

				mas = new double[height][];
				for (int i = 0; i < height; i++)
				{
					mas[i] = new double[width];
					for (int j = 0; j < width; j++)
					{
						mas[i][j] = new double();
						mas[i][j]=double.PositiveInfinity;
					}
				}
			}
			public bool TrySetPixel(int x, int y, double z,Color clr)
			{
				if (x < 0 || x >= width || y < 0 || y >= height)
					return false;
				if (z <= 0)
					return false;
				if (mas[y][x] <= z)
					return false;
				mas[y][x] = z;
				return drawColorPoint (x, y, clr.Red, clr.Green, clr.Blue);
			}
		}
		public class Box3d
		{

		}
		public class Triangle3d
		{
			public Point3d[] mas;
			public Color clr;
			//todo add draw to rastr
			public Triangle3d()
			{
				mas = new Point3d[3];
				for (int i = 0; i < 3; i++)
					mas[i] = new Point3d();
				clr = new Color(0, 255, 0);
			}
			public Triangle3d(Point3d first, Point3d second, Point3d third)
			{

				mas = new Point3d[3];
				for (int i = 0; i < 3; i++)
					mas[i] = new Point3d();
				mas[0].InitFrom(first);
				mas[1].InitFrom(second);
				mas[2].InitFrom(third);
				clr = new Color(0, 255, 0);
			}
			public Triangle3d(Point3d first, Point3d second, Point3d third,Color clr)
			{
				mas = new Point3d[3];
				for (int i = 0; i < 3; i++)
					mas[i] = new Point3d();
				mas[0].InitFrom(first);
				mas[1].InitFrom(second);
				mas[2].InitFrom(third);
				this.clr = clr;
			}

			public void DrawToRastr(ZBuffer zbuf, Matrix3d projection)
			{
				int lowerY = zbuf.Height, higherY = 0;
				Point3d buf = null,buf2=null;
				int[] x_right,x_left;
				double[] z_left,z_right;
				x_right=new int[zbuf.Height];
				x_left=new int[zbuf.Height];
				z_left = new double[zbuf.Height];
				z_right = new double[zbuf.Height];
				for (int i = 0; i < 3; i++)
				{
					buf = projection.Multiply(mas[i]);
					buf.NormalizeByOne();
					int y_buf=(int)Math.Floor(buf.Y);
					if (buf.Y <= lowerY)
						lowerY = y_buf;
					if (buf.Y >= higherY)
						higherY = y_buf+1;
				}
				if (higherY >= zbuf.Height)
					higherY = zbuf.Height - 1;
				if (lowerY < 0)
					lowerY = 0;
				for (int i = lowerY; i <= higherY; i++)
				{
					x_left[i] = zbuf.Width - 1;
					x_right[i] = 0;
					z_left[i] = double.PositiveInfinity;
					z_right[i] = double.PositiveInfinity;
				}
				for(int i=0;i<3;i++)
				{
					int x0,y0,x1,y1;
					double z0,z1;
					buf=projection.Multiply(mas[i]);
					buf2=projection.Multiply(mas[(i+1)%3]);
					buf.NormalizeByOne();
					buf2.NormalizeByOne();
					x0=(int)Math.Floor(buf.X);
					y0=(int)Math.Floor(buf.Y);
					z0=mas[i].Z;
					x1=(int)Math.Floor(buf2.X);
					y1=(int)Math.Floor(buf2.Y);
					z1=mas[(i+1)%3].Z;
					zbuf.TrySetPixel(x0, y0, z0, clr);
					DrawOnBMPRastr.DrawBrezenhemLine(x0, y0, z0, x1, y1, z1, x_left, x_right, z_left, z_right);
					zbuf.TrySetPixel(x1, y1, z1, clr);
				}

				for (int i = lowerY; i <= higherY; i++)
				{
					int xL, xR,ro;
					double delta_z;
					double z=z_left[i];
					xL = x_left[i];
					xR = x_right[i];

					delta_z = z_right[i] - z_left[i];
					ro = xR - xL;
					if (ro > 0)
						delta_z /= ro;
					for (; xL <= xR; xL++, z += delta_z)
						zbuf.TrySetPixel(xL, i, z, clr);
				}

			}
		}
		public static class DrawOnBMPRastr
		{
			public static void MySwap(ref object x0, ref object x1)
			{
				object buf;
				buf = x0;
				x0 = x1;
				x1 = buf;
			}
			public static void MySwap(ref int x0, ref int x1)
			{
				int buf;
				buf = x0;
				x0 = x1;
				x1 = buf;
			}
			public static void MySwap(ref double x0, ref double x1)
			{
				double buf;
				buf = x0;
				x0 = x1;
				x1 = buf;
			}
			/*
			public static void DrawPixel(int x0, int y0, int ix, int iy, Color clr, Bitmap rastr, bool reflect_y)
			{
				if (reflect_y)
					iy = -iy;
				try
				{
					rastr.SetPixel(ix + x0, iy + y0, clr);
				}
				catch
				{ }
			}
			public static void DrawBCircle(int x0, int y0, int rad, Bitmap rastr, Color clr)
			{
				int ix = rad;
				int iy = 0;
				int radSQ = rad * rad;
				for (; iy <= rad && ix >= 0; )
				{
					rastr.SetPixel(x0 + ix, y0 + iy, clr);
					rastr.SetPixel(x0 + ix, y0 - iy, clr);
					rastr.SetPixel(x0 - ix, y0 + iy, clr);
					rastr.SetPixel(x0 - ix, y0 - iy, clr);
					int ix_b, iy_b;
					ix_b = ix;
					iy_b = iy + 1;
					int r_error = Math.Abs(radSQ - ix_b * ix_b - iy_b * iy_b);
					if (Math.Abs(radSQ - (ix - 1) * (ix - 1) - iy * iy) < r_error)
					{
						r_error = Math.Abs(radSQ - (ix - 1) * (ix - 1) - iy * iy);
						ix_b = ix - 1;
						iy_b = iy;
					}
					if (Math.Abs(radSQ - (ix - 1) * (ix - 1) - (iy + 1) * (iy + 1)) < r_error)
					{
						r_error = Math.Abs(radSQ - (ix - 1) * (ix - 1) - (iy + 1) * (iy + 1));
						ix_b = ix - 1;
						iy_b = iy + 1;
					}
					ix = ix_b;
					iy = iy_b;


				}
			}
			public static void DrawBEllipse(int x0, int y0, int a, int b, Bitmap rastr, Color clr)
			{
				int x, y;
				int bSQ = b * b;
				int aSQ = a * a;
				x = 0;
				y = b;
				bool first = true;
				while (y >= 0)
				{

					rastr.SetPixel(x0 + x, y0 + y, clr);
					rastr.SetPixel(x0 + x, y0 - y, clr);
					rastr.SetPixel(x0 - x, y0 + y, clr);
					rastr.SetPixel(x0 - x, y0 - y, clr);
					if (first == true && (a * a * (2 * y - 1) <= 2 * b * b * (x + 1)))
						first = false;
					if (first)
					{
						x++;
						int bufy = y--;
						int r1b, r1y;
						r1b = Math.Abs(bSQ * x * x + aSQ * bufy * bufy - aSQ * bSQ);
						r1y = Math.Abs(bSQ * x * x + aSQ * y * y - aSQ * bSQ);
						if (r1b < r1y)
							y = bufy;
					}
					else
					{
						y--;
						int bufx = x++;
						int r2b, r2x;
						r2x = Math.Abs(bSQ * x * x + aSQ * y * y - aSQ * bSQ);
						r2b = Math.Abs(bSQ * bufx * bufx + aSQ * y * y - aSQ * bSQ);
						if (r2b < r2x)
							x = bufx;
					}
				}
			}
			public static void DrawBrezenhemLine(int x0, int y0, int x1, int y1, Bitmap rastr, Color clr)
			{
				if (x1 < x0)
				{
					MySwap(ref x0, ref x1);
					MySwap(ref y0, ref y1);
				}
				bool flag_reflect_y = false;
				if (y1 < y0)
				{
					flag_reflect_y = true;
					y1 = y0 + y0 - y1;
				}
				int deltax = (x1 - x0);
				int deltay = (y1 - y0);
				int error = 0;
				int iy = 0;
				int ix = 0;
				if (deltax >= deltay)
				{
					for (ix = 0; ix <= deltax; ix++)
					{
						DrawPixel(x0, y0, ix, iy, clr, rastr, flag_reflect_y);
						//rastr.SetPixel(ix + x0, iy + y0, clr);
						error += deltay;
						if ((error << 1) > deltax)
						{
							error -= deltax;
							iy++;
						}
					}
				}
				else
				{
					for (iy = 0; iy <= deltay; iy++)
					{
						DrawPixel(x0, y0, ix, iy, clr, rastr, flag_reflect_y);
						//rastr.SetPixel(ix + x0, iy + y0, clr);
						error += deltax;
						if ((error << 1) > deltay)
						{
							error -= deltay;
							ix++;
						}
					}
				}
			} */
			public static void DrawPixel(int x0, int y0, int ix, int iy,double z0, double delta_z, int[] x_left, int[] x_right, double[] z_left, double[] z_right, bool reflect_y)
			{
				if (reflect_y)
					iy = -iy;
				int drawX = ix + x0, drawY = iy + y0;
				double d_z = ix * ix + iy * iy;
				double z = d_z * delta_z+z0;
				try
				{
					if (drawX < x_left[drawY])
					{
						x_left[drawY] = drawX;
						z_left[drawY] = z;
					}
					if (drawX > x_right[drawY])
					{
						x_right[drawY] = drawX;
						z_right[drawY] = z;
					}
				}
				catch
				{ }
			}
			public static void DrawBrezenhemLine(int x0, int y0, double z0, int x1, int y1, double z1, int[] x_left, int[] x_right, double[] z_left, double[] z_right)
			{
				if (x1 < x0)
				{
					MySwap(ref x0, ref x1);
					MySwap(ref y0, ref y1);
					MySwap(ref z0, ref z1);
				}
				bool flag_reflect_y = false;
				if (y1 < y0)
				{
					flag_reflect_y = true;
					y1 = y0 + y0 - y1;
				}
				int deltax = (x1 - x0);
				int deltay = (y1 - y0);
				int ro = deltax * deltax+deltay*deltay;
				double delta_z = z1 - z0;
				if (ro != 0)
					delta_z /= ro;
				int error = 0;
				int iy = 0;
				int ix = 0;
				if (deltax >= deltay)
				{
					for (ix = 0; ix <= deltax; ix++)
					{
						DrawPixel(x0, y0, ix, iy, z0, delta_z, x_left, x_right, z_left, z_right, flag_reflect_y);
						//rastr.SetPixel(ix + x0, iy + y0, clr);
						error += deltay;
						if ((error << 1) > deltax)
						{
							error -= deltax;
							iy++;
						}
					}
				}
				else
				{
					for (iy = 0; iy <= deltay; iy++)
					{
						DrawPixel(x0, y0, ix, iy, z0, delta_z, x_left, x_right, z_left, z_right, flag_reflect_y);
						//rastr.SetPixel(ix + x0, iy + y0, clr);
						error += deltax;
						if ((error << 1) > deltay)
						{
							error -= deltay;
							ix++;
						}
					}
				}
			}

		}
	}
}