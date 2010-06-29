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
            PuntoFlotante[][] matriz = new PuntoFlotante[CANTIDAD_PIXELES_ALTO_IMAGEN][];
            int k = 0;

            for (int y = 0; y < CANTIDAD_PIXELES_ALTO_IMAGEN; y++)
            {
                matriz[y] = new PuntoFlotante[CANTIDAD_PIXELES_ANCHO_IMAGEN];
                for (int x = 0; x < CANTIDAD_PIXELES_ANCHO_IMAGEN; x++)
                {
                    double coordenadaX = (double)x * (double)X_MAX / (double)CANTIDAD_PIXELES_ANCHO_IMAGEN;
                    double coordenadaY = (double)y * (double)Y_MAX / (double)CANTIDAD_PIXELES_ALTO_IMAGEN;
                    double coordenadaZ = zetas[k++];

                    vertex.Add(coordenadaX);
                    vertex.Add(coordenadaY);
                    vertex.Add(coordenadaZ);

                    // Se ponen los puntos en una matriz para el cálculo de las normales
                    matriz[y][x] = new PuntoFlotante(coordenadaX, coordenadaY, coordenadaZ);
                }
            }

            // se calculan las normales en toda la matriz
            IList<double> normals = new List<double>();
            PuntoFlotante normal;
            for (int i = 0; i < CANTIDAD_PIXELES_ALTO_IMAGEN - 1; i++)
            {
                for (int j = 0; j < CANTIDAD_PIXELES_ANCHO_IMAGEN - 1; j++)
                {
                    normal = (matriz[i + 1][j] - matriz[i][j]) * (matriz[i][j + 1] - matriz[i][j]);
                    normals.Add(normal.X);
                    normals.Add(normal.Y);
                    normals.Add(normal.Z);
                }

                // última columna
                normal = (matriz[i + 1][CANTIDAD_PIXELES_ANCHO_IMAGEN - 1] - matriz[i][CANTIDAD_PIXELES_ANCHO_IMAGEN - 1]) * (matriz[i][CANTIDAD_PIXELES_ANCHO_IMAGEN - 2] - matriz[i][CANTIDAD_PIXELES_ANCHO_IMAGEN - 1]);
                normals.Add(normal.X);
                normals.Add(normal.Y);
                normals.Add(normal.Z);
            }

            // Las de la última fila se calculan con la fila anterior
            for (int j = 0; j < CANTIDAD_PIXELES_ANCHO_IMAGEN - 1; j++)
            {
                normal = (matriz[CANTIDAD_PIXELES_ALTO_IMAGEN - 2][j] - matriz[CANTIDAD_PIXELES_ALTO_IMAGEN - 1][j]) * (matriz[CANTIDAD_PIXELES_ALTO_IMAGEN - 1][j + 1] - matriz[CANTIDAD_PIXELES_ALTO_IMAGEN - 1][j]);
                normals.Add(normal.X);
                normals.Add(normal.Y);
                normals.Add(normal.Z);
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
                    index.Add(k + i + 1);
                    index.Add(k + i + cantidadPixelesAncho + 1);
                    index.Add(k + i + cantidadPixelesAncho);                    
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
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[] { 0.0f, 1.0f, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0, 1.0f, 0, 1 });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales);

            Gl.glDrawElements(Gl.GL_QUADS, indices.Length, Gl.GL_UNSIGNED_INT, indices);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
        }
    }
}
