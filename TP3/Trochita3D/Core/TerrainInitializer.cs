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
    public class TerrainInitializer
    {
        private static int ALTURA_MAXIMA = 2;
        private static double X_MAX = 30;
        private static double Y_MAX = 30;
        private static int CANTIDAD_PIXELES_ANCHO_IMAGEN = 512;
        private static int CANTIDAD_PIXELES_ALTO_IMAGEN = 384;

        private static int[] indices;
        private static double[] vertices;
        private static double[] normales;
        
        public TerrainInitializer()
        {
            this.BuildTerrain();
        }

        private void BuildTerrain()
        {
            // Se procesa la imagen bmp
            byte[] bytesDeLaImagen = BMPUtils.ObtenerBytesDeArchivo(@"../../Imagenes/Bitmap.bmp");
            byte[] bytesUtiles = BMPUtils.ObtenerBytesUtilesDelArchivo(bytesDeLaImagen);
            double[] bytesDeLaImagenEscalados = BMPUtils.EscalarBytes(ALTURA_MAXIMA, bytesUtiles);
            
            IList<double> zetas = new List<double>(bytesDeLaImagenEscalados);
            
            this.GenerarVerticesYNormales(zetas);

            this.GenerarIndices(zetas.Count, CANTIDAD_PIXELES_ALTO_IMAGEN, CANTIDAD_PIXELES_ANCHO_IMAGEN);
        }

        private void GenerarVerticesYNormales(IList<double> zetas)
        {
            IList<double> vertex = new List<double>();
            Punto[][] matriz = new Punto[CANTIDAD_PIXELES_ALTO_IMAGEN][];
            int k = 0;

            for (int y = 0; y < CANTIDAD_PIXELES_ALTO_IMAGEN; y++)
            {
                matriz[y] = new Punto[CANTIDAD_PIXELES_ANCHO_IMAGEN];
                for (int x = 0; x < CANTIDAD_PIXELES_ANCHO_IMAGEN; x++)
                {
                    double coordenadaX = (double)x * (double)X_MAX / (double)CANTIDAD_PIXELES_ANCHO_IMAGEN;
                    double coordenadaY = (double)y * (double)Y_MAX / (double)CANTIDAD_PIXELES_ALTO_IMAGEN;
                    double coordenadaZ = zetas[k++];

                    vertex.Add(coordenadaX);
                    vertex.Add(coordenadaY);
                    vertex.Add(coordenadaZ);

                    // Se ponen los puntos en una matriz para el cálculo de las normales
                    matriz[y][x] = new Punto(coordenadaX, coordenadaY, coordenadaZ);
                }
            }

            // se calculan las normales en toda la matriz
            IList<double> normals = new List<double>();
            Punto normal;
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;

            for (int i = 0; i < CANTIDAD_PIXELES_ALTO_IMAGEN; i++)
            {
                for (int j = 0; j < CANTIDAD_PIXELES_ANCHO_IMAGEN; j++)
                {
                    puntoNorte = null;
                    puntoSur = null;
                    puntoEste = null;
                    puntoOeste = null;
                    if (i + 1 < CANTIDAD_PIXELES_ALTO_IMAGEN - 1) puntoNorte = matriz[i + 1][j];
                    if (i - 1 > 0) puntoSur = matriz[i - 1][j];
                    if (j + 1 < CANTIDAD_PIXELES_ANCHO_IMAGEN - 1) puntoEste = matriz[i][j + 1];
                    if (j - 1 > 0) puntoOeste = matriz[i][j - 1];

                    normal = this.CalcularNormal(matriz[i][j], puntoNorte, puntoEste, puntoSur, puntoOeste);
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);
                }
            }

            vertices = vertex.ToArray<double>();
            normales = normals.ToArray<double>();
        }

        private void GenerarIndices(int cantidadTotalPixeles, int cantidadPixelesAlto, int cantidadPixelesAncho)
        {
            IList<int> index = new List<int>();

            for (int i = 0; i < ((cantidadPixelesAlto - 1) * cantidadPixelesAncho); i += cantidadPixelesAncho)
            {
                for (int k = 0; k < cantidadPixelesAncho - 1; k++)
                {
                    index.Add(k + i);
                    index.Add(k + i + cantidadPixelesAncho);
                    index.Add(k + i + cantidadPixelesAncho + 1);
                    index.Add(k + i + 1);
                }
            }

            indices = index.ToArray<int>();
        }

        public void DrawTerrain()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glTranslated(-(double)X_MAX / (double)2, -(double)Y_MAX / (double)2, 0.0f);
            
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, new float[] { 0.5f, 0.5f, 0.5f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[] { 0.1f, 0.3f, 0.15f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0.1f, 0.3f, 0.15f, 1 });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales);

            Gl.glDrawElements(Gl.GL_QUADS, indices.Length, Gl.GL_UNSIGNED_INT, indices);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
        }

        /// <summary>
        /// Promedia las normales calculadas con todos los puntos de alrededor
        /// </summary>
        /// <param name="verticeCentro"></param>
        /// <param name="verticeNorte"></param>
        /// <param name="verticeEste"></param>
        /// <param name="verticeSur"></param>
        /// <param name="verticeOeste"></param>
        /// <returns></returns>
        private Punto CalcularNormal(Punto verticeCentro, Punto verticeNorte, Punto verticeEste, Punto verticeSur, Punto verticeOeste)
        {
            if (verticeCentro == null) throw new InvalidOperationException("Este método no puede ser invocado con el vertice central nulo");

            // Numeradas en sentido horario son 4
            Punto normalNorEste = null;
            Punto normalSurEste = null;
            Punto normalSurOeste = null;
            Punto normalNorOeste = null;
            Punto normalRetorno = null;

            if (verticeNorte != null && verticeEste != null)
                normalNorEste = (verticeEste - verticeCentro) * (verticeNorte - verticeCentro);
            
            
            if (verticeSur != null && verticeEste != null)
                normalSurEste = (verticeSur - verticeCentro) * (verticeEste - verticeCentro);

            if (verticeSur != null && verticeOeste != null)
                normalSurOeste = (verticeOeste - verticeCentro) * (verticeSur - verticeCentro);

            if (verticeNorte != null && verticeOeste != null)
                normalNorOeste = (verticeNorte - verticeCentro) * (verticeOeste - verticeCentro);

            normalRetorno = new Punto(0, 0, 0);

            if (normalNorEste != null)
                normalRetorno = normalRetorno.SumarPunto(normalNorEste);

            if (normalNorOeste != null)
                normalRetorno = normalRetorno.SumarPunto(normalNorOeste);

            if (normalSurEste != null)
                normalRetorno = normalRetorno.SumarPunto(normalSurEste);

            if (normalSurOeste != null)
                normalRetorno = normalRetorno.SumarPunto(normalSurOeste);

            return normalRetorno;
        }
    }
}
