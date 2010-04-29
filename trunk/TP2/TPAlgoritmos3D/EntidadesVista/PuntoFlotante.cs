using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class PuntoFlotante : Punto
    {
        private double x;
        private double y;

        public double X
        {
            set { this.x = value; }
            get { return this.x; }
        }
        public double Y
        {
            set { this.y = value; }
            get { return this.y; }
        }

        public PuntoFlotante(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public PuntoFlotante(double x, double y, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public override double GetXFlotante()
        {
            return this.X;
        }

        public override double GetYFlotante()
        {
            return this.Y;
        }

        public override int GetXEntero()
        {
            return (int)Math.Round(this.x, MidpointRounding.ToEven);
        }

        public override int GetYEntero()
        {
            return (int)Math.Round(this.y, MidpointRounding.ToEven);
        }
    }
}
