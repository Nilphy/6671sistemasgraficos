using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SistemasGraficos.Entidades;

using Tao.OpenGl;

namespace TPAlgoritmos3D.EntidadesVista
{
    public class CamaraPathPlotter
    {
        /// <summary>
        /// Dibuja la curva correspondiente a la trayectoria de la camara. La misma
        /// se encuentra definida por los puntos de control indicados por parámetros.
        /// </summary>
        /// <param name="puntos">Puntos de control</param>
        public static void DibujarCamaraPath(IList<PuntoFlotante> puntos)
        {
            // Dibujo el lazo punteado que une los puntos.
            PlotHelper.DibujarLazoPunteado(puntos);

            // Dibujo los puntos para denotar su ubicación.
            foreach (PuntoFlotante punto in puntos)
            {
                PlotHelper.DibujarPunto(punto);
            }

            // Dibujo la curva que se forma a partir de los puntos.
            if (puntos.Count > 4)
            {
                CurvaBsplineSegmentosCubicos curvaCamino = new CurvaBsplineSegmentosCubicos(puntos);

                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glPushMatrix();
                Gl.glColor3d(0.5, 0.5, 0);
                foreach (PuntoFlotante punto in curvaCamino.GetPuntosDiscretos(0.001))
                {
                    Gl.glVertex2d(punto.X, punto.Y);
                }
                Gl.glPopMatrix();
                Gl.glEnd();
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glColor3d(1, 1, 1);
            }
        }

    }
}
