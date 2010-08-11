using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Common.Utils;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public abstract class Camara
    {
        protected const double SENSITIVITY = 50;
        protected const double MAX_ANGLE_UP = 1;
        protected const double MAX_ANGLE_DOWN = -1;

        protected double radius = 1;
        protected double moveDist = 1;

        protected double hRadians;
        protected double vRadians;

        protected Point ptLastMousePosit = new Point();
        protected Point ptCurrentMousePosit = new Point();

        public Punto Eye { get; set; }
        public Punto At { get; set; }
        public Punto Up { get; set; }

        public Camara()
        {
            this.Eye = new Punto(50, 50, 20);
            this.At = new Punto();
            this.Up = new Punto(0, 0, 1);

            this.RotateCamera(0, 0);
        }

        public virtual void UpdateCameraByMouse(bool mousing)
        {
            ptCurrentMousePosit.X = Cursor.Position.X;
            ptCurrentMousePosit.Y = Cursor.Position.Y;

            if (mousing)
            {
                int nXDiff = (ptLastMousePosit.X - ptCurrentMousePosit.X);
                int nYDiff = (ptLastMousePosit.Y - ptCurrentMousePosit.Y);

                RotateCamera(MathUtils.DegreeToRadian((double)nXDiff / 3d), MathUtils.DegreeToRadian((double)nYDiff / 3d));
            }

            ptLastMousePosit.X = ptCurrentMousePosit.X;
            ptLastMousePosit.Y = ptCurrentMousePosit.Y;
        }

        public void Look()
        {
            Glu.gluLookAt(Eye.X, Eye.Y, Eye.Z, 
                            At.X, At.Y, At.Z, 
                            Up.X, Up.Y, Up.Z);
        }

        public virtual void RotateCamera(double h, double v)
        {
            hRadians += h;
            vRadians += v;

            At.X = Eye.X + (double)(radius * Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Y = Eye.Y + (double)(radius * Math.Cos(vRadians) * Math.Sin(hRadians));
            At.Z = Eye.Z + (double)(radius * Math.Sin(vRadians));
        }

        public virtual void MoveCamera(float d)
        {
            Eye.X += d * moveDist * (float)(Math.Cos(vRadians) * Math.Cos(hRadians));
            Eye.Y += d * moveDist * (float)(Math.Cos(vRadians) * Math.Sin(hRadians));
            Eye.Z += d * moveDist * (float)Math.Sin(vRadians);
            RotateCamera(0, 0);
        }

        public virtual void SlideCamera(float h, float v)
        {
            Eye.X += h * moveDist * (float)Math.Cos(hRadians + Math.PI / 2);
            Eye.Y += h * moveDist * (float)Math.Sin(hRadians + Math.PI / 2);
            Eye.Z += v * moveDist;
            RotateCamera(0, 0);
        }

    }
}
