using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using SistemasGraficos.Entidades;

namespace TPAlgoritmos3D.EntidadesVista
{
    /// <summary>
    /// Clase encargada de dibujar la curva que representa el terreno.
    /// </summary>
    public class Terreno2DPlotter
    {
        /// <summary>
        /// Dibuja la curva que representa el perfil del terreno.
        /// </summary>
        /// <param name="puntos">Puntos de control</param>
        public static void DibujarTerreno2D(IList<PuntoFlotante> puntos)
        {
            // Dibujo el lazo punteado que une los puntos.
            PlotHelper.DibujarLazoPunteado(puntos);

            // Dibujo los puntos para denotar su ubicación.
            foreach (PuntoFlotante punto in puntos)
            {
                PlotHelper.DibujarPunto(punto);
            }

            /*
            // Dibujo la curva que se forma a partir de los puntos.
            if (puntos.Count > 2)
            {
                CurvaBzierSegmentosCubicos curvaCamino = new CurvaBzierSegmentosCubicos(puntos);

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
            */
        }

    }
}
