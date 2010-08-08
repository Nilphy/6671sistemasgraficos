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
        protected Textura texture;

        public double UTextureAspectRatio { get; set; }
        public double VTextureAspectRatio { get; set; }

        protected int RENDER_MODE { get; set; }

        public Superficie()
        {
            this.RENDER_MODE = Gl.GL_TRIANGLE_STRIP; // Render mode default
        }

        #region Métodos de Dibujo

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
                this.texture.Activate();
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

        #endregion

        #region Construcción de la superficie

        /// <summary>
        /// Obtiene las secciones contiguas que representan la superficie modelada.
        /// </summary>
        protected abstract void LoadSecciones();

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

            // Armo la lista de vertices e indices. Esta ultima pensada que 
            // se arma con TRIANGLE.
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

        /// <summary>
        /// Carga la textura a ser aplicada sobre la superficie.
        /// </summary>
        /// <param name="fileName">Path al archivo imagen a ser utilizado para la textura</param>
        /// <param name="uAspectRatio">Relación de aspecto para la coordenada U</param>
        /// <param name="vAspectRatio">Relación de aspecto para la coordenada V</param>
        public virtual void LoadTextures(string fileName, double uAspectRatio, double vAspectRatio)
        {
            this.UTextureAspectRatio = uAspectRatio;
            this.VTextureAspectRatio = vAspectRatio;
            this.texture = new Textura(fileName, true);
        }

        /// <summary>
        /// Carga la textura a ser aplicada sobre la superficie.
        /// </summary>
        /// <param name="fileName">Path al archivo imagen a ser utilizado para la textura</param>
        public virtual void LoadTextures(string fileName)
        {
            this.LoadTextures(fileName, 1, 1);
        }

        /// <summary>
        /// Calcula las coordenadas de textura creando el buffer de coordenadas de textura
        /// para la superficie.
        /// </summary>
        protected virtual void BuildTextureCoordBuffer()
        {
            double u, v = 0;
            double distU = 0, distV = 0;
            IList<double> textCoord = new List<double>();
            
            for (int i = 0; i < secciones.Count; i++)
            {
                Seccion seccion = secciones[i];

                if (i != 0)
                {
                    Punto verticeAnt = secciones[i - 1].Vertices[secciones[i - 1].Vertices.Count / 2];
                    Punto verticeActual = seccion.Vertices[seccion.Vertices.Count / 2];
                    distV = (verticeActual - verticeAnt).Modulo();
                    v += distV * VTextureAspectRatio;
                }

                u = 0;
                for (int j = 0; j < seccion.Vertices.Count; j++)
                {
                    if (j != 0)
                    {
                        Punto verticeAnt = seccion.Vertices[j - 1];
                        Punto verticeActual = seccion.Vertices[j];
                        distU = (verticeActual - verticeAnt).Modulo();
                        u += distU * UTextureAspectRatio;
                    }

                    textCoord.Add(v);
                    textCoord.Add(u);
                }
            }

            this.textures = textCoord.ToArray<double>();
        }

        /// <summary>
        /// Construye la superficie a partir de los datos configurados para 
        /// la misma.
        /// </summary>
        public virtual void Build()
        {
            // Primero carga las secciones que componen al superficie
            this.LoadSecciones();

            // Genera los vertices correspondientes a la superficie dada
            // por las secciones junto con sus normales e índices.
            this.BuildSurfaceDataBuffers();

            // Si existe una textura a aplicar carga las coordenadas de 
            // textura
            if (this.HasTextures())
            {
                this.BuildTextureCoordBuffer();
            }
        }

        #endregion

        #region Metodos Auxiliares

        private bool HasTextures()
        {
            return this.texture != null;
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
