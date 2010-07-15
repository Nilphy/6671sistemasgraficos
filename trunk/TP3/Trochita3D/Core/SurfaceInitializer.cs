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
        private const double DELTA_U = 0.0005;
        private const int CANT_PUNTOS_TERRAPLEN = 10;

        private IList<Seccion> seccionesTerraplen = new List<Seccion>();
        private IList<Seccion> seccionesRieles1 = new List<Seccion>();
        private IList<Seccion> seccionesRieles2 = new List<Seccion>();
        private IList<Punto> path;
        private IList<double> distanciaAcumuladaPorPuntoPath;

        private static int[] indicesTerraplen;
        private static int[] indicesRieles1;
        private static int[] indicesRieles2;

        private static double[] verticesTerraplen;
        private static double[] verticesRieles1;
        private static double[] verticesRieles2;

        private static double[] normalesTerraplen;
        private static double[] normalesRieles1;
        private static double[] normalesRieles2;

        public SurfaceInitializer()
        {
            this.path = GetBsplineControlPoints(DELTA_U);
            this.BuildTerraplen();
            this.BuildRieles();
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
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

            ptsControl.Add(new Punto(30, 30, 0));
            ptsControl.Add(new Punto(30, -30, 0));
            ptsControl.Add(new Punto(-30, -30, 0));
            ptsControl.Add(new Punto(-30, 30, 0));

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
            Terraplen terraplen = new Terraplen();
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
            const double DIST_RIELES = 0.4;
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
                seccionRiel1.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + Terraplen.ALTURA);

                seccionRiel2.Escalar(1, 0.2, 0.2);
                seccionRiel2.Trasladar(0, DIST_RIELES, 0);
                seccionRiel2.Rotar(angulo);
                seccionRiel2.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + Terraplen.ALTURA);

                seccionesRieles1.Add(seccionRiel1);
                seccionesRieles2.Add(seccionRiel2);
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

        #endregion

        #region Dibujo

        public void DrawSurface()
        {
            Gl.glEnable(Gl.GL_LIGHTING);

            this.DrawTerraplen();
            this.DrawRieles();

            Gl.glDisable(Gl.GL_LIGHTING);
        }

        /// <summary>
        /// Dibuja la superficie correspondiente a los rieles tomando los valores
        /// previamente cargados en las listas de vertices, normales e índices.
        /// </summary>
        private void DrawRieles()
        {
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesRieles1);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesRieles1);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.25f, 0.25f, 0.25f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.7f, 0.7f, 0.7f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 1, 1, 1, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 1.75f });
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indicesRieles1.Length, Gl.GL_UNSIGNED_INT, indicesRieles1);

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesRieles2);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesRieles2);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.25f, 0.25f, 0.25f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.7f, 0.7f, 0.7f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indicesRieles2.Length, Gl.GL_UNSIGNED_INT, indicesRieles2);
        }

        /// <summary>
        /// Dibuja la superficie correspondiente al terraplen tomando los valores
        /// previamente cargados en las listas de vertices, normales e índices.
        /// </summary>
        private void DrawTerraplen()
        {
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), verticesTerraplen);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normalesTerraplen);
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
    }
}
