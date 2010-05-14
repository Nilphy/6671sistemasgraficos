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

        public PuntoFlotante(double x, double y, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public PuntoFlotante(double x, double y)
        {
            this.X = x;
            this.Y = y;
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

        /// <summary>
        /// No modifica el valor de this... devuelve un nuevo punto flotante con el valor multiplicado
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        public PuntoFlotante MultiplicarEscalar(double p)
        {
            return new PuntoFlotante((double)(this.GetXFlotante() * p), (double)(this.GetYFlotante() *p));
        }
        
        /// <summary>
        /// No modifica el valor de this, ni del otro, devuelve un nuevo punto flotante con el valor de la suma de ambos
        /// </summary>
        /// <param name="punto"></param>
        /// <returns></returns>
        public PuntoFlotante SumarPunto(PuntoFlotante punto)
        {
            return new PuntoFlotante(this.GetXFlotante() + punto.GetXFlotante(), this.GetYFlotante() + punto.GetYFlotante());
        }

        internal void SetXFlotante(double p)
        {
            this.x = p;
        }

        internal void SetYFlotante(double p)
        {
            this.y = p;
        }
    }
}
