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
        
        #region Generadores

        protected override void GenerarNormales()
        {
            normales = new List<Punto>();
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;
            Punto puntoCentro = null;
            
            for (int indiceFila = 0; indiceFila < CantidadPixelesAlto; indiceFila++) // indice fila
            {
                for (int indiceColumna = 0; indiceColumna < CantidadPixelesAncho; indiceColumna++) // indice columna
                {
                    puntoCentro = vertices[indiceFila * CantidadPixelesAncho + indiceColumna];
                    
                    if (indiceFila + 1 < CantidadPixelesAlto - 1) // Hay punto norte
                        puntoNorte = vertices[(indiceFila + 1) * CantidadPixelesAncho + indiceColumna];
                    else puntoNorte = this.CompletarPunto(puntoCentro);

                    if (indiceFila - 1 >= 0) // Hay punto sur
                        puntoSur = vertices[(indiceFila - 1) * CantidadPixelesAncho + indiceColumna];
                    else puntoSur = this.CompletarPunto(puntoCentro);
                    
                    if (indiceColumna - 1 >= 0) // Hay punto este
                        puntoOeste = vertices[indiceFila * CantidadPixelesAncho + indiceColumna - 1];
                    else puntoOeste = this.CompletarPunto(puntoCentro);

                    if (indiceColumna + 1 < CantidadPixelesAncho - 1) // Hay punto oeste
                        puntoEste = vertices[indiceFila * CantidadPixelesAncho + indiceColumna + 1];
                    else puntoEste = this.CompletarPunto(puntoCentro);

                    normales.Add(Punto.CalcularNormal(puntoCentro, puntoNorte, puntoEste, puntoSur, puntoOeste, false));
                }
            }
        }

        protected override void GenerarIndices()
        {
            this.indices = new List<int>();

            for (int indicePrimerElementoFila = 0; indicePrimerElementoFila < ((CantidadPixelesAlto - 1) * CantidadPixelesAncho); indicePrimerElementoFila += CantidadPixelesAlto)
            {
                for (int numeroColumna = 0; numeroColumna < CantidadPixelesAncho - 1; numeroColumna++)
                {
                    indices.Add(numeroColumna + indicePrimerElementoFila);
                    indices.Add(numeroColumna + indicePrimerElementoFila + CantidadPixelesAncho);
                    indices.Add(numeroColumna + indicePrimerElementoFila + CantidadPixelesAncho + 1);
                    indices.Add(numeroColumna + indicePrimerElementoFila + 1);
                }
            }
        }

        protected override void GenerarVertices()
        {
            this.vertices = new List<Punto>();

            for (double x = xInicial; x <= xFinal; x += ((this.PasoX == 0) ? 1 : this.PasoX))
            {
                for (double z = zInicial; z <= zFinal; z += ((this.PasoZ == 0) ? 1 : this.PasoZ))
                {
                    vertices.Add(new Punto(x, this.CalcularY(x, z), z));
                }
            }
        }

        #endregion
        #region Utilitarios

        protected override Punto CompletarPunto(Punto puntoCentro)
        {
            return null; 
        }

        /// <summary>
        /// Dadas las dimensiones del cuboide calcula los extremos de los rangos de las coordenadas del cubo
        /// </summary>
        protected override void CalcularExtremos()
        {
            if (Orientacion.Equals(OrientacionesCara.Izquierda))
            {
                xInicial = 0;
                xFinal = Figura.Ancho / 2d;
            }
            else
            {
                xInicial = Figura.Ancho / 2d;
                xFinal = Figura.Ancho;
            }
            
            yInicial = 0;
            yFinal = Figura.Largo;
            zInicial = 0;
            zFinal = Figura.Alto;

            xInicial += Figura.Posicion.X;
            xFinal += Figura.Posicion.X;
            yInicial += Figura.Posicion.Y;
            yFinal += Figura.Posicion.Y;
            zInicial += Figura.Posicion.Z;
            zFinal += Figura.Posicion.Z;
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
