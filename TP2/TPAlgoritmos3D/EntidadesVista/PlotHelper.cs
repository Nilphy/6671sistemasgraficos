using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using SistemasGraficos.Entidades;

namespace TPAlgoritmos3D.EntidadesVista
{
    public class PlotHelper
    {

        public static void DibujarLazoPunteado<T>(IList<T> puntos) where T : Punto
        {
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glLineStipple(4, 0xAAAA);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glPushMatrix();

            foreach (Punto punto in puntos)
            {
                Gl.glVertex2d(punto.GetXFlotante(), punto.GetYFlotante());
            }

            Gl.glPopMatrix();
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        /// <summary>
        /// Dibuja un punto de control de la curva.
        /// </summary>
        /// <param punparam name="punto">Punto a ser dibujado.</param>
        public static void DibujarPunto(Punto punto)
        {
            // TODO: ver de sacar este parametro a traves de la configuracion de la vista.
            double DELTA = 0.01;
            Gl.glPushMatrix();

            Gl.glDisable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();
            Gl.glPushMatrix();
            Gl.glColor3d(1, 0, 0);
            Gl.glTranslated(punto.GetXFlotante(), punto.GetYFlotante(), 0);
            Glu.gluDisk(quad, 0, DELTA, 20, 20);
            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glColor3d(1, 1, 1);
            Glu.gluDeleteQuadric(quad);
        }

    }
}
