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
        private static int ALTURA_MAXIMA = 8;
        private static double X_MAX = 25;
        private static double Y_MAX = 25;
        private static int CANTIDAD_PIXELES_ANCHO_IMAGEN = 512;
        private static int CANTIDAD_PIXELES_ALTO_IMAGEN = 384;

        private IList<int> indices = new List<int>();
        private IList<double> vertices = new List<double>();
        

        public TerrainInitializer()
        {
            this.BuildTerrain();

            // TODO: sacar esto de acá es una trampa mortal¿?¿?
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_AUTO_NORMAL);
        }

        private void BuildTerrain()
        {
            // Se procesa la imagen bmp
            byte[] bytesDeLaImagen = BMPUtils.ObtenerBytesDeArchivo(@"../../Imagenes/Bitmap.bmp");
            byte[] bytesUtiles = BMPUtils.ObtenerBytesUtilesDelArchivo(bytesDeLaImagen);
            double[] bytesDeLaImagenEscalados = BMPUtils.EscalarBytes(ALTURA_MAXIMA, bytesDeLaImagen);
            
            IList<double> zetas = new List<double>(bytesDeLaImagenEscalados);

            this.vertices = this.CompletarVerticesConXY(zetas);

            // Se genera la lista de índices
            this.GenerarIndices(bytesDeLaImagenEscalados.Length, CANTIDAD_PIXELES_ALTO_IMAGEN, CANTIDAD_PIXELES_ANCHO_IMAGEN);

            // Se genera la lista de normales
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices.ToArray<double>());
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }

        private IList<double> CompletarVerticesConXY(IList<double> zetas)
        {
            IList<double> vertices = new List<double>();
            int i = 0;

            for (int y = CANTIDAD_PIXELES_ALTO_IMAGEN - 1; y >= 0; y--)
            {
                for (int x = 0; x < CANTIDAD_PIXELES_ANCHO_IMAGEN; x++)
                {
                    vertices.Add((double)x * (double)X_MAX / (double)CANTIDAD_PIXELES_ANCHO_IMAGEN);
                    vertices.Add((double)y * (double)Y_MAX / (double)CANTIDAD_PIXELES_ALTO_IMAGEN);
                    vertices.Add(zetas[i++]);
                }
            }

            return vertices;
        }

        private void GenerarIndices(int cantidadTotalPixeles, int cantidadPixelesAlto, int cantidadPixelesAncho)
        {
            this.indices = new List<int>();

            for (int i = 0; i < ((cantidadPixelesAlto - 1) * cantidadPixelesAncho); i += cantidadPixelesAncho)
            {
                for (int k = 0; k < cantidadPixelesAncho - 1; k++)
                {
                    indices.Add(k + i);
                    indices.Add(k + i + 1);
                    indices.Add(k + i + cantidadPixelesAncho + 1);
                    indices.Add(k + i + cantidadPixelesAncho);                    
                }
            }
        }

        public void DrawTerrain()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glTranslated(-(double)X_MAX / (double)2, -(double)Y_MAX / (double)2, 0.0f);

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glColor3d(1, 0, 0);
            Gl.glDrawElements(Gl.GL_QUAD_STRIP, indices.Count, Gl.GL_UNSIGNED_INT, indices.ToArray<int>());
            Gl.glDisable(Gl.GL_LIGHTING);
        }
    }
}
