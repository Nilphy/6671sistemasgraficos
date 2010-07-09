using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class CaraFigura
    {
        #region atributos y propiedades

        public Figura Figura { set; get; }
        public OrientacionesCara Orientacion { set; get; }
        public int Numero { set; get; }

        private double[] pVertices;
        public double[] Vertices
        {
            get
            {
                if (recalcularIndices)
                {
                    pVertices = new double[vertices.Count * 3];
                    int i = 0;

                    foreach (Punto vertice in vertices)
                    {
                        pVertices[i++] = vertice.X;
                        pVertices[i++] = vertice.Y;
                        pVertices[i++] = vertice.Z;
                    }
                }

                return pVertices;
            }
        }
        private IList<Punto> vertices;

        private double[] pNormales;
        public double[] Normales
        {
            get
            {
                if (recalcularIndices)
                {
                    pNormales = new Double[normales.Count * 3];
                    int i = 0;

                    foreach (Punto normal in normales)
                    {
                        pNormales[i++] = normal.X;
                        pNormales[i++] = normal.Y;
                        pNormales[i++] = normal.Z;
                    }
                }

                return pNormales;
            }
        }
        private IList<Punto> normales;

        private int[] pIndices;
        public int[] Indices 
        { 
            get 
            {
                if (recalcularIndices) pIndices = this.indices.ToArray<int>();
                return pIndices;
            } 
        }
        private IList<int> indices;

        // Para cálculos
        private double xInicial;
        private double xFinal;
        private double yInicial;
        private double yFinal;
        private double zInicial;
        private double zFinal;

        // Calculan la distancia entre dos vértices dada la cantidad de divisiones
        private double PasoX
        {
            get
            {
                return ((xFinal - xInicial) / Figura.CantidadDivisionesAncho) > 0 ? ((xFinal - xInicial) / Figura.CantidadDivisionesAncho) : -((xFinal - xInicial) / Figura.CantidadDivisionesAncho);
            }
        }
        private double PasoY
        {
            get
            {
                return ((yFinal - yInicial) / Figura.CantidadDivisionesLargo) > 0 ? ((yFinal - yInicial) / Figura.CantidadDivisionesLargo) : -((yFinal - yInicial) / Figura.CantidadDivisionesLargo);
            }
        }
        private double PasoZ
        {
            get
            {
                return ((zFinal - zInicial) / Figura.CantidadDivisionesAlto) > 0 ? ((zFinal - zInicial) / Figura.CantidadDivisionesAlto) : -((zFinal - zInicial) / Figura.CantidadDivisionesAlto);
            }
        }

        private int CantidadPixelesAncho
        {
            get
            {
                int cantidadPixelesAncho = 0;

                if (this.Orientacion.Equals(OrientacionesCara.Abajo) || this.Orientacion.Equals(OrientacionesCara.Arriba))
                    cantidadPixelesAncho = Figura.CantidadDivisionesLargo;
                else if (this.Orientacion.Equals(OrientacionesCara.Derecha) || this.Orientacion.Equals(OrientacionesCara.Izquierda))
                    cantidadPixelesAncho = Figura.CantidadDivisionesAncho;
                else
                    cantidadPixelesAncho = Figura.CantidadDivisionesLargo;

                return cantidadPixelesAncho + 1;
            }
        }
        private int CantidadPixelesAlto
        {
            get
            {
                if (this.Orientacion.Equals(OrientacionesCara.Abajo) || this.Orientacion.Equals(OrientacionesCara.Arriba))
                    return Figura.CantidadDivisionesAncho + 1;                
                else
                    return Figura.CantidadDivisionesAlto + 1;
            }
        }

        // Cada vez que se vuelve a generar la cara cuboide se deben recalcular los arrais 
        private Boolean recalcularIndices;

        #endregion
               
        public CaraFigura(Figura figura, OrientacionesCara orientacion, int numero)
        {
            this.Figura = figura;
            this.Orientacion = orientacion;
            this.Numero = numero;
        }

        public void Draw()
        {
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, new float[] { 0.20f, 0.20f, 0.35f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0.5f, 0.5f, 0.5f, 1 });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), this.Vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), this.Normales);

            Gl.glDrawElements(Gl.GL_QUADS, this.Indices.Length, Gl.GL_UNSIGNED_INT, this.Indices);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
        }

        #region Generadores

        /// <summary>
        /// No está en el constructor para que se controle cuando lo quiero generar
        /// </summary>
        public void Generar()
        {
            this.recalcularIndices = true;
 
            this.CalcularExtremos();
            this.GenerarVertices();
            this.GenerarIndices();
            this.GenerarNormales();
        }

        private void GenerarNormales()
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

        private void GenerarIndices()
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

        private void GenerarVertices()
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

        /// <summary>
        /// Dadas las dimensiones del cuboide calcula los extremos de los rangos de las coordenadas del cubo
        /// </summary>
        private void CalcularExtremos()
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

        private Punto CompletarPunto(Punto puntoCentro)
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
    }
}
