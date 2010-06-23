using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{

    /// <summary>
    /// Representa una seccion de superficie. Sobre la misma se pueden aplicar las 
    /// distintas transformaciones.
    /// </summary>
    public class Seccion
    {

        public IList<PuntoFlotante> Vertices { get; set; }

        public Seccion()
        {
            this.Vertices = new List<PuntoFlotante>();
        }

        public Seccion(IList<PuntoFlotante> puntos)
        {
            this.Vertices = new List<PuntoFlotante>();

            foreach (PuntoFlotante punto in puntos)
            {
                Vertices.Add(new PuntoFlotante(punto.X, punto.Y, punto.Z));
            }
        }

        public void Rotar(double angulo)
        {
            foreach (PuntoFlotante punto in Vertices)
            {
                double x = punto.X;
                double y = punto.Y;
                punto.X = (x * Math.Cos(angulo)) - (y * Math.Sin(angulo));
                punto.Y = (x * Math.Sin(angulo)) + (y * Math.Cos(angulo));
            }
        }

        public void Trasladar(double dx, double dy, double dz)
        {
            foreach (PuntoFlotante punto in Vertices)
            {
                punto.X += dx;
                punto.Y += dy;
                punto.Z += dz;
            }
        }

        public void Escalar(double ex, double ey, double ez)
        {
            foreach (PuntoFlotante punto in Vertices)
            {
                punto.X *= ex;
                punto.Y *= ey;
                punto.Z *= ez;
            }
        }
    }
}
