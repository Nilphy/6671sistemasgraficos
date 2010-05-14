using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

using Tao.OpenGl;

using Modelo;
using SistemasGraficos.EstrategiasDibujo;
using TPAlgoritmos3D;

namespace SistemasGraficos.Entidades
{
    public class Vista
    {
        // Para los calculos y los límites de las cosas
        public double X_MIN_MUNDO = 0;
        public double X_MAX_MUNDO = 100;
        public double Y_MIN_MUNDO = 0;
        public double Y_MAX_MUNDO = 100;

        public double X_MIN_CAMARA = -15;
        public double X_MAX_CAMARA = 15;
        public double Y_MIN_CAMARA = -15;
        public double Y_MAX_CAMARA = 15;

        // Creo que es 20 pero con 15 seguro queda mejor :S
        public double X_MIN_VIEWPORT_ESCENA3D = -10;
        public double X_MAX_VIEWPORT_ESCENA3D = 10;
        public double Y_MIN_VIEWPORT_ESCENA3D = -5;
        public double Y_MAX_VIEWPORT_ESCENA3D = 5;

        public Escena escena { get; set; }
        private IWindowParameterProvider windowParameterProvider;

        public Vista(IWindowParameterProvider windowParameterProvider, Escena escena)
        {
            this.windowParameterProvider = windowParameterProvider;
            this.escena = escena;
        }

        #region Funciones de escalado que están acá por no tener un mejor lugar



        public void EscalarMundoToEscena3D()
        {
            Gl.glTranslated(0,X_MIN_VIEWPORT_ESCENA3D, Y_MIN_VIEWPORT_ESCENA3D);
            Gl.glScaled(1,(X_MAX_VIEWPORT_ESCENA3D - X_MIN_VIEWPORT_ESCENA3D) / (X_MAX_MUNDO - X_MIN_MUNDO), (Y_MAX_VIEWPORT_ESCENA3D - Y_MIN_VIEWPORT_ESCENA3D) / (Y_MAX_MUNDO - Y_MIN_MUNDO));
            Gl.glTranslated(0,-X_MIN_MUNDO, -Y_MIN_MUNDO);
        }

        public void EscalarPuntosVentanitas(IList<PuntoFlotante> puntosBzier, Boolean mundoOcamara)
        {
            // Obtengo los límites de los puntos y los escalo a los límites del mundo

            float maxX = this.GetMaxX(puntosBzier);
            float maxY = this.getMaxY(puntosBzier);
            float minX = this.GetMinX(puntosBzier);
            float minY = this.GetMinY(puntosBzier);

            this.TrasladarPuntosAlOrigen(puntosBzier, minX, minY);
            this.EscalarPuntos(puntosBzier, maxX, maxY, minX, minY, mundoOcamara);
            this.TrasladarPuntosAlComienzoDelMundo(puntosBzier, mundoOcamara);
        }

