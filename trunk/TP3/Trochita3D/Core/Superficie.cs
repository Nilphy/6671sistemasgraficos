using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace Trochita3D.Core
{
    public abstract class Superficie
    {
        protected IList<Seccion> secciones = new List<Seccion>();
        protected double[] vertex;
        protected double[] normals;
        protected int[] indexes;
        protected double[] textures;

        protected int RENDER_MODE { get; set; }

        public Superficie()
        {
            this.RENDER_MODE = Gl.GL_QUAD_STRIP; // Render mode default
        }

        /// <summary>
        /// Obtiene las secciones contiguas que representan la superficie modelada.
        /// </summary>
        protected abstract void LoadSecciones();

        protected virtual void LoadTextures() { }

        /// <summary>
        /// Carga las propiedades de material correspondientes a la superficie.
        /// </summary>
        protected virtual void LoadMaterialProperties() { }

        public void Dibujar()
        {
            Gl.glEnable(Gl.GL_LIGHTING);

            if (this.HasTextures())
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
                // this.texture.Activate();
            }
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);

            this.LoadMaterialProperties();

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertex);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normals);
            if (this.HasTextures()) Gl.glTexCoordPointer(2, Gl.GL_DOUBLE, 2 * sizeof(double), textures);

            Gl.glDrawElements(this.RENDER_MODE, indexes.Length, Gl.GL_UNSIGNED_INT, indexes);

            if (this.HasTextures())
            {
                Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);

            Gl.glDisable(Gl.GL_LIGHTING);
        }

        /// <summary>
        /// Calcula los datos necesarios para poder generar la superficie indicada por las secciones
        /// contiguas indicadas en la colección de secciones. A través de esta calcula los vértices,
        /// normales e índices correspondientes a la superficie a representar.
        /// </summary>
        protected virtual void BuildSurfaceDataBuffers()
        {
            IList<double> vertices = new List<double>();
            IList<int> indices = new List<int>();
            IList<double> normales = new List<double>();
            Seccion seccion;
            int indexCount = 0;
            vertices.Clear();
            indices.Clear();
            normales.Clear();

            // Armo la lista de vertices e indices. Esta ultima pensada que se arma con TRIANGLE.
            for (int i = 0; i < secciones.Count; i++)
            {
                seccion = secciones[i];

                for (int j = 0; j < seccion.Vertices.Count; j++)
                {
                    // Vertices
                    vertices.Add(seccion.Vertices[j].X);
                    vertices.Add(seccion.Vertices[j].Y);
                    vertices.Add(seccion.Vertices[j].Z);

                    // Indices
                    indices.Add(indexCount);
                    indices.Add((indexCount + seccion.Vertices.Count) % (secciones.Count * seccion.Vertices.Count));
                    indexCount++;

                    // Normales
                    Punto normal = this.GetNormalForVertex(seccion, j);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);
                }
            }

            this.vertex = vertices.ToArray<double>();
            this.normals = normales.ToArray<double>();
            this.indexes = indices.ToArray<int>();
        }

        #region Metodos Auxiliares

        private bool HasTextures()
        {
            return this.textures != null;
        }

        protected Punto GetNormalForVertex(Seccion seccion, int vertexPos)
        {
            Punto normalSeccion = (seccion.Vertices[1] - seccion.Vertices[0]) * (seccion.Vertices[2] - seccion.Vertices[1]);

            if (vertexPos == 0)
            {
                return (seccion.Vertices[vertexPos + 1] - seccion.Vertices[vertexPos]) * normalSeccion;
            }
            else if (vertexPos == (seccion.Vertices.Count - 1))
            {
                return (seccion.Vertices[vertexPos] - seccion.Vertices[vertexPos - 1]) * normalSeccion;
            }
            else
            {
                return (seccion.Vertices[vertexPos + 1] - seccion.Vertices[vertexPos - 1]) * normalSeccion;
            }
        }

        #endregion

        #region Metodos DEBUG

        public void DibujarNormales()
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            for (int i = 0; i < vertex.Length; i += 3)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(vertex[i], vertex[i + 1], vertex[i + 2]);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(1, 0, 0);
                Gl.glVertex3d(0, 0, 0);
                Gl.glColor3d(0.2, 0, 0);
                Gl.glVertex3d(normals[i], normals[i + 1], normals[i + 2]);
                Gl.glEnd();
                Gl.glPopMatrix();
            }
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        #endregion

    }
}
