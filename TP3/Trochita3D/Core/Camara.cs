using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class Camara
    {
        //public float[] eye = new float[3] { 50.0f, 50.0f, 20.0f };
        //public float[] at = new float[3] { 1.0f, 1.0f, 1.0f };
        //public float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        public Punto Eye { get; set; }
        public Punto At { get; set; }
        public Punto Up { get; set; }

        //public Punto Position { get; set; }
        //public Punto Orientation { get; set; }

        //private double currentRotationAboutX = 0.0;
        private const double SENSITIVITY = 50;
        private const double MAX_ANGLE_UP = 1;
        private const double MAX_ANGLE_DOWN = -1;

        public double radius = 1;
        public double moveDist = 1d;

        private double hRadians;
        private double vRadians;

        /*
        public float[] Direccion 
        { 
            get { return new float[] { eye[0] - at[0], eye[1] - at[1], eye[2] - at[2] }; }
        }
        */

        public Camara()
        {
            this.Eye = new Punto(50, 50, 20);
            this.At = new Punto();
            this.Up = new Punto();

            //hRadians = -Math.PI / 2;
            //vRadians = -Math.PI / 4;
            this.RotateCamera(0, 0);
            //this.Position = new Punto();
            //this.Orientation = new Punto();
        }

        public void RotateCamera(double h, double v)
        {
            hRadians += h;
            vRadians += v;

            At.Y = Eye.Y + (double)(radius * Math.Sin(vRadians));
            At.X = Eye.X + (double)(radius * Math.Cos(vRadians) * Math.Cos(hRadians));
            At.Z = Eye.Z + (double)(radius * Math.Cos(vRadians) * Math.Sin(hRadians));

            Up.X = Eye.X - At.X;
            Up.Z = Math.Abs(Eye.Z + (double)(radius * Math.Sin(vRadians + Math.PI / 2)));
            Up.Y = Eye.Y - At.Y;
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




        /*
        public void AvanzarAdelante()
        {
            Punto direccion = new Punto(Direccion[0], Direccion[1], Direccion[2]);

            direccion.Normalizar();

            this.eye[0] -= (float)direccion.X;
            this.eye[1] -= (float)direccion.Y;
            //this.eye[2] += direccion.Z; --> z nunca se incrementa?
        }

        public void AvanzarAtraz()
        {
            Punto direccion = new Punto(Direccion[0], Direccion[1], Direccion[2]);

            direccion.Normalizar();

            this.eye[0] += (float)direccion.X;
            this.eye[1] += (float)direccion.Y;
            //this.eye[2] += direccion.Z; --> z nunca se incrementa?
        }

        public void AvanzarDerecha()
        {
            Punto direccion = new Punto(Direccion[0], Direccion[1], Direccion[2]);
            
            direccion.Normalizar();

            // sacar vector ortogonal a la proyección en xy -> es rotarlo 90 grados...
            direccion.RotarProyeccionXY(Math.PI / 2d);

            this.eye[0] += (float)direccion.X;
            this.eye[1] += (float)direccion.Y;
            //this.eye[2] += direccion.Z; --> z nunca se incrementa?
        }

        public void AvanzarIzquierda()
        {
            Punto direccion = new Punto(Direccion[0], Direccion[1], Direccion[2]);

            direccion.Normalizar();

            // sacar vector ortogonal a la proyección en xy -> es rotarlo 90 grados...
            direccion.RotarProyeccionXY(Math.PI / 2d);

            this.eye[0] -= (float)direccion.X;
            this.eye[1] -= (float)direccion.Y;
            //this.eye[2] += direccion.Z; --> z nunca se incrementa?
        }

        public void CambiarDireccion(int x, int y, int width, int heigth)
        {
            int middleX = width / 2;
            int middleY = heigth / 2;

            if ((x == middleX) && (y == middleY))
                return;

            Punto direccion = new Punto();
            direccion.X = (middleX - x) / SENSITIVITY;
            direccion.Y = (middleY - y) / SENSITIVITY;

            currentRotationAboutX += direccion.Y;

            if (currentRotationAboutX > MAX_ANGLE_UP)
            {
                currentRotationAboutX = MAX_ANGLE_UP;
                //return;
            }

            if (currentRotationAboutX < MAX_ANGLE_DOWN)
            {
                currentRotationAboutX = MAX_ANGLE_DOWN;
                //return;
            }

            Punto axis = (At - Eye) * Up;
            RotateCamera(direccion.Y, axis.X, axis.Y, axis.Z);
            RotateCamera(direccion.X, 0, 1, 0);
        }

        private void RotateCamera(double angle, double x, double y, double z)
        {
            Quaternion temp = new Quaternion(Math.Cos(angle / 2), new Vector());
            Quaternion quat_view = new Quaternion(0, new Vector());
            Quaternion result = new Quaternion();

            temp.Vector.X = x * Math.Sin(angle / 2);
            temp.Vector.Y = y * Math.Sin(angle / 2);
            temp.Vector.Z = z * Math.Sin(angle / 2);

            quat_view.Vector.X = At.X;
            quat_view.Vector.Y = At.Y;
            quat_view.Vector.Z = At.Z;

            result = (temp * quat_view) * temp.Conjugate;

            At.X = result.Vector.X;
            At.Y = result.Vector.Y;
            At.Z = result.Vector.Z;
        }
        */

        /*
        public void Rotate(double angleX, double angleY)
        {
            Orientation.X = (Orientation.X + angleX) % 360;
            Orientation.Y = (Orientation.Y + angleY) % 360;
        }

        private Punto ToVectorInFixedSystem1(double dx, double dy, double dz)
        {
            //Don't calculate for nothing ...
            if (dx == 0.0f & dy == 0.0f && dz == 0.0f)
                return new Punto();

            //Convert to Radian : 360° = 2PI
            double xRot = ToRadians(Orientation.X);
            double yRot = ToRadians(Orientation.Y);

            //Calculate the formula
            float x = (float)(dx * Math.Cos(yRot) + dy * Math.Sin(xRot) * Math.Sin(yRot) - dz * Math.Cos(xRot) * Math.Sin(yRot));
            float y = (float)(+dy * Math.Cos(xRot) + dz * Math.Sin(xRot));
            float z = (float)(dx * Math.Sin(yRot) - dy * Math.Sin(xRot) * Math.Cos(yRot) + dz * Math.Cos(xRot) * Math.Cos(yRot));

            //Return the vector expressed in the global axis system
            return new Punto(x, y, z);
        }

        public Punto ToVectorInFixedSystem2(double dx, double dy, double dz)
        {
            //Don't calculate for nothing ...
            if (dx == 0.0f & dy == 0.0f && dz == 0.0f)
                return new Punto();

            //Convert to Radian : 360° = 2PI
            double xRot = ToRadians(Orientation.X);
            double yRot = ToRadians(Orientation.Y);

            //Calculate the formula
            float x = (float)(dx * Math.Cos(yRot) + 0 - dz * Math.Sin(yRot));
            float y = (float)(0 + dy * Math.Cos(xRot) + 0);
            float z = (float)(dx * Math.Sin(yRot) + 0 + dz * Math.Cos(yRot));

            //Return the vector expressed in the global axis system
            return new Punto(x, y, z);
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public void Update()
        {
            //Get upward and forward vector, convert vectors to fixed coordinate sstem (similar than for translation 1)
            Up = ToVectorInFixedSystem1(0.0f, 1.0f, 0.0f);        //Note: need to calculate at each frame
            Punto forward = ToVectorInFixedSystem1(0.0f, 0.0f, 1.0f);
            Eye = Position;

            At.X = Position.X + forward.X;
            At.Y = Position.Y + forward.Y;
            At.Z = Position.Z + forward.Z;
        }
        */

    }
}
