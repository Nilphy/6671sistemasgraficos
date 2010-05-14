using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Tao.OpenGl;

using Modelo;
using SistemasGraficos.EstrategiasDibujo;

namespace SistemasGraficos.Entidades
{
    public class Vista
    {
        // Para los calculos y los límites de las cosas
        public double X_MIN_MUNDO = 0;
        public double X_MAX_MUNDO = 100;
        public double Y_MIN_MUNDO = 0;
        public double Y_MAX_MUNDO = 100;

        // Creo que es 20 pero con 15 seguro queda mejor :S
        public double X_MIN_VIEWPORT_ESCENA3D = -20;
        public double X_MAX_VIEWPORT_ESCENA3D = 20;
        public double Y_MIN_VIEWPORT_ESCENA3D = -20;
        public double Y_MAX_VIEWPORT_ESCENA3D = 20;

        public static int W_WIDTH = 1024;
        public static int W_HEIGHT = 768;

        public static int TOP_VIEW_POSX = (int)((float)W_WIDTH * 0.70f);
        public static int TOP_VIEW_POSY = (int)((float)W_HEIGHT * 0.70f);
        public static int TOP_VIEW_W = (int)((float)W_WIDTH * 0.30f);
        public static int TOP_VIEW_H = (int)((float)W_HEIGHT * 0.30f);

        public static int HEIGHT_VIEW_POSX = (int)((float)W_WIDTH * 0.00f);
        public static int HEIGHT_VIEW_POSY = (int)((float)W_HEIGHT * 0.70f);
        public static int HEIGHT_VIEW_W = (int)((float)W_WIDTH * 0.30f);
        public static int HEIGHT_VIEW_H = (int)((float)W_HEIGHT * 0.30f);

        //public static int CURVE_POINTS = 36;

        public IList PoligonosTerreno { set; get; }
        public IList PoligonosRueda { set; get; }

        public Vista()
        {
            this.PoligonosTerreno = new ArrayList();
            this.PoligonosRueda = new ArrayList();
        }

        private void CrearPoligonosTerreno(Terreno terreno)
        {
            if (PoligonosTerreno.Count == 0)
            {
                Vertice verticeAnterior = null;

                foreach (Vertice vertice in terreno.Vertices)
                {
                    if (verticeAnterior != null)
                    {
                        Poligono poligono = new Poligono();

                        poligono.ColorLinea = new ColorRGB(0.4f, 0.4f, 0.4f);
                        poligono.ColorRelleno = new ColorRGB(0.4f, 0.4f, 0.4f);

                        poligono.Puntos.Add(new PuntoFlotante(verticeAnterior.X, 0));
                        poligono.Puntos.Add(new PuntoFlotante(verticeAnterior.X, verticeAnterior.Y));
                        poligono.Puntos.Add(new PuntoFlotante(vertice.X, vertice.Y));
                        poligono.Puntos.Add(new PuntoFlotante(vertice.X, 0));

                        this.PoligonosTerreno.Add(poligono);
                    }

                    verticeAnterior = vertice;
                }
            }
        }

        public void EscalarMundoToEscena3D()
        {
            Gl.glTranslated(X_MIN_VIEWPORT_ESCENA3D, Y_MIN_VIEWPORT_ESCENA3D, 0);
            Gl.glScaled((X_MAX_VIEWPORT_ESCENA3D - X_MIN_VIEWPORT_ESCENA3D) / (X_MAX_MUNDO - X_MIN_MUNDO), (Y_MAX_VIEWPORT_ESCENA3D - Y_MIN_VIEWPORT_ESCENA3D) / (Y_MAX_MUNDO - Y_MIN_MUNDO), 1);
            Gl.glTranslated(-X_MIN_MUNDO, -Y_MIN_MUNDO, 0);
        }

        public void EscalarPuntosBzier(IList<PuntoFlotante> puntosBzier)
        {
            // Obtengo los límites de los puntos y los escalo a los límites del mundo

            float maxX = this.GetMaxX(puntosBzier);
            float maxY = this.getMaxY(puntosBzier);
            float minX = this.GetMinX(puntosBzier);
            float minY = this.GetMinY(puntosBzier);

            this.TrasladarPuntosAlOrigen(puntosBzier, minX, minY);
            this.EscalarPuntos(puntosBzier, maxX, maxY, minX, minY);
            this.TrasladarPuntosAlComienzoDelMundo(puntosBzier);
        }

        private void TrasladarPuntosAlComienzoDelMundo(IList<PuntoFlotante> puntosBzier)
        {
            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() + X_MIN_MUNDO);
                punto.SetYFlotante(punto.GetYFlotante() + Y_MIN_MUNDO);
            }
        }

        private void EscalarPuntos(IList<PuntoFlotante> puntosBzier, float maxX, float maxY, float minX, float minY)
        {
            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() * ((X_MAX_MUNDO - X_MIN_MUNDO) / (maxX - minX)));
                punto.SetYFlotante(punto.GetYFlotante() * ((Y_MAX_MUNDO - Y_MIN_MUNDO) / (maxY - minY)));
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

        // TODO: obtener de acá los puntos que hernan sacó de la pantallita de la derecha
        internal IList<PuntoFlotante> GetPuntosBzier()
        {
            IList<PuntoFlotante> puntos = new List<PuntoFlotante>();

            // Pueden estar en cualquier sistema de coordenadas
            puntos.Add(new PuntoFlotante(200, 300));
            puntos.Add(new PuntoFlotante(300, 100));
            puntos.Add(new PuntoFlotante(350, 100));
            puntos.Add(new PuntoFlotante(400, 300));
            puntos.Add(new PuntoFlotante(450, 500));
            puntos.Add(new PuntoFlotante(500, 300));
            puntos.Add(new PuntoFlotante(550, 100));
            puntos.Add(new PuntoFlotante(600, 200));

            return puntos;
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
    }
}
