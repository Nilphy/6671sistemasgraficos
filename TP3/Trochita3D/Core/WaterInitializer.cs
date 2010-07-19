using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Curvas;
using Trochita3D.Entidades;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class WaterInitializer
    {
        private static float ALTURA_PLANO_AGUA = 0.5f;
        private static double X_MAX = 180;
        private static double Y_MAX = 180;
        private static int CANTIDAD_PUNTOS_POR_EJE_X_Y = 2;

        private static int[] indices;
        private static double[] vertices;
        private static double[] normales;
        
        public WaterInitializer()
        {
            this.BuildPlaneOfWater();
        }

        private void BuildPlaneOfWater()
        {
            this.GenerarVerticesYNormales();

            this.GenerarIndices(CANTIDAD_PUNTOS_POR_EJE_X_Y * CANTIDAD_PUNTOS_POR_EJE_X_Y, CANTIDAD_PUNTOS_POR_EJE_X_Y, CANTIDAD_PUNTOS_POR_EJE_X_Y);
        }

        private void GenerarVerticesYNormales()
        {
            IList<double> vertex = new List<double>();
            Punto[][] matriz = new Punto[2][];

            for (int y = 0; y < CANTIDAD_PUNTOS_POR_EJE_X_Y; y++)
            {
                matriz[y] = new Punto[2];
                for (int x = 0; x < CANTIDAD_PUNTOS_POR_EJE_X_Y; x++)
                {
                    double coordenadaX = (double)x * (double)X_MAX;
                    double coordenadaY = (double)y * (double)Y_MAX;
                    double coordenadaZ = ALTURA_PLANO_AGUA;

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

            for (int i = 0; i < CANTIDAD_PUNTOS_POR_EJE_X_Y; i++)
            {
                for (int j = 0; j < CANTIDAD_PUNTOS_POR_EJE_X_Y; j++)
                {
                    puntoNorte = null;
                    puntoSur = null;
                    puntoEste = null;
                    puntoOeste = null;
                    if (i + 1 < CANTIDAD_PUNTOS_POR_EJE_X_Y - 1) puntoNorte = matriz[i + 1][j];
                    if (i - 1 > 0) puntoSur = matriz[i - 1][j];
                    if (j + 1 < CANTIDAD_PUNTOS_POR_EJE_X_Y - 1) puntoEste = matriz[i][j + 1];
                    if (j - 1 > 0) puntoOeste = matriz[i][j - 1];

                    normal = Punto.CalcularNormal(matriz[i][j], puntoNorte, puntoEste, puntoSur, puntoOeste, false);
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

        public void DrawPlaneOfWater()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glTranslated(-(double)X_MAX / (double)2, -(double)Y_MAX / (double)2, 0.0f);
            
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.60f, 0.70f, 0.95f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.0f, 0.0f, 0.9f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.0f, 0.0f, 0.9f, 1 });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales);

            Gl.glDrawElements(Gl.GL_QUADS, indices.Length, Gl.GL_UNSIGNED_INT, indices);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();
        }
    }
}
