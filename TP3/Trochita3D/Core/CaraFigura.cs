using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public abstract class CaraFigura
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
        protected IList<Punto> vertices;

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
        protected IList<Punto> normales;

        private int[] pIndices;
        public int[] Indices 
        { 
            get 
            {
                if (recalcularIndices) pIndices = this.indices.ToArray<int>();
                return pIndices;
            } 
        }
        protected IList<int> indices;

        // Para cálculos
        protected double xInicial;
        protected double xFinal;
        protected double yInicial;
        protected double yFinal;
        protected double zInicial;
        protected double zFinal;

        protected virtual int CantidadPixelesAncho { set; get; }
        protected virtual int CantidadPixelesAlto { set; get; }

        // Cada vez que se vuelve a generar la cara cuboide se deben recalcular los arrais 
        private Boolean recalcularIndices;

        public float[] LuzAmbiente { set; get; }
        public float[] LuzBrillo { set; get; }
        public float[] Luz { set; get; }
        public int Shininess { set; get; }

        #endregion

        public CaraFigura(Figura figura, OrientacionesCara orientacion, int numero, float[] luzAmbiente, float[] luzBrillo, float[] luz, int shininess)
        {
            this.Figura = figura;
            this.Orientacion = orientacion;
            this.Numero = numero;
            this.Luz = luz;
            this.LuzAmbiente = luzAmbiente;
            this.LuzBrillo = luzBrillo;
            this.Shininess = shininess;

            this.CalcularCantidadDivisiones();
        }

        #region Metodos abstractos 

        protected abstract void CalcularCantidadDivisiones();

        protected abstract void CalcularExtremos();

        protected abstract Punto CompletarPunto(Punto puntoCentro);

        protected abstract void GenerarNormales();

        protected abstract void GenerarIndices();

        protected abstract void GenerarVertices();

        #endregion 

        public void Draw()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, LuzAmbiente);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, Luz);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, LuzBrillo);
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, this.Shininess);

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), this.Vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), this.Normales);

            Gl.glDrawElements(Gl.GL_QUADS, this.Indices.Length, Gl.GL_UNSIGNED_INT, this.Indices);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
        }

        public void Generar()
        {
            this.recalcularIndices = true;
 
            this.CalcularExtremos();
            this.GenerarVertices();
            this.GenerarIndices();
            this.GenerarNormales();
        }

    }
}
