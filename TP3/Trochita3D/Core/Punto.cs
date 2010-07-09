using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class Punto
    {
        #region Atributos y Propiedades

        private double x;
        private double y;
        private double z;
        private double normalX;
        private double normalY;
        private double normalZ;
        private Punto origenCoordenadas;
        
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
        public Punto OrigenCoordenadas
        {
            set { this.origenCoordenadas = value; }
            get { return this.origenCoordenadas; }
        }

        #endregion
        #region Constructores

        public Punto()
        {
        }

        public Punto(double x, double y, double z, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public Punto(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            Punto origen = new Punto();

            origen.X = 0;
            origen.Y = 0;
            origen.Z = 0;

            this.OrigenCoordenadas = origen;
        }

        #endregion

        public int GetCuadrante()
        {
            if ((this.X - OrigenCoordenadas.X) >= 0 &&
                (Y - OrigenCoordenadas.Y) >= 0)
                return 1;

            if ((this.X - OrigenCoordenadas.X) <= 0 &&
                (Y - OrigenCoordenadas.Y) >= 0)
                return 2;

            if ((this.X - OrigenCoordenadas.X) <= 0 &&
                (this.Y - OrigenCoordenadas.Y) <= 0)
                return 3;

            if ((this.X - OrigenCoordenadas.X) >= 0 &&
                (this.Y - OrigenCoordenadas.Y) <= 0)
                return 4;

            throw new InvalidProgramException("Está mál hecho el GetCuadrante");
        }

        #region Operadores

        public static Punto operator *(double escalar, Punto punto)
        {
            punto.X *= escalar;
            punto.Y *= escalar;
            punto.Z *= escalar;
            return punto;
        }

        // Producto vectorial, normalizado
        public static Punto operator *(Punto punto1, Punto punto2)
        {
            Punto punto = new Punto(0, 0, 0);

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
        
        public static Punto operator *(Punto punto, double escalar)
        {
            punto.X *= escalar;
            punto.Y *= escalar;
            punto.Z *= escalar;
            return punto;
        }

        public static Punto operator +(Punto punto1, Punto punto2)
        {
            if (punto1 == null) return punto2;
            if (punto2 == null) return punto1;
            return new Punto(punto1.X + punto2.X, punto1.Y + punto2.Y, punto1.Z + punto2.Z);
        }

        public static Punto operator -(Punto punto1, Punto punto2)
        {
            return new Punto(punto1.X - punto2.X, punto1.Y - punto2.Y, punto1.Z - punto2.Z);
        }

        #endregion

        /// <summary>
        /// No modifica el valor de this... devuelve un nuevo punto flotante con el valor multiplicado
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        public Punto MultiplicarEscalar(double p)
        {
            return new Punto((double)(this.X * p), (double)(this.Y *p), (double)(this.Z *p));
        }
        
        /// <summary>
        /// No modifica el valor de this, ni del otro, devuelve un nuevo punto flotante con el valor de la suma de ambos
        /// </summary>
        /// <param name="punto"></param>
        /// <returns></returns>
        public Punto SumarPunto(Punto punto)
        {
            return new Punto(this.X + punto.X, this.Y + punto.Y, this.Z + punto.Z);
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
        public double ProductoEscalar(Punto punto)
        {
            return (this.X * punto.X) + (this.Y * punto.Y) + (this.Z * punto.Z);
        }

        public Punto Clone()
        {
            Punto puntoNuevo = new Punto();

            puntoNuevo.NormalX = this.NormalX;
            puntoNuevo.NormalY = this.NormalY;
            puntoNuevo.NormalZ = this.NormalZ;
            puntoNuevo.OrigenCoordenadas = this.OrigenCoordenadas;
            puntoNuevo.X = this.X;
            puntoNuevo.Y = this.Y;
            puntoNuevo.Z = this.Z;

            return puntoNuevo;
        }

        /// <summary>
        /// Promedia las normales calculadas con todos los puntos de alrededor
        /// </summary>
        /// <param name="verticeCentro"></param>
        /// <param name="verticeNorte"></param>
        /// <param name="verticeEste"></param>
        /// <param name="verticeSur"></param>
        /// <param name="verticeOeste"></param>
        /// <returns></returns>
        public static Punto CalcularNormal(Punto verticeCentro, Punto verticeNorte, Punto verticeEste, Punto verticeSur, Punto verticeOeste, Boolean invertirSentido)
        {
            if (verticeCentro == null) throw new InvalidOperationException("Este método no puede ser invocado con el vertice central nulo");

            // Numeradas en sentido horario son 4
            Punto normalNorEste = null;
            Punto normalSurEste = null;
            Punto normalSurOeste = null;
            Punto normalNorOeste = null;
            Punto normalRetorno = null;

            if (verticeNorte != null && verticeEste != null)
            {
                if (invertirSentido) normalNorEste = (verticeNorte - verticeCentro) * (verticeEste - verticeCentro);
                else normalNorEste = (verticeEste - verticeCentro) * (verticeNorte - verticeCentro);
            }

            if (verticeSur != null && verticeEste != null)
            {
                if (invertirSentido) normalSurEste = (verticeEste - verticeCentro) * (verticeSur - verticeCentro);
                else normalSurEste = (verticeSur - verticeCentro) * (verticeEste - verticeCentro);
            }

            if (verticeSur != null && verticeOeste != null)
            {
                if (invertirSentido) normalSurOeste = (verticeSur - verticeCentro) * (verticeOeste - verticeCentro);
                else normalSurOeste = (verticeOeste - verticeCentro) * (verticeSur - verticeCentro);
            }

            if (verticeNorte != null && verticeOeste != null)
            {
                if (invertirSentido) normalNorOeste = (verticeOeste - verticeCentro) * (verticeNorte - verticeCentro);
                else normalNorOeste = (verticeNorte - verticeCentro) * (verticeOeste - verticeCentro);
            }

            normalRetorno = new Punto(0, 0, 0);

            if (normalNorEste != null)
                normalRetorno = normalRetorno.SumarPunto(normalNorEste);

            if (normalNorOeste != null)
                normalRetorno = normalRetorno.SumarPunto(normalNorOeste);

            if (normalSurEste != null)
                normalRetorno = normalRetorno.SumarPunto(normalSurEste);

            if (normalSurOeste != null)
                normalRetorno = normalRetorno.SumarPunto(normalSurOeste);

            return normalRetorno;
        }
    }
}
