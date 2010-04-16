using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using SistemasGraficos.Entidades;

namespace SistemasGraficos.EstrategiasDibujo
{
    public static class DDA
    {
        public static void DibujarLinea(Linea linea)
        {
            double xInicio = linea.Inicio.GetXFlotante();
            double xFin = linea.Fin.GetXFlotante();
            double yInicio = linea.Inicio.GetYFlotante();
            double yFin = linea.Fin.GetYFlotante();

            double pendiente = linea.GetPendiente();

            double pasoY = 1;
            double pasoX = 1;

            if (pendiente == 0) pasoY = 0;
            else if (double.IsInfinity(pendiente)) pasoX = 0;
            else if (Math.Abs(yFin - yInicio) > Math.Abs(xFin - xInicio)) pasoX = Math.Abs(1/pendiente);
            else pasoY = Math.Abs(pendiente);

            if (yFin < yInicio) pasoY = -pasoY;
            if (xFin < xInicio) pasoX = -pasoX;

            double xIteracion = xInicio;
            double yIteracion = yInicio;

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
