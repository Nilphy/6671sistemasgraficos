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
    public class CamaraTerreno : Camara
    {
        public CamaraTerreno()
        {
            this.Eye = new Punto(10, 10, 4);
            this.At = new Punto(15, 15, 4);
            this.Up = new Punto(0, 0, 1);

            this.RotateCamera(0, 0);
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
            //Eye.Z += d * moveDist * (float)Math.Sin(vRadians);
            RotateCamera(0, 0);
        }

        public override void SlideCamera(float h, float v)
        {
            Eye.X += h * moveDist * (float)Math.Cos(hRadians + Math.PI / 2);
            Eye.Y += h * moveDist * (float)Math.Sin(hRadians + Math.PI / 2);
            //Eye.Z += v * moveDist;
            RotateCamera(0, 0);
        }

    }
}
