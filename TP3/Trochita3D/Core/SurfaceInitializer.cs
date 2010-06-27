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
        private const int CANT_PUNTOS_TERRAPLEN = 10;

        private IList<Seccion> seccionesTerraplen = new List<Seccion>();
        private IList<Seccion> seccionesRieles1 = new List<Seccion>();
        private IList<Seccion> seccionesRieles2 = new List<Seccion>();
        private IList<PuntoFlotante> path;

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
        
        /*
        private void BuildTerraplen()
        {
            Terraplen terraplen = new Terraplen();
            PuntoFlotante puntoAnterior = path[path.Count - 1];
            PuntoFlotante puntoActual;
            Seccion seccion;
            seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);
            seccion.Rotar(Math.Atan(puntoAnterior.Y / puntoAnterior.X));
            seccion.Trasladar(puntoAnterior.X, puntoAnterior.Y, puntoAnterior.Z);
            seccionesTerraplen.Add(seccion);

            double dx, dy, angulo;

            for (int i = 0; i < path.Count; i++)
            {
                puntoActual = path[i];
                seccion = new Seccion(seccion.Vertices);

                double anguloAnterior = seccionesTerraplen.Last<Seccion>().Angulo;
                double anguloActual = Math.Atan(puntoActual.Y / puntoActual.X);

                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;

                angulo = dy / dx;

                seccion.Trasladar(-puntoAnterior.X, -puntoAnterior.Y, -puntoAnterior.Z);
                seccion.Rotar(Math.Atan(angulo));
                //seccion.Rotar(anguloAnterior);
                //seccion.Rotar(anguloActual - anguloAnterior);
                seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);

                seccionesTerraplen.Add(seccion);
                //seccion = new Seccion(seccion.Vertices);
            }
        }
        */
        
        private void BuildTerraplen()
        {
            Terraplen terraplen = new Terraplen();
            PuntoFlotante puntoAnterior = path[path.Count - 1];
            PuntoFlotante puntoActual;
            Seccion seccion;
            seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);
            double dx, dy, angulo;

            for (int i = 0; i < path.Count; i++)
            {
                puntoActual = path[i];
                seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);

                double anguloAnterior = Math.Atan(puntoAnterior.Y / puntoAnterior.X);
                double anguloActual = Math.Atan(puntoActual.Y / puntoActual.X);

                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;

                angulo = dy / dx;
                seccion.Rotar(Math.Atan(angulo));
                //seccion.Rotar(anguloActual - anguloAnterior);
                seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);

                seccionesTerraplen.Add(seccion);
                puntoAnterior = puntoActual;
                seccion = new Seccion(seccion.Vertices);
            }
        }

        private void BuildRieles()
        {
            const double DIST_RIELES = 0.4;
            Riel riel = new Riel();
            PuntoFlotante puntoAnterior = path[path.Count - 1];
            PuntoFlotante puntoActual;
            Seccion seccionRiel1;
            Seccion seccionRiel2;
            double dx, dy, angulo;

            for (int i = 0; i < path.Count; i++)
            {
                puntoActual = path[i];
                seccionRiel1 = riel.GetSeccion();
                seccionRiel2 = riel.GetSeccion();

                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;

                angulo = dy / dx;
                seccionRiel1.Escalar(1, 0.2, 0.2);
                seccionRiel1.Trasladar(0, -DIST_RIELES, 0);
                seccionRiel1.Rotar(Math.Atan(angulo));
                seccionRiel1.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + Terraplen.ALTURA);

                seccionRiel2.Escalar(1, 0.2, 0.2);
                seccionRiel2.Trasladar(0, DIST_RIELES, 0);
                seccionRiel2.Rotar(Math.Atan(angulo));
                seccionRiel2.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + Terraplen.ALTURA);

                seccionesRieles1.Add(seccionRiel1);
                seccionesRieles2.Add(seccionRiel2);
                puntoAnterior = puntoActual;
            }
        }

        /// <summary>
        /// Obtiene los puntos discretos del trayecto a realizar por el terraplen
        /// a partir de los puntos de control definidos en esta misma funcion.
        /// </summary>
        /// <param name="du">Delta U</param>
        /// <returns>
        /// Lista de vertices que corresponden a la curva que representa el trayecto.
        /// </returns>
        private IList<PuntoFlotante> GetBsplineControlPoints(double du)
        {
            IList<PuntoFlotante> ptsControl = new List<PuntoFlotante>();

            ptsControl.Add(new PuntoFlotante(10, 10, 0));
            ptsControl.Add(new PuntoFlotante(10, -10, 0));
            ptsControl.Add(new PuntoFlotante(-10, -10, 0));
            ptsControl.Add(new PuntoFlotante(-10, 10, 0));

            CurvaBsplineSegmentosCubicos path = new CurvaBsplineSegmentosCubicos(ptsControl);

            return path.GetPuntosDiscretos(du);
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
            Seccion seccionSiguiente;
            Seccion seccionAnterior;
            int indexCount = 0;
            vertices.Clear();
            indices.Clear();
            normales.Clear();

            // Armo la lista de vertices e indices. Esta ultima pensada que se arma con TRIANGLE.
            for (int i = 0; i < secciones.Count; i++)
            {
                seccion = secciones[i];
                //seccion.ReordenarVertices();

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
                    seccionAnterior = this.GetSeccionAnterior(secciones, i);
                    seccionSiguiente = this.GetSeccionSiguiente(secciones, i);
                    PuntoFlotante normal = this.GetNormalForVertex(seccion, seccionAnterior, seccionSiguiente, j);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);
                }
            }
        }

        private PuntoFlotante GetNormalForVertex(Seccion seccion, Seccion seccionAnterior, Seccion seccionSiguiente, int vertexPos)
        {
            PuntoFlotante n1 = null, n2 = null, n3 = null, n4 = null;

            // Planos inferiores
            if (vertexPos > 0)
            {
                n1 = GetNormalInferior(seccion, seccionAnterior, vertexPos);
                n2 = GetNormalInferior(seccion, seccionSiguiente, vertexPos);
                n1.ZPositivo();
                n2.ZPositivo();
            }

            if (vertexPos < seccion.Vertices.Count - 1)
            {
                n3 = GetNormalSuperior(seccion, seccionAnterior, vertexPos);
                n4 = GetNormalSuperior(seccion, seccionSiguiente, vertexPos);
                n3.ZPositivo();
                n4.ZPositivo();
            }

            PuntoFlotante n13 = n1 + n3;
            PuntoFlotante n24 = n2 + n4;
            PuntoFlotante n = n13 + n24;
            n.Normalizar();
            return n;
        }

        private PuntoFlotante GetNormalInferior(Seccion seccion, Seccion seccion2, int vertexPos)
        {
            PuntoFlotante v1 = new PuntoFlotante(seccion2.Vertices[vertexPos].X - seccion.Vertices[vertexPos].X,
                                                 seccion2.Vertices[vertexPos].Y - seccion.Vertices[vertexPos].Y,
                                                 seccion2.Vertices[vertexPos].Z - seccion.Vertices[vertexPos].Z);
            PuntoFlotante v2 = new PuntoFlotante(seccion.Vertices[vertexPos - 1].X - seccion.Vertices[vertexPos].X,
                                                 seccion.Vertices[vertexPos - 1].Y - seccion.Vertices[vertexPos].Y,
                                                 seccion.Vertices[vertexPos - 1].Z - seccion.Vertices[vertexPos].Z);
            return v1 * v2;
        }

        private PuntoFlotante GetNormalSuperior(Seccion seccion, Seccion seccion2, int vertexPos)
        {
            PuntoFlotante v1 = new PuntoFlotante(seccion2.Vertices[vertexPos].X - seccion.Vertices[vertexPos].X,
                                                 seccion2.Vertices[vertexPos].Y - seccion.Vertices[vertexPos].Y,
                                                 seccion2.Vertices[vertexPos].Z - seccion.Vertices[vertexPos].Z);
            PuntoFlotante v2 = new PuntoFlotante(seccion.Vertices[vertexPos + 1].X - seccion.Vertices[vertexPos].X,
                                                 seccion.Vertices[vertexPos + 1].Y - seccion.Vertices[vertexPos].Y,
                                                 seccion.Vertices[vertexPos + 1].Z - seccion.Vertices[vertexPos].Z);
            return v1 * v2;
        }

        private Seccion GetSeccionAnterior(IList<Seccion> secciones, int posSeccionActual)
        {
            if (posSeccionActual == 0)
                return secciones[secciones.Count - 1];
            else
                return secciones[posSeccionActual - 1];
        }

        private Seccion GetSeccionSiguiente(IList<Seccion> secciones, int posSeccionActual)
        {
            if (posSeccionActual == (secciones.Count - 1))
                return secciones[0];
            else
                return secciones[posSeccionActual + 1];
        }

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

    }
}
