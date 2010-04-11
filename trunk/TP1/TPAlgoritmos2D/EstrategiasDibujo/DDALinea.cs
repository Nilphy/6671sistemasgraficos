using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using SistemasGraficos.Entidades;

namespace SistemasGraficos.EstrategiasDibujo
{
    public static class DDALinea
    {
        public static void DibujarLinea(Linea linea)
        {
            float xInicio = linea.Inicio.GetXFlotante();
            float xFin = linea.Fin.GetXFlotante();
            float yInicio = linea.Inicio.GetYFlotante();
            float yFin = linea.Fin.GetYFlotante();

            float pendiente = linea.GetPendiente();

            float pasoY = 1;
            float pasoX = 1;

            if (pendiente == 0) pasoY = 0;
            else if (float.IsInfinity(pendiente)) pasoX = 0;
            else if (Math.Abs(yFin - yInicio) > Math.Abs(xFin - xInicio)) pasoX = Math.Abs(1/pendiente);
            else pasoY = Math.Abs(pendiente);

            if (yFin < yInicio) pasoY = -pasoY;
            if (xFin < xInicio) pasoX = -pasoX;

            float xIteracion = xInicio;
            float yIteracion = yInicio;

            while (((xIteracion >= xInicio && xIteracion <= xFin) ||
                (xIteracion <= xInicio && xIteracion >= xFin)) &&
                ((yIteracion >= yInicio && yIteracion <= yFin) ||
                (yIteracion <= yInicio && yIteracion >= xFin)))
            {
                Gl.glVertex2d(xIteracion, yIteracion);

                xIteracion = xIteracion + pasoX;
                yIteracion = yIteracion + pasoY;
            }
        }
    }
}
