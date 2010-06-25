﻿using System;
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
        private static int ALTURA_MAXIMA = 1;
        private static double X_MAX = 25;
        private static double Y_MAX = 25;
        private static int CANTIDAD_PIXELES_ANCHO_IMAGEN = 512;
        private static int CANTIDAD_PIXELES_ALTO_IMAGEN = 384;

        private IList<int> indices = new List<int>();
        private IList<double> vertices = new List<double>();
        private IList<double> normales = new List<double>();
        
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
            
            this.GenerarVerticesYNormales(zetas);

            this.GenerarIndices(bytesDeLaImagenEscalados.Length, CANTIDAD_PIXELES_ALTO_IMAGEN, CANTIDAD_PIXELES_ANCHO_IMAGEN);

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices.ToArray<double>());
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales.ToArray<double>());
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }

        private void GenerarVerticesYNormales(IList<double> zetas)
        {
            this.vertices = new List<double>();
            PuntoFlotante[][] matriz = new PuntoFlotante[CANTIDAD_PIXELES_ALTO_IMAGEN][];
            int k = 0;

            for (int y = CANTIDAD_PIXELES_ALTO_IMAGEN - 1; y >= 0; y--)
            {
                matriz[y] = new PuntoFlotante[CANTIDAD_PIXELES_ANCHO_IMAGEN];
                for (int x = 0; x < CANTIDAD_PIXELES_ANCHO_IMAGEN; x++)
                {
                    double coordenadaX = (double)x * (double)X_MAX / (double)CANTIDAD_PIXELES_ANCHO_IMAGEN;
                    double coordenadaY = (double)y * (double)Y_MAX / (double)CANTIDAD_PIXELES_ALTO_IMAGEN;
                    double coordenadaZ = zetas[k++];
                        
                    this.vertices.Add(coordenadaX);
                    this.vertices.Add(coordenadaY);
                    this.vertices.Add(coordenadaZ);

                    matriz[y][x] = new PuntoFlotante(coordenadaX, coordenadaY, coordenadaZ);
                }
            }

            // calculo las normales en toda la matriz
            for (int i = CANTIDAD_PIXELES_ALTO_IMAGEN -2 ; i <= 0; i--)
            {
                for (int j = 0; j < CANTIDAD_PIXELES_ANCHO_IMAGEN - 1; j++)
                {
                    PuntoFlotante normal = (matriz[i + 1][j] - matriz[i][j]) * (matriz[i][j + 1] - matriz[i][j]);
                    this.normales.Add(normal.X);
                    this.normales.Add(normal.Y);
                    this.normales.Add(normal.Z);
                }
            }
            
            
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
            
            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, new float[] { 0.3f, 0.3f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[] { 0.0f, 1.0f, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glDrawElements(Gl.GL_QUADS, indices.Count, Gl.GL_UNSIGNED_INT, indices.ToArray<int>());
            Gl.glDisable(Gl.GL_LIGHTING);
        }
    }
}
