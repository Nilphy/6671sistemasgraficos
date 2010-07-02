using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Trochita3D.Core;

namespace Trochita3D.Entidades
{
    /// <summary>
    /// Contiene los datos principales para dibujar un riel.
    /// </summary>
    public class Riel
    {

        private IList<Punto> puntosControl = new List<Punto>();

        public Riel()
        {
            puntosControl.Add(new Punto(0, -0.5, 0));
            puntosControl.Add(new Punto(0, 0.5, 0));
            puntosControl.Add(new Punto(0, 0.5, 0.3));
            puntosControl.Add(new Punto(0, 0.3, 0.3));
            puntosControl.Add(new Punto(0, 0.3, 0.65));
            puntosControl.Add(new Punto(0, 0.5, 0.65));
            puntosControl.Add(new Punto(0, 0.5, 1));
            puntosControl.Add(new Punto(0, -0.5, 1));
            puntosControl.Add(new Punto(0, -0.5, 0.65));
            puntosControl.Add(new Punto(0, -0.3, 0.65));
            puntosControl.Add(new Punto(0, -0.3, 0.3));
            puntosControl.Add(new Punto(0, -0.5, 0.3));
            puntosControl.Add(new Punto(0, -0.5, 0));
        }

        /// <summary>
        /// Obtiene la seccion representada por los puntos de control para dibujar
        /// la seccion del riel.
        /// </summary>
        /// <returns>
        /// Seccion con los puntos de control para dibujar un riel.
        /// </returns>
        public Seccion GetSeccion()
        {
            return new Seccion(puntosControl);
        }

        public void Dibujar()
        {
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glPushMatrix();

            Gl.glColor3d(0.5, 0.5, 0.5);

            foreach (Punto punto in puntosControl)
            {
                Gl.glVertex3d(0, punto.Y, punto.Z);
            }

            Gl.glPopMatrix();
            Gl.glEnd();
        }

    }
}
