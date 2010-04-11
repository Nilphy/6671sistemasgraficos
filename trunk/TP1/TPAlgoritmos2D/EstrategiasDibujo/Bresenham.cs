using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SistemasGraficos.Entidades;
using Tao.OpenGl;
using System.Collections;
using Common.Utils;

namespace SistemasGraficos.EstrategiasDibujo
{
    public static class Bresenham
    {
        public static void DibujarLinea(Linea linea)
        {
            int xInicio = linea.Inicio.GetXEntero();
            int xFin = linea.Fin.GetXEntero();
            int yInicio = linea.Inicio.GetYEntero();
            int yFin = linea.Fin.GetYEntero();

            int xIteracion, yIteracion, deltaX, deltaY, parametroDecision, incE, incNE, pasoX, pasoY;

            deltaX = xFin - xInicio;
            deltaY = yFin - yInicio;

            // Se determina que punto se usa para empezar y que punto para terminar
            if (deltaY < 0)
            {
                deltaY = -deltaY;
                pasoY = -1;
            }
            else pasoY = 1;

            if (deltaX < 0)
            {
                deltaX = -deltaX;
                pasoX = -1;
            }
            else pasoX = 1;

            xIteracion = xInicio;
            yIteracion = yInicio;

            Gl.glVertex2d(xIteracion, yIteracion);

            // Se cicla hasta llegar al extremo de la línea
            if (deltaX > deltaY)
            {
                parametroDecision = 2 * deltaY - deltaX;
                incE = 2 * deltaY;
                incNE = 2 * (deltaY - deltaX);
                while (!(xIteracion == xFin || yIteracion == yFin))
                {
                    xIteracion = xIteracion + pasoX;
                    if (parametroDecision < 0) parametroDecision = parametroDecision + incE;
                    else
                    {
                        yIteracion = yIteracion + pasoY;
                        parametroDecision = parametroDecision + incNE;
                    }

                    Gl.glVertex2d(xIteracion, yIteracion);
                }
            }
            else
            {
                parametroDecision = 2 * deltaX - deltaY;
                incE = 2 * deltaX;
                incNE = 2 * (deltaX - deltaY);
                while (yIteracion != yFin)
                {
                    yIteracion = yIteracion + pasoY;
                    if (parametroDecision < 0)
                    {
                        parametroDecision = parametroDecision + incE;
                    }
                    else
                    {
                        xIteracion = xIteracion + pasoX;
                        parametroDecision = parametroDecision + incNE;
                    }
                    Gl.glVertex2d(xIteracion, yIteracion);
                }
            }
        }

        public static void DibujarCirculo(Circulo circulo)
        {
            int x0 = circulo.Centro.GetXEntero();
            int y0 = circulo.Centro.GetYEntero();
            int radius = circulo.Radio;

            int f = 1 - radius;
            int ddF_x = 1;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            Gl.glVertex2d(x0, y0 + radius);
            Gl.glVertex2d(x0, y0 - radius);
            Gl.glVertex2d(x0 + radius, y0);
            Gl.glVertex2d(x0 - radius, y0);

            while (x < y)
            {
                // ddF_x == 2 * x + 1;
                // ddF_y == -2 * y;
                // f == x*x + y*y - radius*radius + 2*x - y + 1;
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;
                Gl.glVertex2d(x0 + x, y0 + y);
                Gl.glVertex2d(x0 - x, y0 + y);
                Gl.glVertex2d(x0 + x, y0 - y);
                Gl.glVertex2d(x0 - x, y0 - y);
                Gl.glVertex2d(x0 + y, y0 + x);
                Gl.glVertex2d(x0 - y, y0 + x);
                Gl.glVertex2d(x0 + y, y0 - x);
                Gl.glVertex2d(x0 - y, y0 - x);
            }
        }

        public static IList ObtenerPuntosEquidistantesCirculo(Circulo circulo, int cantidadPuntos)
        {
            if (!(cantidadPuntos % 2 == 0)) throw new InvalidOperationException("Solo se pueden obtener cantidades pares de vértices");

            // Vector donde se van a poner todos los puntos
            IList puntos = new ArrayList();

            int x0 = circulo.Centro.GetXEntero();
            int y0 = circulo.Centro.GetYEntero();
            int radius = circulo.Radio;

            int f = 1 - radius;
            int ddF_x = 1;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;
            
            puntos.Add(new PuntoEntero(x0, y0 + radius, circulo.Centro));
            puntos.Add(new PuntoEntero(x0, y0 - radius, circulo.Centro));
            puntos.Add(new PuntoEntero(x0 + radius, y0, circulo.Centro));
            puntos.Add(new PuntoEntero(x0 - radius, y0, circulo.Centro));

            while (x < y)
            {
                // ddF_x == 2 * x + 1;
                // ddF_y == -2 * y;
                // f == x*x + y*y - radius*radius + 2*x - y + 1;
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;

                puntos.Add(new PuntoEntero(x0 + x, y0 + y, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 - x, y0 + y, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 + x, y0 - y, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 - x, y0 - y, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 + y, y0 + x, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 - y, y0 + x, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 + y, y0 - x, circulo.Centro));
                puntos.Add(new PuntoEntero(x0 - y, y0 - x, circulo.Centro));
            }

            CollectionUtils.Sort(puntos, new ComparadorPuntosCirculo());

            int cantidadTotalPuntos = puntos.Count;
            int cantidadSaltoPuntos = cantidadTotalPuntos / cantidadPuntos;
            int indice;
            IList retorno = new ArrayList();

            for (int i = 0; i < cantidadPuntos; i++)
            {
                indice = i == 0 ? 0 : (i*cantidadSaltoPuntos) - 1;
                retorno.Add(puntos[indice]);
            }

            return retorno;
        }
    }
}
