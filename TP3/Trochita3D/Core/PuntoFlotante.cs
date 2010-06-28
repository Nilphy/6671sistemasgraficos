using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class PuntoFlotante : Punto
    {
        private double x;
        private double y;
        private double z;

        private double normalX;
        private double normalY;
        private double normalZ;
        
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


        public double NormalX
        {
            set { this.normalX = value; }
            get { return this.normalX; }
        }
        public double NormalY
        {
            set { this.normalY = value; }
            get { return this.normalY; }
        }

        public double NormalZ
        {
            set { this.normalZ = value; }
            get { return this.normalZ; }
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

        public PuntoFlotante(PuntoFlotante punto)
        {
            this.X = punto.X;
            this.Y = punto.Y;
            this.Z = punto.Z;
            this.NormalX = punto.NormalX;
            this.NormalY = punto.NormalY;
            this.NormalZ = punto.NormalZ;
            this.OrigenCoordenadas = punto.OrigenCoordenadas;
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

        // Producto vectorial, normalizado
        public static PuntoFlotante operator *(PuntoFlotante punto1, PuntoFlotante punto2)
        {
            PuntoFlotante punto = new PuntoFlotante(0, 0, 0);

            punto.X = punto1.Y * punto2.Z - punto1.Z * punto2.Y;
            punto.Y = punto1.Z * punto2.X - punto1.X * punto2.Z;
            punto.Z = punto1.X * punto2.Y - punto1.Y * punto2.X;

            double modulo = punto.Modulo();
            if (modulo != 0)
            {
                punto.X = punto.X / modulo;
                punto.Y = punto.Y / modulo;
                punto.Z = punto.Z / modulo;
            }

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
            if (punto1 == null) return punto2;
            if (punto2 == null) return punto1;
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

        public void Normalizar()
        {
            double modulo = this.Modulo();

            this.X /= modulo;
            this.Y /= modulo;
            this.Z /= modulo;
        }

        public double Modulo()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
        }

        public void ZPositivo()
        {
            if (this.Z < 0)
            {
                this.X *= -1;
                this.Y *= -1;
                this.Z *= -1;
            }
        }

        internal void SetXFlotante(double p)
        {
            this.x = p;
        }

        internal void SetYFlotante(double p)
        {
            this.y = p;
        }

        public double ProductoEscalar(PuntoFlotante punto)
        {
            return (this.X * punto.X) + (this.Y * punto.Y) + (this.Z * punto.Z);
        }
    }
}
