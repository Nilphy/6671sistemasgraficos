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
        public double X_MIN = -10;
        public double X_MAX = 10;
        public double Y_MIN = -5;
        public double Y_MAX = 5;

        public double X_MIN_ZOOM = 0;
        public double X_MAX_ZOOM = 1;
        public double Y_MIN_ZOOM = 0;
        public double Y_MAX_ZOOM = 1;

        public IList PoligonosTerreno { set; get; }
        public IList PoligonosRueda { set; get; }

        public Vista()
        {
            this.PoligonosTerreno = new ArrayList();
            this.PoligonosRueda = new ArrayList();
        }

        public void DibujarEscena(Escena escena)
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            CrearPoligonosTerreno(escena.Terreno);
            CrearPoligonosRueda(escena.Rueda);

            Gl.glPushMatrix();

            this.EscalarEscenaToViewSceneWindow();

            DibujarTerreno(escena.Terreno, false, null);
            DibujarRueda(escena.Rueda);

            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void DibujarTerreno(Terreno terreno, Boolean aplicarClipping, ViewPort viewPort)
        {
            foreach (Poligono poligono in PoligonosTerreno)
            {
                IList puntosDibujo;

                if (aplicarClipping) puntosDibujo = Clipping.RecortarPoligono(poligono.Puntos, viewPort);
                else puntosDibujo = poligono.Puntos;

                // Se rellena el polígono
                Pintar.RellenarPoligonoScanLine(puntosDibujo, poligono.ColorRelleno);

                Gl.glColor3f(poligono.ColorLinea.Red, poligono.ColorLinea.Green, poligono.ColorLinea.Blue);

                // Todos los puntos van a ser unidos por segmentos y el último se une al primero
                Gl.glBegin(Gl.GL_LINE_LOOP);

                foreach (Punto punto in puntosDibujo)
                {
                    Gl.glVertex2d(punto.GetXFlotante(), punto.GetYFlotante());
                }

                Gl.glEnd();
            }
        }

        private void CrearPoligonosTerreno(Terreno terreno)
        {
            // Creo los poligonos del terreno una única vez.
            if (PoligonosTerreno.Count == 0)
            {
                Vertice verticeAnterior = null;

                foreach (Vertice vertice in terreno.Vertices)
                {
                    Gl.glPushMatrix();

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

                    Gl.glPopMatrix();
                }
            }
        }

        private void DibujarRueda(Rueda rueda)
        {
            foreach (Poligono poligono in PoligonosRueda)
            {
                Gl.glPushMatrix();

                Gl.glTranslated(rueda.Centro.X, rueda.Centro.Y, 0);
                Gl.glRotated(rueda.AnguloRotacion * 180 / Math.PI, 0, 0, 1);
                Gl.glTranslated(-rueda.Centro.X, -rueda.Centro.Y, 0);

                Gl.glColor3f(poligono.ColorLinea.Red, poligono.ColorLinea.Green, poligono.ColorLinea.Blue);

                // Todos los puntos van a ser unidos por segmentos y el último se une al primero
                Gl.glBegin(Gl.GL_LINE_LOOP);

                foreach (Punto punto in poligono.Puntos)
                {
                    Gl.glVertex2d(punto.GetXFlotante(), punto.GetYFlotante());
                }

                Gl.glEnd();

                // Se rellena el polígono
                Pintar.RellenarPoligonoScanLine(poligono.Puntos, poligono.ColorRelleno);

                Gl.glPopMatrix();
            }
        }

        private void CrearPoligonosRueda(Rueda rueda)
        {
            PoligonosRueda.Clear();

            // primero agrego la rueda externa
            Poligono ruedaExterna = new Poligono();
            ruedaExterna.ColorLinea = new ColorRGB(1, 0, 0);
            ruedaExterna.ColorRelleno = new ColorRGB(1, 0, 0);
            ruedaExterna.Puntos = Bresenham.ObtenerPuntosEquidistantesCirculo(
                new Circulo(new PuntoFlotante(rueda.Centro.X, rueda.Centro.Y), rueda.RadioExterno), 8);

            PoligonosRueda.Add(ruedaExterna);

            // agrego a rueda interna
            Poligono ruedaInterna = new Poligono();
            ruedaInterna.ColorLinea = new ColorRGB(1, 1, 1);
            ruedaInterna.ColorRelleno = new ColorRGB(1, 1, 1);
            ruedaInterna.Puntos = Bresenham.ObtenerPuntosEquidistantesCirculo(
                new Circulo(new PuntoFlotante(rueda.Centro.X, rueda.Centro.Y), rueda.RadioInterno), 8);

            PoligonosRueda.Add(ruedaInterna);

            // agrego los triangulos
            Poligono triangulo1 = new Poligono();
            triangulo1.ColorLinea = new ColorRGB(0, 0, 0);
            triangulo1.ColorRelleno = new ColorRGB(0, 0, 0);
            triangulo1.Puntos.Add(new PuntoFlotante(rueda.Centro.X, rueda.Centro.Y));
            triangulo1.Puntos.Add(ruedaInterna.Puntos[0]);
            triangulo1.Puntos.Add(ruedaInterna.Puntos[1]);
            triangulo1.Puntos.Add(ruedaInterna.Puntos[2]);

            PoligonosRueda.Add(triangulo1);

            Poligono triangulo2 = new Poligono();
            triangulo2.ColorLinea = new ColorRGB(0, 0, 0);
            triangulo2.ColorRelleno = new ColorRGB(0, 0, 0);
            triangulo2.Puntos.Add(new PuntoFlotante(rueda.Centro.X, rueda.Centro.Y));
            triangulo2.Puntos.Add(ruedaInterna.Puntos[4]);
            triangulo2.Puntos.Add(ruedaInterna.Puntos[5]);
            triangulo2.Puntos.Add(ruedaInterna.Puntos[6]);

            PoligonosRueda.Add(triangulo2);
        }

        private void EscalarEscenaToViewSceneWindow()
        {
            Gl.glTranslated(X_MIN, Y_MIN, 0);
            Gl.glScaled((X_MAX - X_MIN) / (Escena.X_MAX - Escena.X_MIN), (Y_MAX - Y_MIN) / (Escena.Y_MAX - Escena.Y_MIN), 1);
            Gl.glTranslated(-Escena.X_MIN, -Escena.Y_MIN, 0);
        }

        private void AplicarClipping(Rueda rueda)
        {
            foreach (Poligono poligono in PoligonosTerreno)
            {
                poligono.Puntos = Clipping.RecortarPoligono(poligono.Puntos, new ViewPort(rueda));
            }

            foreach (Poligono poligono in PoligonosRueda)
            {
                poligono.Puntos = Clipping.RecortarPoligono(poligono.Puntos, new ViewPort(rueda));
            }
        }


        internal void DibujarZoomEscena(Escena escena)
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            CrearPoligonosTerreno(escena.Terreno);
            CrearPoligonosRueda(escena.Rueda);

            Gl.glPushMatrix();

            ViewPort vp = new ViewPort(escena.Rueda);

            this.EscalarEscenaToViewCameraWindow(vp);

            DibujarTerreno(escena.Terreno, true, vp);
            DibujarRueda(escena.Rueda);

            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void EscalarEscenaToViewCameraWindow(ViewPort viewPort)
        {
            Gl.glTranslated(X_MIN_ZOOM, Y_MIN_ZOOM, 0);
            Gl.glScaled((X_MAX_ZOOM - X_MIN_ZOOM) / (viewPort.XDer - viewPort.XIzq), (Y_MAX_ZOOM - Y_MIN_ZOOM) / (viewPort.YArriba - viewPort.YAbajo), 1);
            Gl.glTranslated(-viewPort.XIzq, -viewPort.YAbajo, 0);
        }
    }
}
