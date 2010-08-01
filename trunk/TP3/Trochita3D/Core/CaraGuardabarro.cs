using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class CaraGuardabarro : CaraFigura
    {
        public CaraGuardabarro(Figura figura, OrientacionesCara orientacion, int numero, float[] luzAmbiente, float[] luzBrillo, float[] luz, int shininess) :
            base(figura, orientacion, numero, luzAmbiente, luzBrillo, luz, shininess) { }
        
        #region Métodos Heredados

        protected override void GenerarNormales()
        {
            normales = new List<Punto>();
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;
            Punto puntoCentro = null;

            for (int indiceColumna = 0; indiceColumna < CantidadPixelesAncho; indiceColumna++) // indice columna
            {
                for (int indiceFila = 0; indiceFila < CantidadPixelesAlto; indiceFila++) // indice fila
                {
                    puntoCentro = vertices[indiceFila + (indiceColumna * CantidadPixelesAlto)];

                    if (indiceColumna + 1 < CantidadPixelesAncho - 1) // Hay punto norte
                        puntoNorte = vertices[(indiceColumna + 1) * CantidadPixelesAlto + indiceFila];
                    else puntoNorte = this.CompletarPunto(puntoCentro);

                    if (indiceColumna - 1 >= 0) // Hay punto sur
                        puntoSur = vertices[(indiceColumna - 1) * CantidadPixelesAlto + indiceFila];
                    else puntoSur = this.CompletarPunto(puntoCentro);

                    if (indiceFila + 1 < CantidadPixelesAlto) // Hay punto este
                        puntoEste = vertices[indiceColumna * CantidadPixelesAlto + indiceFila + 1];
                    else puntoEste = this.CompletarPunto(puntoCentro);

                    if (indiceFila - 1 >= 0) // Hay punto oeste
                        puntoOeste = vertices[indiceColumna * CantidadPixelesAlto + indiceFila - 1];
                    else puntoOeste = this.CompletarPunto(puntoCentro);

                    normales.Add(Punto.CalcularNormal(puntoCentro, puntoNorte, puntoEste, puntoSur, puntoOeste, false));
                }
            }
        }

        protected override void GenerarIndices()
        {
            this.indices = new List<int>();

            for (int indicePrimerElementoColumna = 0; indicePrimerElementoColumna < ((CantidadPixelesAlto - 1) * (CantidadPixelesAncho - 1)); indicePrimerElementoColumna += CantidadPixelesAlto)
            {
                for (int numeroFila = 0; numeroFila < CantidadPixelesAlto - 1; numeroFila++)
                {
                    indices.Add(numeroFila + indicePrimerElementoColumna);
                    indices.Add(numeroFila + indicePrimerElementoColumna + CantidadPixelesAlto);
                    indices.Add(numeroFila + indicePrimerElementoColumna + CantidadPixelesAlto + 1);
                    indices.Add(numeroFila + indicePrimerElementoColumna + 1);
                }
            }
        }

        protected override void GenerarVertices()
        {
            this.vertices = new List<Punto>();

            for (int indicePixelX = 0; indicePixelX < this.CantidadPixelesAncho; indicePixelX++)
            {
                for (int indicePixelZ = 0; indicePixelZ < this.CantidadPixelesAlto; indicePixelZ++)
                {
                    vertices.Add(this.CalcularPuntoXindices(indicePixelX, indicePixelZ));
                }
            }
        }

        protected override Punto CompletarPunto(Punto puntoCentro)
        {
            return null;
        }

        protected override void CalcularExtremos()
        {
            if (Orientacion.Equals(OrientacionesCara.Izquierda))
            {
                xInicial = 0;
                xFinal = Figura.LongitudX / 2d;
            }
            else
            {
                xInicial = Figura.LongitudX / 2d;
                xFinal = Figura.LongitudX;
            }

            yInicial = 0;
            yFinal = Figura.LongitudY;
            zInicial = 0;
            zFinal = Figura.LongitudZ;

            xInicial += Figura.Posicion.X;
            xFinal += Figura.Posicion.X;
            yInicial += Figura.Posicion.Y;
            yFinal += Figura.Posicion.Y;
            zInicial += Figura.Posicion.Z;
            zFinal += Figura.Posicion.Z;
        }

        protected override void CalcularCantidadDivisiones()
        {
            this.CantidadPixelesAncho = (Figura.CantidadDivisionesX / 2) + 1;
            this.CantidadPixelesAlto = Figura.CantidadDivisionesZ + 1;
        }

        #endregion
        #region Utilitarios

        private Punto CalcularPuntoXindices(int indicePixelX, int indicePixelZ)
        {
            Punto punto = new Punto();

            punto.X = this.xInicial + ((double)indicePixelX) * this.Figura.PasoX;
            punto.Z = this.zInicial + ((double)indicePixelZ) * this.Figura.PasoZ;
            punto.Y = this.CalcularY(punto.X, punto.Z);

            return punto;
        }

        private double CalcularY(double x, double z)
        {
            // El valor de y máximo depende de z
            Double yMax = (yFinal - yInicial) * (1 - ((z - zInicial) / (zFinal - zInicial)));

            // Cuanto más avanzado en x más cercano al y final
            if (Orientacion.Equals(OrientacionesCara.Izquierda))
                return yInicial + (yMax * ((x - xInicial) / (xFinal - xInicial)));
            else
                return yInicial + (yMax * (1 - ((x - xInicial) / (xFinal - xInicial))));
        }

        #endregion
    }
}
