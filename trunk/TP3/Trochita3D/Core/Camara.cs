using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class Camara
    {
        private const double SENSITIVITY = 50;
        private const double MAX_ANGLE_UP = 1;
        private const double MAX_ANGLE_DOWN = -1;

        private double radius = 1;
        private double moveDist = 1;

        private double hRadians;
        private double vRadians;

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

        public void RotateCamera(double h, double v)
        {
            hRadians += h;
            vRadians += v;

            At.X = Eye.X + (double)(radius * Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Y = Eye.Y + (double)(radius * Math.Cos(vRadians) * Math.Sin(hRadians));
            At.Z = Eye.Z + (double)(radius * Math.Sin(vRadians));
            /*
            At.Y = Eye.Y + (double)(radius * Math.Sin(vRadians));
            At.X = Eye.X + (double)(radius * Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Z = Eye.Z + (double)(radius * Math.Cos(vRadians) * Math.Sin(hRadians));
            */
            /*
            Up.X = Eye.X - At.X;
            Up.Z = Math.Abs(Eye.Z + (double)(radius * Math.Sin(vRadians + Math.PI / 2)));
            Up.Y = Eye.Y - At.Y;
            */
        }

        public void MoveCamera(float d)
        {
            Eye.Y += d * moveDist * (float)Math.Sin(vRadians);
            Eye.X += d * moveDist * (float)(Math.Cos(vRadians) * Math.Cos(hRadians));
            Eye.Z += d * moveDist * (float)(Math.Cos(vRadians) * Math.Sin(hRadians));
            RotateCamera(0, 0);
        }

        public void SlideCamera(float h, float v)
        {
            Eye.Y += v * moveDist;
            Eye.X += h * moveDist * (float)Math.Cos(hRadians + Math.PI / 2);
            Eye.Z += h * moveDist * (float)Math.Sin(hRadians + Math.PI / 2);
            RotateCamera(0, 0);
        }

    }
}
