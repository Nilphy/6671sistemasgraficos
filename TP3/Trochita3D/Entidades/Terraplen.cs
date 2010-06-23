using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

using Trochita3D.Core;
using Trochita3D.Curvas;

namespace Trochita3D.Entidades
{
    public class Terraplen
    {

        public const double ALTURA = 2.2;

        private IList<PuntoFlotante> puntosControl = new List<PuntoFlotante>();
        private CurvaBzierSegmentosCubicos curvaCamino;

        public Terraplen()
        {
            // Curva del terraplen
            puntosControl.Add(new PuntoFlotante(0, -2, 0));
            puntosControl.Add(new PuntoFlotante(0, -1.6, 1.6));
            puntosControl.Add(new PuntoFlotante(0, -1.1, 1.8));
            puntosControl.Add(new PuntoFlotante(0, -1, 2));

            // Plano del terraplen
            puntosControl.Add(new PuntoFlotante(0, -0.5, ALTURA));
            puntosControl.Add(new PuntoFlotante(0, 0.5, ALTURA));
            puntosControl.Add(new PuntoFlotante(0, 1, 2));

            // Curva del terraplen
            puntosControl.Add(new PuntoFlotante(0, 1, 2));
            puntosControl.Add(new PuntoFlotante(0, 1.1, 1.8));
            puntosControl.Add(new PuntoFlotante(0, 1.6, 1.6));
            puntosControl.Add(new PuntoFlotante(0, 2, 0));

            this.curvaCamino = new CurvaBzierSegmentosCubicos(puntosControl);
        }

        public Seccion GetSeccion(int cantPuntos)
        {
            return new Seccion(this.curvaCamino.GetPuntosDiscretos(1d / cantPuntos));
        }

        public void Dibujar()
        {
            Gl.glPushMatrix();
            Gl.glBegin(Gl.GL_LINE_STRIP);

            Gl.glColor3d(0.5, 0.5, 0);
            foreach (PuntoFlotante punto in curvaCamino.GetPuntosDiscretos(0.001))
            {
                Gl.glVertex3d(punto.X, punto.Y, punto.Z);
            }

            Gl.glEnd();
            Gl.glPopMatrix();
        }

    }
}
