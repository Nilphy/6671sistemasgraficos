using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using SistemasGraficos.Entidades;
using System.Collections;
using Modelo;

namespace SistemasGraficos.EstrategiasDibujo
{
    public static class Pintar
    {
        public static void RellenarPoligonoScanLine(IList vertices, ColorRGB color)
        {
            //Algoritmo scanline para relleno de polígonos
            Dictionary<int, ArrayList> segmentos = new Dictionary<int, ArrayList>();
            double dySegmentoActual;
            double dySegmentoAnterior;
            Punto puntoInicial;
            Punto puntoAnterior;
            if (vertices.Count < 3) return;

            puntoAnterior = (Punto)vertices[vertices.Count - 2];
            puntoInicial = (Punto)vertices[vertices.Count - 1];
            foreach (Punto puntoFinal in vertices)
            {
                dySegmentoActual = puntoFinal.GetYEntero() - puntoInicial.GetYEntero();
                dySegmentoAnterior = puntoInicial.GetYEntero() - puntoAnterior.GetYEntero();

                if (dySegmentoActual != 0)
                {
                    //agrego el vértice una vez
                    agregarPunto(segmentos, puntoInicial.GetXEntero(), puntoInicial.GetYEntero());
                    if (dySegmentoAnterior < 0 && dySegmentoActual > 0 || dySegmentoAnterior > 0 && dySegmentoActual < 0)
                    {
                        //tengo un máximo local, agrego el punto 2 veces
                        agregarPunto(segmentos, puntoInicial.GetXEntero(), puntoInicial.GetYEntero());
                    }
                    procesarSegmento(puntoInicial, puntoFinal, segmentos);
                }
                puntoAnterior = puntoInicial;
                puntoInicial = puntoFinal;
            }

            /*for (int i = 0; i < vertices.Count; i++)
            {
                dySegmentoActual = vertices[(i + 1) % vertices.Count].GetYEntero() - vertices[i].GetYEntero();
                dySegmentoAnterior = vertices[i].GetYEntero() - vertices[(i - 1 + vertices.Count) % vertices.Count].GetYEntero();
                if (dySegmentoActual != 0)
                {
                    //agrego el vértice una vez
                    agregarPunto(segmentos, vertices[i].GetXEntero(), vertices[i].GetYEntero());
                    if (dySegmentoAnterior < 0 && dySegmentoActual > 0 || dySegmentoAnterior > 0 && dySegmentoActual < 0)
                    {
                        //tengo un máximo local, agrego el punto 2 veces
                        agregarPunto(segmentos, vertices[i].GetXEntero(), vertices[i].GetYEntero());
                    }
                    procesarSegmento(vertices[i], vertices[(i + 1) % vertices.Count], segmentos);
                }
            }*/

            foreach (int y in segmentos.Keys)
            {
                segmentos[y].Sort();
                for (int i = 0; i < segmentos[y].Count; i += 2)
                {
                    pintar((int)segmentos[y][i], (int)segmentos[y][(i + 1) % segmentos[y].Count], y, color);
                }
            }
        }
        
        #region Métodos auxiliares para rellenado
        


        private static void pintar(int xInicial, int xFinal, int y, ColorRGB color)
        {
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3f(color.Red, color.Green, color.Blue);
            //desde xInicial+1 para no pintar el borde
            int x = xInicial + 1;
            //hasta xFinal-1 para no pintar el borde
            while (x < xFinal)
            {
                Gl.glVertex2i(x, y);
                x++;
            }
            Gl.glEnd();
        }

        private static void agregarPunto(Dictionary<int, ArrayList> segmentos, int x, int y)
        {
            if (!segmentos.ContainsKey(y))
            {
                segmentos.Add(y, new ArrayList());
            }
            segmentos[y].Add(x);
        }
        
        private static void procesarSegmento(Punto inicial, Punto final, Dictionary<int, ArrayList> segmentos)
        {
            int dy = final.GetYEntero() - inicial.GetYEntero();
            int dx = final.GetXEntero() - inicial.GetXEntero();

            int stepX = 1;
            int stepY = 1;

            if (dx < 0)
            {
                //el x final es menor al inicial, entonces doy vuelta el punto.
                //esto lo logro invirtiendo el orden de recorrida de los puntos
                //y por lo tanto doy vuelta el dx
                stepX = -1;
                dx = -dx;
            }

            if (dy < 0)
            {
                //el y final es menor al inicial, entonces doy vuelta el punto.
                //esto lo logro invirtiendo el orden de recorrida de los puntos
                //y por lo tanto doy vuelta el dy
                stepY = -1;
                dy = -dy;
            }

            int x = inicial.GetXEntero();
            int y = inicial.GetYEntero();

            if (Math.Abs(dy) < Math.Abs(dx))
            {
                int error = 2 * dy - dx;
                int errorE = 2 * dy;
                int errorD = 2 * (dy - dx);

                bool sentidoCreciente = (dx == 1);
                int minX = x;
                int maxX = x;
                while (x != (final.GetXEntero() + stepX))
                {
                    minX = x;
                    maxX = x;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;

                    x += stepX;
                    if (error < 0)
                    {
                        error += errorE;
                    }
                    else
                    {
                        if (sentidoCreciente)
                        {
                            agregarPunto(segmentos, maxX, y);
                        }
                        else
                        {
                            agregarPunto(segmentos, minX, y);
                        }
                        error += errorD;
                        y += stepY;
                    }
                }

                if (segmentos.ContainsKey(inicial.GetYEntero()) && segmentos[inicial.GetYEntero()].Contains(inicial.GetXEntero()))
                {
                    segmentos[inicial.GetYEntero()].RemoveAt(segmentos[inicial.GetYEntero()].LastIndexOf(inicial.GetXEntero()));
                }
                if (segmentos.ContainsKey(final.GetYEntero()) && segmentos[final.GetYEntero()].Contains(final.GetXEntero()))
                {
                    segmentos[final.GetYEntero()].RemoveAt(segmentos[final.GetYEntero()].LastIndexOf(final.GetXEntero()));
                }
            }
            else
            {
                int error = 2 * dx - dy;
                int errorE = 2 * dx;
                int errorD = 2 * (dx - dy);

                while (y != (final.GetYEntero() + stepY))
                {
                    agregarPunto(segmentos, x, y);
                    y += stepY;
                    if (error < 0)
                    {
                        error += errorE;
                    }
                    else
                    {
                        error += errorD;
                        x += stepX;
                    }
                }
                segmentos[inicial.GetYEntero()].RemoveAt(segmentos[inicial.GetYEntero()].LastIndexOf(inicial.GetXEntero()));
                segmentos[final.GetYEntero()].RemoveAt(segmentos[final.GetYEntero()].LastIndexOf(final.GetXEntero()));
            }
        }
        
        #endregion
    }
}
