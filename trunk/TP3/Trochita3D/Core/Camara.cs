using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class Camara
    {
        public float[] eye = new float[3] { 10.0f, 10.0f, 5.0f };
        public float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        public float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        public float[] Direccion 
        { 
            get { return new float[] { eye[0] - at[0], eye[1] - at[1], eye[2] - at[2] }; }
        }

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


    }
}
