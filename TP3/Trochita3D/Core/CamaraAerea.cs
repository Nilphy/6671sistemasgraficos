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
    public class CamaraAerea : Camara
    {
        public CamaraAerea()
        {
            this.Eye = new Punto(50, 50, 60);
            this.At = new Punto(50, 50, 0);
            this.Up = new Punto(1, 0, 0);

           // this.RotateCamera(0, 0);
        }

        public override void UpdateCameraByMouse(bool mousing)
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

        public override void RotateCamera(double h, double v)
        {
            hRadians += h;
            vRadians += v;

            At.X = Eye.X + (double)(radius * Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Y = Eye.Y + (double)(radius * Math.Cos(vRadians) * Math.Sin(hRadians));
            At.Z = Eye.Z + (double)(radius * Math.Sin(vRadians));
        }

        public override void MoveCamera(float d)
        {
            Eye.X += d * moveDist * (float)(Math.Cos(vRadians) * Math.Cos(hRadians));
            Eye.Y += d * moveDist * (float)(Math.Cos(vRadians) * Math.Sin(hRadians));
            At.X += d * moveDist * (float)(Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Y += d * moveDist * (float)(Math.Cos(vRadians) * Math.Sin(hRadians));
            //Eye.Z += d * moveDist * (float)Math.Sin(vRadians);
            //RotateCamera(0, 0);
        }

        public override void SlideCamera(float h, float v)
        {
            Eye.X += v * moveDist;
            Eye.Y += h * moveDist;
            At.X += v * moveDist;
            At.Y += h * moveDist;
            //Eye.Z += v * moveDist;
            //RotateCamera(0, 0);
        }

    }
}
