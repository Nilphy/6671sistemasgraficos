using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class CaraCuboide : CaraFigura
    {
        private int cantidadPixelesX;
        private int cantidadPixelesY;
        private int cantidadPixelesZ;

        protected override int CantidadPixelesAncho
        {
            get
            {
                int cantidadPixelesAncho = 0;

                if (this.Orientacion.Equals(OrientacionesCara.Abajo) || this.Orientacion.Equals(OrientacionesCara.Arriba))
                    cantidadPixelesAncho = Figura.CantidadDivisionesY;
                else if (this.Orientacion.Equals(OrientacionesCara.Derecha) || this.Orientacion.Equals(OrientacionesCara.Izquierda))
                    cantidadPixelesAncho = Figura.CantidadDivisionesX;
                else
                    cantidadPixelesAncho = Figura.CantidadDivisionesY;

                return cantidadPixelesAncho + 1;
            }
        }
        protected override int CantidadPixelesAlto
        {
            get
            {
                if (this.Orientacion.Equals(OrientacionesCara.Abajo) || this.Orientacion.Equals(OrientacionesCara.Arriba))
                    return Figura.CantidadDivisionesX + 1;
                else
                    return Figura.CantidadDivisionesZ + 1;
            }
        }

        public CaraCuboide(Figura figura, OrientacionesCara orientacion, int numero, float[] luz, float[] luzBrillo, float[] luzAmbiente, int shininess) 
            : base(figura, orientacion, numero, luzAmbiente, luzBrillo, luz, shininess) { }

        #region Métodos heredados

        protected override void GenerarNormales()
        {
            normales = new List<Punto>();
            Boolean invertirNormal = false;
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;
            Punto puntoCentro = null;

            if (Orientacion.Equals(OrientacionesCara.Arriba) || 
                Orientacion.Equals(OrientacionesCara.Adelante) ||
                Orientacion.Equals(OrientacionesCara.Izquierda)) invertirNormal = true;

            if (Orientacion.Equals(OrientacionesCara.Arriba) || Orientacion.Equals(OrientacionesCara.Abajo))
            {
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

                        normales.Add(Punto.CalcularNormal(puntoCentro, puntoNorte, puntoEste, puntoSur, puntoOeste, invertirNormal));
                    }
                }
            }
            else
            {
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

                        normales.Add(Punto.CalcularNormal(puntoCentro, puntoNorte, puntoEste, puntoSur, puntoOeste, invertirNormal));
                    }
                }
            }
        }

        protected override void GenerarIndices()
        {
            this.indices = new List<int>();

            if (Orientacion.Equals(OrientacionesCara.Arriba) || Orientacion.Equals(OrientacionesCara.Abajo))
            {
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
            else
            {
                
                for (int indicePrimerElementoColumna = 0; indicePrimerElementoColumna < ((CantidadPixelesAlto - 1) * CantidadPixelesAncho); indicePrimerElementoColumna += CantidadPixelesAncho)
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
        }

        protected override void GenerarVertices()
        {
            this.vertices = new List<Punto>();

            for (int indicePixelX = 0; indicePixelX < this.cantidadPixelesX; indicePixelX++)
            {
                for (int indicePixelY = 0; indicePixelY < this.cantidadPixelesY; indicePixelY++)
                {
                    for (int indicePixelZ = 0; indicePixelZ < this.cantidadPixelesZ; indicePixelZ++)
                    {
                        vertices.Add(this.CalcularPuntoXCoordenadasPixeles(indicePixelX, indicePixelY, indicePixelZ));
                    }
                }
            }
        }

        protected override void CalcularCantidadDivisiones()
        {
            this.cantidadPixelesX = Figura.CantidadDivisionesX + 1;
            this.cantidadPixelesY = Figura.CantidadDivisionesY + 1;
            this.cantidadPixelesZ = Figura.CantidadDivisionesZ + 1;

            if (this.Orientacion.Equals(OrientacionesCara.Abajo) ||
                this.Orientacion.Equals(OrientacionesCara.Arriba))
            {
                this.cantidadPixelesZ = 1;
            }
            else if (this.Orientacion.Equals(OrientacionesCara.Adelante) ||
                this.Orientacion.Equals(OrientacionesCara.Atraz))
            {
                this.cantidadPixelesX = 1;
            }
            else
            {
                this.cantidadPixelesY = 1;
            }

        }

        protected override void CalcularExtremos()
        {
            if (Orientacion.Equals(OrientacionesCara.Abajo))
            {
                xInicial = 0;
                xFinal = Figura.LongitudX;
                yInicial = 0;
                yFinal = Figura.LongitudY;
                zInicial = 0;
                zFinal = 0;
            }
            else if (Orientacion.Equals(OrientacionesCara.Adelante))
            {
                xInicial = Figura.LongitudX;
                xFinal = Figura.LongitudX;
                yInicial = 0;
                yFinal = Figura.LongitudY;
                zInicial = 0;
                zFinal = Figura.LongitudZ;
            }
            else if (Orientacion.Equals(OrientacionesCara.Arriba))
            {
                xInicial = 0;
                xFinal = Figura.LongitudX;
                yInicial = 0;
                yFinal = Figura.LongitudY;
                zInicial = Figura.LongitudZ;
                zFinal = Figura.LongitudZ;
            }
            else if (Orientacion.Equals(OrientacionesCara.Atraz))
            {
                xInicial = 0;
                xFinal = 0;
                yInicial = 0;
                yFinal = Figura.LongitudY;
                zInicial = 0;
                zFinal = Figura.LongitudZ;
            }
            else if (Orientacion.Equals(OrientacionesCara.Derecha))
            {
                xInicial = 0;
                xFinal = Figura.LongitudX;
                yInicial = Figura.LongitudY;
                yFinal = Figura.LongitudY;
                zInicial = 0;
                zFinal = Figura.LongitudZ;
            }
            else if (Orientacion.Equals(OrientacionesCara.Izquierda))
            {
                xInicial = 0;
                xFinal = Figura.LongitudX;
                yInicial = 0;
                yFinal = 0;
                zInicial = 0;
                zFinal = Figura.LongitudZ;
            }

            xInicial += Figura.Posicion.X;
            xFinal += Figura.Posicion.X;
            yInicial += Figura.Posicion.Y;
            yFinal += Figura.Posicion.Y;
            zInicial += Figura.Posicion.Z;
            zFinal += Figura.Posicion.Z;
        }
        
        protected override Punto CompletarPunto(Punto puntoCentro)
        {
            if (Orientacion.Equals(OrientacionesCara.Abajo))
            {
                return puntoCentro.SumarPunto(new Punto(0, 0, Figura.PasoZ));
            }
            else if (Orientacion.Equals(OrientacionesCara.Adelante))
            {
                return puntoCentro.SumarPunto(new Punto(-Figura.PasoX, 0, 0));
            }
            else if (Orientacion.Equals(OrientacionesCara.Arriba))
            {
                return puntoCentro.SumarPunto(new Punto(0, 0, -Figura.PasoZ));
            }
            else if (Orientacion.Equals(OrientacionesCara.Atraz))
            {
                return puntoCentro.SumarPunto(new Punto(Figura.PasoX, 0, 0));
            }
            else if (Orientacion.Equals(OrientacionesCara.Derecha))
            {
                return puntoCentro.SumarPunto(new Punto(0, -Figura.PasoY, 0));
            }
            else //if (Orientacion.Equals(OrientacionesCara.Izquierda))
            {
                return puntoCentro.SumarPunto(new Punto(0, Figura.PasoY, 0));
            }
        }

        #endregion
        #region Utilitarios

        private Punto CalcularPuntoXCoordenadasPixeles(int indicePixelX, int indicePixelY, int indicePixelZ)
        {
            Punto punto = new Punto();

            punto.X = this.xInicial + indicePixelX * this.Figura.PasoX;
            punto.Y = this.yInicial + indicePixelY * this.Figura.PasoY;
            punto.Z = this.zInicial + indicePixelZ * this.Figura.PasoZ;

            return punto;
        }

        #endregion
    }
}
