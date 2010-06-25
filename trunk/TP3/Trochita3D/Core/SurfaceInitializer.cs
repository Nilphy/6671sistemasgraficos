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
        private IList<Seccion> seccionesRieles = new List<Seccion>();
        private IList<PuntoFlotante> path;

        private IList<int> indices = new List<int>();
        private IList<double> vertices = new List<double>();
        private IList<double> normales = new List<double>();

        public SurfaceInitializer()
        {
            this.path = GetBsplineControlPoints(DELTA_U);
            this.BuildTerraplen();
            this.BuildRieles();

            // TODO: eliminar de aca para abajo.
            this.BuildSurface();
            //Gl.glEnable(Gl.GL_NORMALIZE);
            //Gl.glEnable(Gl.GL_AUTO_NORMAL);
        }

        private void BuildTerraplen()
        {
            Terraplen terraplen = new Terraplen();
            PuntoFlotante puntoAnterior = path[0];
            PuntoFlotante puntoActual;
            Seccion seccion;
            double dx, dy, angulo;

            for (int i = 1; i < path.Count; i++)
            {
                puntoActual = path[i];
                seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);

                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;

                angulo = dy / dx;
                seccion.Rotar(Math.Atan(angulo));
                seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);

                seccionesTerraplen.Add(seccion);
                puntoAnterior = puntoActual;
            }

            // Crea la ultima seccion.
            puntoActual = path[0];
            seccion = terraplen.GetSeccion(CANT_PUNTOS_TERRAPLEN);

            dx = puntoActual.X - puntoAnterior.X;
            dy = puntoActual.Y - puntoAnterior.Y;

            angulo = dy / dx;
            seccion.Rotar(Math.Atan(angulo));
            seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);

            seccionesTerraplen.Add(seccion);
        }

        private void BuildRieles()
        {
            const double DIST_RIELES = 0.4;
            Riel riel = new Riel();
            PuntoFlotante puntoAnterior = path[0];
            PuntoFlotante puntoActual;
            Seccion seccionRiel1;
            Seccion seccionRiel2;
            double dx, dy, angulo;

            for (int i = 1; i < path.Count; i++)
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

                seccionesRieles.Add(seccionRiel1);
                seccionesRieles.Add(seccionRiel2);
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

        public void BuildSurface()
        {
            Seccion seccion;
            Seccion seccionSiguiente;
            Seccion seccionAnterior;
            int indexCount = 0;

            // Armo la lista de vertices e indices. Esta ultima pensada que se arma con TRIANGLE.
            for (int i = 0; i < seccionesTerraplen.Count; i++)
            {
                seccion = seccionesTerraplen[i];
                seccion.ReordenarVertices();

                for (int j = 0; j < seccion.Vertices.Count; j++)
                {
                    // Vertices
                    vertices.Add(seccion.Vertices[j].X);
                    vertices.Add(seccion.Vertices[j].Y);
                    vertices.Add(seccion.Vertices[j].Z);

                    // Indices
                    indices.Add(indexCount);
                    indices.Add((indexCount + seccion.Vertices.Count) % (seccionesTerraplen.Count * seccion.Vertices.Count));
                    indexCount++;

                    // Normales
                    seccionAnterior = this.GetSeccionAnterior(seccionesTerraplen, i);
                    seccionSiguiente = this.GetSeccionSiguiente(seccionesTerraplen, i);
                    PuntoFlotante normal = this.GetNormalForVertex(seccion, seccionAnterior, seccionSiguiente, j);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);
                }
            }

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices.ToArray<double>());
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales.ToArray<double>());
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
        }

        private PuntoFlotante GetNormalForVertex(Seccion seccion, Seccion seccionAnterior, Seccion seccionSiguiente, int vertexPos)
        {
            PuntoFlotante n1 = null, n2 = null, n3 = null, n4 = null;

            // Planos inferiores
            if (vertexPos > 0)
            {
                n1 = GetNormalInferior(seccion, seccionAnterior, vertexPos);
                n2 = GetNormalInferior(seccion, seccionSiguiente, vertexPos);
            }

            if (vertexPos < seccion.Vertices.Count - 1)
            {
                n3 = GetNormalSuperior(seccion, seccionAnterior, vertexPos);
                n4 = GetNormalSuperior(seccion, seccionSiguiente, vertexPos);
            }

            PuntoFlotante n13 = n1 + n3;
            PuntoFlotante n24 = n2 + n4;
            PuntoFlotante n = n13 + n24;

            n.Normalizar();

            if (n.Z < 0)
                n = n * -1;

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
            //Gl.glColor3d(1, 0, 0);
            //Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.3f, 0, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 1.0f, 0, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indices.Count, Gl.GL_UNSIGNED_INT, indices.ToArray<int>());
            Gl.glDisable(Gl.GL_LIGHTING);
        }

        public void Dibujar()
        {
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glColor3d(1, 0, 0);
            foreach (PuntoFlotante punto in GetBsplineControlPoints(DELTA_U))
            {
                Gl.glVertex3d(punto.X, punto.Y, punto.Z);
            }
            Gl.glEnd();
            
            Gl.glColor3d(0, 0, 1);
            foreach (Seccion seccion in seccionesTerraplen)
            {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                foreach (PuntoFlotante punto in seccion.Vertices)
                {
                    Gl.glVertex3d(punto.X, punto.Y, punto.Z);
                }
                Gl.glEnd();
            }

            Gl.glColor3d(0, 1, 0);
            foreach (Seccion seccion in seccionesRieles)
            {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                foreach (PuntoFlotante punto in seccion.Vertices)
                {
                    Gl.glVertex3d(punto.X, punto.Y, punto.Z);
                }
                Gl.glEnd();
            }
        }

    }
}
