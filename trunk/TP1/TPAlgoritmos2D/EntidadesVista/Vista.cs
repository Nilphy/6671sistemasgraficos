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
        public double Y_MIN = -8;
        public double Y_MAX = 8;

        public IList PoligonosTerreno { set; get; }
        public IList PoligonosRueda { set; get; }

        public Vista()
        {
            this.PoligonosTerreno = new ArrayList();
            this.PoligonosRueda = new ArrayList();
        }

        public void DibujarEscena(Escena escena)
        {
            CrearPoligonosTerreno(escena.Terreno);
            CrearPoligonosRueda(escena.Rueda);

            this.EscalarEscenaToViewSceneWindow();

            DibujarTerreno(escena.Terreno);
            DibujarRueda(escena.Rueda);
        }

        private void DibujarTerreno(Terreno terreno)
        {
            foreach (Poligono poligono in PoligonosTerreno)
            {
                Gl.glPushMatrix();

                // Se rellena el polígono
                Pintar.RellenarPoligonoScanLine(poligono.Puntos, poligono.ColorRelleno);

                Gl.glColor3f(poligono.ColorLinea.Red, poligono.ColorLinea.Green, poligono.ColorLinea.Blue);

                // Todos los puntos van a ser unidos por segmentos y el último se une al primero
                Gl.glBegin(Gl.GL_LINE_LOOP);

                foreach (Punto punto in poligono.Puntos)
                {
                    Gl.glVertex2d(punto.GetXFlotante(), punto.GetYFlotante());
                }

                Gl.glEnd();

                Gl.glPopMatrix();
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
                    if (verticeAnterior != null)
                    {
                        Poligono poligono = new Poligono();

                        poligono.ColorLinea = new ColorRGB(1, 0, 0);
                        poligono.ColorRelleno = new ColorRGB(1, 0, 0);

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

        private void DibujarRueda(Rueda rueda)
        {
            foreach (Poligono poligono in PoligonosRueda)
            {
                Gl.glPushMatrix();

                Gl.glTranslated(rueda.Centro.X, rueda.Centro.Y, 0);
                Gl.glRotated(rueda.AnguloRotacion, 0, 0, 1);
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
            Gl.glTranslated(-Escena.X_MIN, Escena.Y_MIN, 0);
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

    }
}
