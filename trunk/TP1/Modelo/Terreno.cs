using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Terreno
    {

        public IList<Vertice> Vertices { get; set; }

        private IList<Plano> Planos { get; set; }

        public Terreno()
        {
            this.Vertices = new List<Vertice>();
            this.Planos = new List<Plano>();
        }

        public void AddVertice(double x, double y)
        {
            Vertice nuevoVertice = new Vertice(x, y);

            this.Vertices.Add(nuevoVertice);

            // Si tengo mas de un vertice, puedo empezar a formar los planos.
            if (this.Vertices.Count > 1)
            {
                this.Planos.Add(new Plano(this.Vertices.Last<Vertice>(), nuevoVertice));
            }
        }

        public double GetAltura(double x)
        {
            return GetPlanoForX(x).GetAltura(x);
        }

        public double GetAnguloInclinacion(double x)
        {
            return GetPlanoForX(x).GetAnguloInclinacion();
        }

        private Plano GetPlanoForX(double x)
        {
            foreach (Plano plano in Planos)
            {
                if (plano.ContainsX(x))
                    return plano;
            }

            throw new Exception("No existe un plano que contenga el valor para X = " + x + ". No se puede realizar la operacion.");
        }

    }

    class Plano
    {

        public Vertice VerticeInicial { get; set; }

        public Vertice VerticeFinal { get; set; }

        public Plano(Vertice verticeInicial, Vertice verticeFinal)
        {
            this.VerticeInicial = verticeInicial;
            this.VerticeFinal = verticeFinal;
        }

        public double GetPendiente()
        {
            return (VerticeFinal.Y - VerticeInicial.Y) / (VerticeFinal.X - VerticeInicial.X);
        }

        public double GetAnguloInclinacion()
        {
            return Math.Atan(GetPendiente());
        }

        public bool ContainsX(double x)
        {
            return VerticeInicial.X > x && x < VerticeFinal.X;
        }

        public double GetAltura(double x)
        {
            double b = VerticeInicial.Y - (VerticeInicial.X * GetPendiente());
            return (GetPendiente() * x) + b;
        }

    }
}
