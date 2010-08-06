using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Curvas;
using Trochita3D.Entidades;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    /// <summary>
    /// Clase encargada de inicializar la superficie inicial de la escena.
    /// </summary>
    public class SurfaceInitializer
    {
        //private const double DELTA_U = 0.05;
        //private const double DELTA_U2 = 0.0005;
        //private const double ALTURA_TERRAPLEN = 2.2;
        //private const int DIST_TABLA = 2; // Distancia entre tablas de la vía.

        //private IList<Seccion> seccionesTablas = new List<Seccion>();
        //private IList<Punto> path;
        //private IList<Punto> detailPath;

        //private static int[] indicesTablas;

        //private static double[] verticesTablas;

        //private static double[] normalesTablas;

        private static double[] texCoordTerraplen;
        private static double[] texCoordRieles1;
        private static double[] texCoordRieles2;
        private static double[] texCoordTablas;

        private Textura texTierra;
        private Textura texRiel;

        public SurfaceInitializer()
        {
            //this.path = GetBsplineControlPoints(DELTA_U);
            //this.detailPath = GetBsplineControlPoints(DELTA_U2);

            this.LoadTextures();

            //this.BuildRieles();
            //this.BuildTablas();
        }

        /// <summary>
        /// Obtiene los puntos discretos del trayecto a realizar por el terraplen
        /// a partir de los puntos de control definidos en esta misma funcion.
        /// </summary>
        /// <param name="du">Delta U</param>
        /// <returns>
        /// Lista de vertices que corresponden a la curva que representa el trayecto.
        /// </returns>
        private IList<Punto> GetBsplineControlPoints(double du)
        {
            IList<Punto> ptsControl = new List<Punto>();

            ptsControl.Add(new Punto(55, 80, 0));
            ptsControl.Add(new Punto(72, 60, 0));
            ptsControl.Add(new Punto(76, 35, 0));
            ptsControl.Add(new Punto(46, 11, 0));
            ptsControl.Add(new Punto(46, -15, 0));
            ptsControl.Add(new Punto(60, -30, 0));
            ptsControl.Add(new Punto(75, -35, 0));
            ptsControl.Add(new Punto(83, -50, 0));
            ptsControl.Add(new Punto(83, -65, 0));
            ptsControl.Add(new Punto(77, -79, 0));
            ptsControl.Add(new Punto(43, -71, 0));
            ptsControl.Add(new Punto(21, -63, 0));
            ptsControl.Add(new Punto(1, -76, 0));
            ptsControl.Add(new Punto(-8, -74, 0));
            ptsControl.Add(new Punto(-25, -69, 0));
            ptsControl.Add(new Punto(-52, -64, 0));
            ptsControl.Add(new Punto(-63, -45, 0));
            ptsControl.Add(new Punto(-57, -14, 0));
            ptsControl.Add(new Punto(-40, 10, 0));
            ptsControl.Add(new Punto(-45, 30, 0));
            ptsControl.Add(new Punto(-67, 47, 0));
            ptsControl.Add(new Punto(-57, 70, 0));
            ptsControl.Add(new Punto(-23, 80, 0));

            CurvaBsplineSegmentosCubicos path = new CurvaBsplineSegmentosCubicos(ptsControl);

            return path.GetPuntosDiscretos(du);
        }

        #region Armado de Superficies

        /// <summary>
        /// Construye las secciones correspondientes a las tablas de los rieles. 
        /// Completa el listado de secciones ubicadas a través del trayecto indicado 
        /// en path para ir formando las secciones correspondientes a la superficie 
        /// de los rieles.
        /// </summary>
        /*
        private void BuildTablas()
        {
            Tabla tabla = new Tabla();
            Punto puntoAnterior = detailPath[detailPath.Count - 1];
            Punto puntoActual;
            Punto vectorPuntoAnteriorActual;
            Seccion seccion1, seccion2;
            double dx, dy, angulo;
            double distancia = 0;

            for (int i = 0; i < detailPath.Count; i++)
            {
                puntoActual = detailPath[i];
                vectorPuntoAnteriorActual = puntoActual - puntoAnterior;
                distancia += vectorPuntoAnteriorActual.Modulo();

                if (distancia > DIST_TABLA)
                {
                    seccion1 = tabla.GetSeccion();
                    seccion2 = tabla.GetSeccion();
                    distancia = 0;
                    dx = puntoActual.X - puntoAnterior.X;
                    dy = puntoActual.Y - puntoAnterior.Y;

                    angulo = Math.Atan(dy / dx);

                    int cuadrante = vectorPuntoAnteriorActual.GetCuadrante();
                    if (cuadrante.Equals(1) || cuadrante.Equals(4))
                        angulo -= Math.PI;

                    seccion1.Escalar(1, 1, 0.2);
                    seccion1.Rotar(angulo);
                    seccion1.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + ALTURA_TERRAPLEN - seccion1.Vertices[1].Z);

                    seccion2.Escalar(1, 1, 0.2);
                    seccion2.Trasladar(0.5, 0, 0);
                    seccion2.Rotar(angulo);
                    seccion2.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + ALTURA_TERRAPLEN - seccion2.Vertices[1].Z);

                    seccionesTablas.Add(seccion1);
                    seccionesTablas.Add(seccion2);
                }

                puntoAnterior = puntoActual;
            }
        }
        */

        /// <summary>
        /// Construye las distintas superficies de la escena.
        /// </summary>
        public void BuildSurface()
        {
            IList<int> indices = new List<int>();
            IList<double> vertices = new List<double>();
            IList<double> normales = new List<double>();

            // Tablas
            /*
            this.BuildTablasDataBuffers(seccionesTablas, vertices, indices, normales);
            verticesTablas = vertices.ToArray<double>();
            normalesTablas = normales.ToArray<double>();
            indicesTablas = indices.ToArray<int>();
            */
        }

        /// <summary>
        /// Calcula los datos necesarios para poder generar la superficie indicada por las secciones
        /// contiguas indicadas en la colección de secciones. A través de esta completa las listas de
        /// vértices, normales e índices correspondientes a la superficie a representar.
        /// </summary>
        /// <remarks>
        /// El método tiene cuatro parámetros de los cuales solo uno es de entrada (la lista de secciones)
        /// y los tres restantes son de salida. Es decir, se pasan tres listas las cuales serán LIMPIADAS
        /// por el método y cargadas con los valores correspondientes a las secciones indicadas.
        /// </remarks>
        /// <param name="secciones">Lista de secciones contiguas y ordenadas que representan
        /// la superficie final que se desea representar.</param>
        /// <param name="vertices">Lista de vertices que representan la superficies. La misma se encuentra
        /// ordenada para dibujar la superficie de manera directa con Gl.GL_QUAD_STRIP o Gl.GL_TRIANGLE_STRIP</param>
        /// <param name="indices">Lista de índices que de los vertices que forman los distintos polígonos 
        /// para graficar la superficie.</param>
        /// <param name="normales">Lista de normales correspondientes a los vértices de la superficie.</param>
        private void BuildSurfaceDataBuffers(IList<Seccion> secciones, IList<double> vertices, IList<int> indices, IList<double> normales)
        {
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
        }

        private void BuildTablasDataBuffers(IList<Seccion> secciones, IList<double> vertices, IList<int> indices, IList<double> normales)
        {
            Seccion seccion1, seccion2;
            int indexCount = 0;
            vertices.Clear();
            indices.Clear();
            normales.Clear();

            for (int i = 0; i < secciones.Count; i += 2)
            {
                seccion1 = secciones[i];
                seccion2 = secciones[i + 1];

                for (int j = 0; j < seccion1.Vertices.Count; j++)
                {
                    // Vertices
                    vertices.Add(seccion1.Vertices[j].X);
                    vertices.Add(seccion1.Vertices[j].Y);
                    vertices.Add(seccion1.Vertices[j].Z);

                    // Vertices
                    vertices.Add(seccion2.Vertices[j].X);
                    vertices.Add(seccion2.Vertices[j].Y);
                    vertices.Add(seccion2.Vertices[j].Z);

                    // Normales
                    Punto normal = this.GetNormalForVertex(seccion1, j);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);

                    normal = this.GetNormalForVertex(seccion2, j);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);
                }

                // Q1
                indices.Add(indexCount);
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 1);

                // Q2
                indices.Add(indexCount + 1);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 7);

                // Q3
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 7);
                indices.Add(indexCount + 6);
                indices.Add(indexCount + 4);

                // Q4
                indices.Add(indexCount);
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 4);
                indices.Add(indexCount + 6);

                // Q5
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 4);

                // Q6
                indices.Add(indexCount);
                indices.Add(indexCount + 1);
                indices.Add(indexCount + 7);
                indices.Add(indexCount + 6);

                indexCount += 8;
            }
        }

        #endregion

        #region Dibujo

        public void DrawSurface()
        {
            Gl.glEnable(Gl.GL_LIGHTING);

            //Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
            //Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            //this.DrawTablas();

            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);
            //Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            //Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_LIGHTING);
        }

        /// <summary>
        /// Dibuja la superficie correspondiente a las tablas de los rieles tomando 
        /// los valores previamente cargados en las listas de vertices, normales e 
        /// índices.
        /// </summary>
        /*
        private void DrawTablas()
        {
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesTablas);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesTablas);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 128f / 255f, 64f / 255f, 0.0f, 0.25f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 128f / 255f, 64f / 255f, 0.0f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glDrawElements(Gl.GL_QUADS, indicesTablas.Length, Gl.GL_UNSIGNED_INT, indicesTablas);
        }
        */

        #endregion

        #region Métodos Auxiliares

        private Punto GetNormalForVertex(Seccion seccion, int vertexPos)
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

        private void LoadTextures()
        {
            this.texTierra = new Textura(@"../../Imagenes/Texturas/Tierra.bmp", true);
            this.texRiel = new Textura(@"../../Imagenes/Texturas/Riel.bmp", true);
        }
    }
}