        private void TrasladarPuntosAlComienzoDelMundo(IList<PuntoFlotante> puntosBzier, Boolean isMundoOCamara)
        {
            double xmin; double ymin;

            if (isMundoOCamara)
            {
                xmin = X_MIN_MUNDO;
                ymin = Y_MIN_MUNDO;
            }
            else
            {
                xmin = X_MIN_CAMARA;
                ymin = Y_MIN_CAMARA;
            }


            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() + xmin);
                punto.SetYFlotante(punto.GetYFlotante() + ymin);
            }
        }

        private void EscalarPuntos(IList<PuntoFlotante> puntosBzier, float maxX, float maxY, float minX, float minY, Boolean isMundoOcamara)
        {
            double xMin; double xMax; double yMin; double yMax;

            if (isMundoOcamara)
            {
                xMin = X_MIN_MUNDO;
                xMax = X_MAX_MUNDO;
                yMin = Y_MIN_MUNDO;
                yMax = Y_MAX_MUNDO;
            }
            else
            {
                xMin = X_MIN_CAMARA;
                xMax = X_MAX_CAMARA;
                yMin = Y_MIN_CAMARA;
                yMax = Y_MAX_CAMARA;
            }


            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() * ((xMax - xMin) / (maxX - minX)));
                punto.SetYFlotante(punto.GetYFlotante() * ((yMax - yMin) / (maxY - minY)));
            }
        }

        private float GetMinY(IList<PuntoFlotante> puntosBzier)
        {
            float minY = float.MaxValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetYFlotante() < minY) minY = (float)punto.GetYFlotante();
            }

            return minY;
        }

        private float GetMinX(IList<PuntoFlotante> puntosBzier)
        {
            float minX = float.MaxValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetXFlotante() < minX) minX = (float)punto.GetXFlotante();
            }

            return minX;
        }

        private float getMaxY(IList<PuntoFlotante> puntosBzier)
        {
            float maxY = float.MinValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetYFlotante() > maxY) maxY = (float)punto.GetYFlotante();
            }

            return maxY;
        }

        private float GetMaxX(IList<PuntoFlotante> puntosBzier)
        {
            float maxX = 0;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetXFlotante() > maxX) maxX = (float)punto.GetXFlotante();
            }

            return maxX;
        }

        private void TrasladarPuntosAlOrigen(IList<PuntoFlotante> puntosBzier, float minX, float minY)
        {
            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() - minX);
                punto.SetYFlotante(punto.GetYFlotante() - minY);
            }
        }

        internal float[] ConvertirPuntos(IList puntosBzierEscalados)
        {
            // algo así para pasar de la lista al vector

            int i;
            int cantidadPuntos = puntosBzierEscalados.Count;
            float[] default_curve = new float[cantidadPuntos * 2];

            for (i = 0; i < cantidadPuntos; i++)
            {
                default_curve[i * 2 + 0] = (float)((PuntoFlotante)puntosBzierEscalados[i]).GetXFlotante();
                default_curve[i * 2 + 1] = (float)((PuntoFlotante)puntosBzierEscalados[i]).GetYFlotante();
            }

            return default_curve;
        }



        #endregion

        // TODO: obtener de acá los puntos que hernan sacó de la pantallita de la derecha
        internal IList<PuntoFlotante> GetPuntosBzier()
        {
            IList<PuntoFlotante> puntos = new List<PuntoFlotante>();

            // Pueden estar en cualquier sistema de coordenadas
            puntos.Add(new PuntoFlotante(200, 300));
            puntos.Add(new PuntoFlotante(300, 100));
            puntos.Add(new PuntoFlotante(350, 150));
            puntos.Add(new PuntoFlotante(400, 200));
            puntos.Add(new PuntoFlotante(450, 300));
            puntos.Add(new PuntoFlotante(500, 250));
            //puntos.Add(new PuntoFlotante(550, 100));
            //puntos.Add(new PuntoFlotante(600, 200));

            return puntos;
        }

        // TODO: obtener de acá los puntos que hernán sacó de la pantallita de la izquierda
        internal IList<PuntoFlotante> GetPuntosBspline()
        {
            IList<PuntoFlotante> puntos = new List<PuntoFlotante>();

            // Pueden estar en cualquier sistema de coordenadas
            puntos.Add(new PuntoFlotante(1, 1));
            puntos.Add(new PuntoFlotante(1.5, 2));
            puntos.Add(new PuntoFlotante(1, 4));
            puntos.Add(new PuntoFlotante(4, 4));
            puntos.Add(new PuntoFlotante(4, 1));

            return puntos;
        }

        public void DibujarRueda()
        {
            Glu.GLUquadric quad = Glu.gluNewQuadric();

            Gl.glPushMatrix();

            Gl.glTranslated(0, escena.Rueda.Centro.X, escena.Rueda.Centro.Y);
            Gl.glRotated(escena.Rueda.AnguloRotacion, 1, 0, 0);

            Gl.glRotated(90, 0, 1, 0);

            Glu.gluDisk(quad, 0.6, 1, 20, 20);

            Glu.gluCylinder(quad, 1, 1, 1, 20, 20);
            Glu.gluCylinder(quad, 0.6, 0.6, 1, 20, 20);

            Gl.glTranslated(0, 0, 1);

            Glu.gluDisk(quad, 0.6, 1, 20, 20);

            Gl.glPopMatrix();

            Glu.gluDeleteQuadric(quad);
        }

        internal IList GetPuntosCurvaBzier()
        {
            // Se obtienen los puntos seleccionados por el usuario
            IList<PuntoFlotante> puntosPoligonoControlBzier = this.GetPuntosBzier();

            // Se escalan los puntos a las coordenadas de mundo, para poder controlar el paso de discretisación
            this.EscalarPuntosVentanitas(puntosPoligonoControlBzier, true);

            // Se crea la curva
            CurvaBzierSegmentosCubicos curva = new CurvaBzierSegmentosCubicos(puntosPoligonoControlBzier);

            // Se obtienen los puntos discretos de la curva
            IList puntosBzier = (IList)curva.GetPuntosDiscretos(0.01);

            return puntosBzier;
        }
    }
}
