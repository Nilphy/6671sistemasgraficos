using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

using Trochita3D.Core;
using Trochita3D.Curvas;

namespace Trochita3D.Entidades
{
    public class Terraplen : SuperficieBarrido
    {
        // Cantidad de puntos que definen la sección del terraplen.
        private const int CANT_PTOS_SECCION = 10;

        private IList<Punto> puntosControl = new List<Punto>();
        private CurvaBzierSegmentosCubicos curvaSeccion;
        private IList<double> distanciaAcumuladaPorPuntoPath;

        public Terraplen(double altura)
        {
            this.RENDER_MODE = Gl.GL_TRIANGLE_STRIP;

            // Curva del terraplen
            puntosControl.Add(new Punto(0, -2, 0));
            puntosControl.Add(new Punto(0, -1.6, 1.6));
            puntosControl.Add(new Punto(0, -1.1, 1.8));
            puntosControl.Add(new Punto(0, -1, 2));

            // Plano del terraplen
            puntosControl.Add(new Punto(0, -0.5, altura));
            puntosControl.Add(new Punto(0, 0.5, altura));
            puntosControl.Add(new Punto(0, 1, 2));

            // Curva del terraplen
            puntosControl.Add(new Punto(0, 1, 2));
            puntosControl.Add(new Punto(0, 1.1, 1.8));
            puntosControl.Add(new Punto(0, 1.6, 1.6));
            puntosControl.Add(new Punto(0, 2, 0));

            this.curvaSeccion = new CurvaBzierSegmentosCubicos(puntosControl);
        }

        protected override void LoadMaterialProperties()
        {
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.6f, 0.5f, 0.35f, 0.25f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.61568f, 0.48627f, 0.34117f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
        }

        public override Seccion GetSeccion()
        {
            return new Seccion(curvaSeccion.GetPuntosDiscretos(3d / CANT_PTOS_SECCION));
        }

        public override void SetCamino(IList<Punto> camino)
        {
            base.SetCamino(camino);
            this.CalcularDistanciaAcumulada();
        }

        private void CalcularDistanciaAcumulada()
        {
            distanciaAcumuladaPorPuntoPath = new List<double>();
            Punto puntoAnterior = camino[camino.Count - 1];
            Punto puntoActual;
            for (int i = 0; i < camino.Count; i++)
            {
                puntoActual = camino[i];

                if (i == 0)
                    distanciaAcumuladaPorPuntoPath.Add(0d);
                else
                    distanciaAcumuladaPorPuntoPath.Add(puntoAnterior.CalcularDistancia(puntoActual) + distanciaAcumuladaPorPuntoPath[i - 1]);

                puntoAnterior = puntoActual;
            }
        }

        public Punto GetPositionByDistancia(double distancia)
        {
            double distanciaDesdeElComienzoDeLaVuelta = distancia % distanciaAcumuladaPorPuntoPath[distanciaAcumuladaPorPuntoPath.Count - 1];

            for (int i = 0; i < camino.Count; i++)
            {
                if (distanciaDesdeElComienzoDeLaVuelta <= distanciaAcumuladaPorPuntoPath[i])
                {
                    return camino[i];
                }
            }

            throw new InvalidProgramException("No se encontró el punto donde supera la distancia");
        }

        public double GetInclinacionByDistancia(double distancia)
        {
            double distanciaDesdeElComienzoDeLaVuelta = distancia % distanciaAcumuladaPorPuntoPath[distanciaAcumuladaPorPuntoPath.Count - 1];

            for (int i = 0; i < camino.Count; i++)
            {
                if (distanciaDesdeElComienzoDeLaVuelta <= distanciaAcumuladaPorPuntoPath[i])
                {
                    return secciones[i].Angulo * (180 / Math.PI) + 90;
                }
            }

            throw new InvalidProgramException("No se encontró el punto donde supera la distancia");
        }
    }
}
