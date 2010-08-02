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
        private const double DELTA_U = 0.05;
        private const double DELTA_U2 = 0.0005;
        private const double ALTURA_TERRAPLEN = 2.2;
        private const double DIST_RIELES = 0.4;
        private const int CANT_PUNTOS_TERRAPLEN = 10;
        private const int DIST_TABLA = 2; // Distancia entre tablas de la vía.

        private IList<Seccion> seccionesTerraplen = new List<Seccion>();
        private IList<Seccion> seccionesRieles1 = new List<Seccion>();
        private IList<Seccion> seccionesRieles2 = new List<Seccion>();
        private IList<Seccion> seccionesTablas = new List<Seccion>();
        private IList<Punto> path;
        private IList<Punto> detailPath;
        private IList<double> distanciaAcumuladaPorPuntoPath;

        private static int[] indicesTerraplen;
        private static int[] indicesRieles1;
        private static int[] indicesRieles2;
        private static int[] indicesTablas;

        private static double[] verticesTerraplen;
        private static double[] verticesRieles1;
        private static double[] verticesRieles2;
        private static double[] verticesTablas;

        private static double[] normalesTerraplen;
        private static double[] normalesRieles1;
        private static double[] normalesRieles2;
        private static double[] normalesTablas;

        private static double[] texCoordTerraplen;
        private static double[] texCoordRieles1;
        private static double[] texCoordRieles2;
        private static double[] texCoordTablas;

        private Textura texTierra;
        private Textura texRiel;

        public SurfaceInitializer()
        {
            this.path = GetBsplineControlPoints(DELTA_U);
            this.detailPath = GetBsplineControlPoints(DELTA_U2);

            this.LoadTextures();

            this.BuildTerraplen();
            this.BuildRieles();
            this.BuildTablas();
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
        /// Construye las secciones correspondientes al terraplen. Completa el listado
        /// de secciones ubicadas a través del trayecto indicado en path para ir
        /// formando las secciones correspondientes a la superficie del terraplen.
        /// </summary>
        private void BuildTerraplen()
        {
            distanciaAcumuladaPorPuntoPath = new List<double>();
            Terraplen terraplen = new Terraplen(ALTURA_TERRAPLEN);
            Punto puntoAnterior = path[path.Count - 1];
            Punto puntoActual;
            Punto vectorPuntoAnteriorActual;
            Seccion seccion;
            seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);
            double dx, dy, angulo;

            for (int i = 0; i < path.Count; i++)
            {
                puntoActual = path[i];
                vectorPuntoAnteriorActual = puntoActual - puntoAnterior;
                seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);
                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;
                angulo = Math.Atan(dy / dx);

                int cuadrante = vectorPuntoAnteriorActual.GetCuadrante();
                if (cuadrante.Equals(1) || cuadrante.Equals(4))
                    angulo -= Math.PI;

                seccion.Rotar(angulo);
                seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);
                seccionesTerraplen.Add(seccion);

                if (i == 0)
                    distanciaAcumuladaPorPuntoPath.Add(0d);
                else
                    distanciaAcumuladaPorPuntoPath.Add(puntoAnterior.CalcularDistancia(puntoActual) + distanciaAcumuladaPorPuntoPath[i-1]);

                puntoAnterior = puntoActual;
            }
        }

        /// <summary>
        /// Construye las secciones correspondientes al riel. Completa el listado
        /// de secciones ubicadas a través del trayecto indicado en path para ir
        /// formando las secciones correspondientes a la superficie de los rieles.
        /// </summary>
        private void BuildRieles()
        {
            Riel riel = new Riel();
            Punto puntoAnterior = path[path.Count - 1];
            Punto puntoActual;
            Punto vectorPuntoAnteriorActual;
            Seccion seccionRiel1;
            Seccion seccionRiel2;
            double dx, dy, angulo;

            for (int i = 0; i < path.Count; i++)
            {
                puntoActual = path[i];
                vectorPuntoAnteriorActual = puntoActual - puntoAnterior;
                seccionRiel1 = riel.GetSeccion();
                seccionRiel2 = riel.GetSeccion();

                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;

                angulo = Math.Atan(dy / dx);

                int cuadrante = vectorPuntoAnteriorActual.GetCuadrante();
                if (cuadrante.Equals(1) || cuadrante.Equals(4))
                    angulo -= Math.PI;

                seccionRiel1.Escalar(1, 0.2, 0.2);
                seccionRiel1.Trasladar(0, -DIST_RIELES, 0);
                seccionRiel1.Rotar(angulo);
                seccionRiel1.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + ALTURA_TERRAPLEN);

                seccionRiel2.Escalar(1, 0.2, 0.2);
                seccionRiel2.Trasladar(0, DIST_RIELES, 0);
                seccionRiel2.Rotar(angulo);
                seccionRiel2.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + ALTURA_TERRAPLEN);

                seccionesRieles1.Add(seccionRiel1);
                seccionesRieles2.Add(seccionRiel2);
                puntoAnterior = puntoActual;
            }
        }

        /// <summary>
        /// Construye las secciones correspondientes a las tablas de los rieles. 
        /// Completa el listado de secciones ubicadas a través del trayecto indicado 
        /// en path para ir formando las secciones correspondientes a la superficie 
        /// de los rieles.
        /// </summary>
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

        /// <summary>
        /// Construye las distintas superficies de la escena.
        /// </summary>
        public void BuildSurface()
        {
            IList<int> indices = new List<int>();
            IList<double> vertices = new List<double>();
            IList<double> normales = new List<double>();

            // Terraplen
            this.BuildSurfaceDataBuffers(seccionesTerraplen, vertices, indices, normales);
            verticesTerraplen = vertices.ToArray<double>();
            normalesTerraplen = normales.ToArray<double>();
            indicesTerraplen = indices.ToArray<int>();

            // Riel 1
            this.BuildSurfaceDataBuffers(seccionesRieles1, vertices, indices, normales);
            verticesRieles1 = vertices.ToArray<double>();
            normalesRieles1 = normales.ToArray<double>();
            indicesRieles1 = indices.ToArray<int>();

            // Riel 2
            this.BuildSurfaceDataBuffers(seccionesRieles2, vertices, indices, normales);
            verticesRieles2 = vertices.ToArray<double>();
            normalesRieles2 = normales.ToArray<double>();
            indicesRieles2 = indices.ToArray<int>();

            // Tablas
            this.BuildTablasDataBuffers(seccionesTablas, vertices, indices, normales);
            verticesTablas = vertices.ToArray<double>();
            normalesTablas = normales.ToArray<double>();
            indicesTablas = indices.ToArray<int>();
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

            this.DrawTerraplen();
            this.DrawRieles();
            this.DrawTablas();

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
        private void DrawTablas()
        {
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesTablas);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesTablas);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 128f / 255f, 64f / 255f, 0.0f, 0.25f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 128f / 255f, 64f / 255f, 0.0f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glDrawElements(Gl.GL_QUADS, indicesTablas.Length, Gl.GL_UNSIGNED_INT, indicesTablas);
        }

        /// <summary>
        /// Dibuja la superficie correspondiente a los rieles tomando los valores
        /// previamente cargados en las listas de vertices, normales e índices.
        /// </summary>
        private void DrawRieles()
        {
            // Silver
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.19225f, 0.19225f, 0.19225f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.50754f, 0.50754f, 0.50754f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.508273f, 0.508273f, 0.508273f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 51.2f });

            // Polished Silver
            /*
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.23125f, 0.23125f, 0.23125f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2775f, 0.2775f, 0.2775f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.773911f, 0.773911f, 0.773911f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 0.896f });
            */

            // Chrome
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.25f, 0.25f, 0.25f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.4f, 0.4f, 0.4f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.774597f, 0.774597f, 0.774597f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 76.8f });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesRieles1);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesRieles1);
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indicesRieles1.Length, Gl.GL_UNSIGNED_INT, indicesRieles1);

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesRieles2);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesRieles2);
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indicesRieles2.Length, Gl.GL_UNSIGNED_INT, indicesRieles2);
        }

        /// <summary>
        /// Dibuja la superficie correspondiente al terraplen tomando los valores
        /// previamente cargados en las listas de vertices, normales e índices.
        /// </summary>
        private void DrawTerraplen()
        {
            texTierra.Activate();
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesTerraplen);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesTerraplen);
            //Gl.glTexCoordPointer(2, Gl.GL_DOUBLE, 2 * sizeof(double), texCoordTerraplen);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.6f, 0.5f, 0.35f, 0.25f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.61568f, 0.48627f, 0.34117f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });

            Gl.glDrawElements(Gl.GL_TRIANGLE_STRIP, indicesTerraplen.Length, Gl.GL_UNSIGNED_INT, indicesTerraplen);
        }

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

        private void DibujarNormales(double[] vertices, double[] normales)
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            for (int i = 0; i < vertices.Length; i += 3)
            {
                Gl.glPushMatrix();
                Gl.glTranslated(vertices[i], vertices[i + 1], vertices[i + 2]);
                Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3d(1, 0, 0);
                Gl.glVertex3d(0, 0, 0);
                Gl.glColor3d(0.2, 0, 0);
                Gl.glVertex3d(normales[i], normales[i + 1], normales[i + 2]);
                Gl.glEnd();
                Gl.glPopMatrix();
            }
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        public Punto GetPositionByDistancia(double distancia)
        {
            double distanciaDesdeElComienzoDeLaVuelta = distancia % distanciaAcumuladaPorPuntoPath[distanciaAcumuladaPorPuntoPath.Count - 1];

            for (int i = 0; i < path.Count; i++)
            {
                if (distanciaDesdeElComienzoDeLaVuelta <= distanciaAcumuladaPorPuntoPath[i])
                {
                    return path[i];
                }
            }

            throw new InvalidProgramException("No se encontró el punto donde supera la distancia");
        }

        private void LoadTextures()
        {
            this.texTierra = new Textura(@"../../Imagenes/Texturas/Tierra.bmp", true);
            this.texRiel = new Textura(@"../../Imagenes/Texturas/Riel.bmp", true);
        }

        internal double GetInclinacionByDistancia(double distancia)
        {
            double distanciaDesdeElComienzoDeLaVuelta = distancia % distanciaAcumuladaPorPuntoPath[distanciaAcumuladaPorPuntoPath.Count - 1];

            for (int i = 0; i < path.Count; i++)
            {
                if (distanciaDesdeElComienzoDeLaVuelta <= distanciaAcumuladaPorPuntoPath[i])
                {
                    return 180 + seccionesTerraplen[i].Angulo;
                }
            }

            throw new InvalidProgramException("No se encontró el punto donde supera la distancia");
        }
    }
}
