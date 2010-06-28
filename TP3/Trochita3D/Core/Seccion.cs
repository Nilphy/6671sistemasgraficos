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
        public double Angulo { get; set; }

        public Seccion()
        {
            this.Angulo = 0;
            this.Vertices = new List<PuntoFlotante>();
        }

        public Seccion(IList<PuntoFlotante> puntos)
        {
            this.Angulo = 0;
            this.Vertices = new List<PuntoFlotante>();

            foreach (PuntoFlotante punto in puntos)
            {
                Vertices.Add(new PuntoFlotante(punto.X, punto.Y, punto.Z));
            }
        }

        public void Rotar(double angulo)
        {
            this.Angulo = ((this.Angulo + angulo) % (2 * Math.PI));
            foreach (PuntoFlotante punto in Vertices)
            {
                double x = punto.X;
                double y = punto.Y;
                punto.X = (x * Math.Cos(angulo)) - (y * Math.Sin(angulo));
                punto.Y = (x * Math.Sin(angulo)) + (y * Math.Cos(angulo));
            }
        }

        /// <summary>
        /// Reordena los vertices para que el primer vertice sea el mas próximo al origen.
        /// </summary>
        public void ReordenarVertices()
        {
            // Calculo las normas de los vectores extremo de la superficie
            double norm1 = Math.Sqrt(Math.Pow(Vertices[0].X, 2) + (Math.Pow(Vertices[0].Y, 2)));
            double norm2 = Math.Sqrt(Math.Pow(Vertices[Vertices.Count - 1].X, 2) + (Math.Pow(Vertices[Vertices.Count - 1].Y, 2)));

            if (norm2 < norm1)
                Vertices = Vertices.Reverse<PuntoFlotante>().ToList<PuntoFlotante>();
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

        public void InvertirVertices()
        {
            Vertices = Vertices.Reverse<PuntoFlotante>().ToList<PuntoFlotante>();
        }
    }
}
