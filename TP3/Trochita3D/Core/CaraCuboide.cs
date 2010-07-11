using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class CaraCuboide : CaraFigura
    {
        public CaraCuboide(Figura figura, OrientacionesCara orientacion, int numero, float[] luz, float[] luzBrillo, float[] luzAmbiente, int shininess) 
            : base(figura, orientacion, numero, luzAmbiente, luzBrillo, luz, shininess) { }

        #region Generadores

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

            for (double x = xInicial; x <= xFinal; x += ((this.PasoX == 0) ? 1 : this.PasoX))
            {
                for (double y = yInicial; y <= yFinal; y += ((this.PasoY == 0) ? 1 : this.PasoY))
                {
                    for (double z = zInicial; z <= zFinal; z += ((this.PasoZ == 0) ? 1 : this.PasoZ))
                    {
                        vertices.Add(new Punto(x, y, z));
                    }
                }
            }
        }

        #endregion
        #region Utilitarios

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

        protected override void CalcularExtremos()
        {
            if (Orientacion.Equals(OrientacionesCara.Abajo))
            {
                xInicial = 0;
                xFinal = Figura.Ancho;
                yInicial = 0;
                yFinal = Figura.Largo;
                zInicial = 0;
                zFinal = 0;
            }
            else if (Orientacion.Equals(OrientacionesCara.Adelante))
            {
                xInicial = Figura.Ancho;
                xFinal = Figura.Ancho;
                yInicial = 0;
                yFinal = Figura.Largo;
                zInicial = 0;
                zFinal = Figura.Alto;
            }
            else if (Orientacion.Equals(OrientacionesCara.Arriba))
            {
                xInicial = 0;
                xFinal = Figura.Ancho;
                yInicial = 0;
                yFinal = Figura.Largo;
                zInicial = Figura.Alto;
                zFinal = Figura.Alto;
            }
            else if (Orientacion.Equals(OrientacionesCara.Atraz))
            {
                xInicial = 0;
                xFinal = 0;
                yInicial = 0;
                yFinal = Figura.Largo;
                zInicial = 0;
                zFinal = Figura.Alto;
            }
            else if (Orientacion.Equals(OrientacionesCara.Derecha))
            {
                xInicial = 0;
                xFinal = Figura.Ancho;
                yInicial = Figura.Largo;
                yFinal = Figura.Largo;
                zInicial = 0;
                zFinal = Figura.Alto;
            }
            else if (Orientacion.Equals(OrientacionesCara.Izquierda))
            {
                xInicial = 0;
                xFinal = Figura.Ancho;
                yInicial = 0;
                yFinal = 0;
                zInicial = 0;
                zFinal = Figura.Alto;
            }

            xInicial += Figura.Posicion.X;
            xFinal += Figura.Posicion.X;
            yInicial += Figura.Posicion.Y;
            yFinal += Figura.Posicion.Y;
            zInicial += Figura.Posicion.Z;
            zFinal += Figura.Posicion.Z;
        }

        #endregion
    }
}
