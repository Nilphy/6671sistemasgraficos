using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class PuntoFlotante : Punto
    {
        public double x;
        private double y;
        private double z;
        
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

        public double Z
        {
            set { this.z = value; }
            get { return this.z; }
        }

        public PuntoFlotante(double x, double y, double z, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public PuntoFlotante(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
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

        #region Operadores

        public static PuntoFlotante operator *(double escalar, PuntoFlotante punto)
        {
            punto.X *= escalar;
            punto.Y *= escalar;
            punto.Z *= escalar;
            return punto;
        }
        
        public static PuntoFlotante operator *(PuntoFlotante punto, double escalar)
        {
            punto.X *= escalar;
            punto.Y *= escalar;
            punto.Z *= escalar;
            return punto;
        }

        public static PuntoFlotante operator +(PuntoFlotante punto1, PuntoFlotante punto2)
        {
            return new PuntoFlotante(punto1.X + punto2.X, punto1.Y + punto2.Y, punto1.Z + punto2.Z);
        }

        public static PuntoFlotante operator -(PuntoFlotante punto1, PuntoFlotante punto2)
        {
            return new PuntoFlotante(punto1.X - punto2.X, punto1.Y - punto2.Y, punto1.Z - punto2.Z);
        }

        #endregion

        /// <summary>
        /// No modifica el valor de this... devuelve un nuevo punto flotante con el valor multiplicado
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        public PuntoFlotante MultiplicarEscalar(double p)
        {
            return new PuntoFlotante((double)(this.GetXFlotante() * p), (double)(this.GetYFlotante() *p), (double)(this.Z *p));
        }
        
        /// <summary>
        /// No modifica el valor de this, ni del otro, devuelve un nuevo punto flotante con el valor de la suma de ambos
        /// </summary>
        /// <param name="punto"></param>
        /// <returns></returns>
        public PuntoFlotante SumarPunto(PuntoFlotante punto)
        {
            return new PuntoFlotante(this.GetXFlotante() + punto.GetXFlotante(), this.GetYFlotante() + punto.GetYFlotante(), this.Z + punto.Z);
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
