using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Curvas;
using Trochita3D.Entidades;
using Tao.OpenGl;
using System.Drawing;

namespace Trochita3D.Core
{
    /// <summary>
    /// Clase encargada de inicializar la superficie inicial de la escena.
    /// </summary>
    public class TerrainInitializer
    {
        private const int ALTURA_MAXIMA = 2;
        private const double X_MAX = 180;
        private const double Y_MAX = 180;
        private const int MAX_VERTEX_X = 90;
        private const int MAX_VERTEX_Y = 90;

        private static int[] indices;
        private static double[] vertices;
        private static double[] normales;
        private static double[] textures;

        private Textura textura;
        
        public TerrainInitializer()
        {
            this.textura = new Textura(@"../../Imagenes/Texturas/Pasto.bmp", true);
            this.BuildTerrain();
        }

        private void BuildTerrain()
        {
            // Se procesa la imagen bmp
            Bitmap mapa = new Bitmap(@"../../Imagenes/Bitmap.bmp");
            this.GenerarVerticesYNormales(mapa);
            this.GenerarIndices(vertices.Length, MAX_VERTEX_Y, MAX_VERTEX_X);
        }

        private void GenerarVerticesYNormales(Bitmap mapa)
        {
            IList<double> vertex = new List<double>();
            Punto[][] matriz = new Punto[MAX_VERTEX_Y][];

            for (int y = 0; y < MAX_VERTEX_Y; y++)
            {
                matriz[y] = new Punto[MAX_VERTEX_X];
                for (int x = 0; x < MAX_VERTEX_X; x++)
                {
                    double coordenadaX = (double)x * (double)X_MAX / (double)MAX_VERTEX_X;
                    double coordenadaY = (double)y * (double)Y_MAX / (double)MAX_VERTEX_Y;
                    double coordenadaZ = mapa.GetPixel(x * (mapa.Width / MAX_VERTEX_X), y * (mapa.Height / MAX_VERTEX_Y)).B * ((double)ALTURA_MAXIMA / (double)255);

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

            for (int i = 0; i < MAX_VERTEX_Y; i++)
            {
                for (int j = 0; j < MAX_VERTEX_X; j++)
                {
                    puntoNorte = null;
                    puntoSur = null;
                    puntoEste = null;
                    puntoOeste = null;
                    if (i + 1 < MAX_VERTEX_Y - 1) puntoNorte = matriz[i + 1][j];
                    if (i - 1 > 0) puntoSur = matriz[i - 1][j];
                    if (j + 1 < MAX_VERTEX_X - 1) puntoEste = matriz[i][j + 1];
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
            IList<double> texturesCoord = new List<double>();

            for (int i = 0; i < ((cantidadPixelesAlto - 1) * cantidadPixelesAncho); i += cantidadPixelesAncho)
            {
                for (int k = 0; k < cantidadPixelesAncho - 1; k++)
                {
                    index.Add(k + i);
                    index.Add(k + i + cantidadPixelesAncho);
                    index.Add(k + i + cantidadPixelesAncho + 1);
                    index.Add(k + i + 1);

                    texturesCoord.Add(0);
                    texturesCoord.Add(0);
                    texturesCoord.Add(1);
                    texturesCoord.Add(0);
                    texturesCoord.Add(1);
                    texturesCoord.Add(1);
                    texturesCoord.Add(0);
                    texturesCoord.Add(1);
                }
            }

            indices = index.ToArray<int>();
            textures = texturesCoord.ToArray<double>();
        }

        public void DrawTerrain()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glTranslated(-(double)X_MAX / (double)2, -(double)Y_MAX / (double)2, 0.0f);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, new float[] { .5f, .5f, .5f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.0f, 0.0f, 0.0f, 1 });

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            textura.Activate();
            
            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), vertices);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), normales);
            Gl.glTexCoordPointer(2, Gl.GL_DOUBLE, 2 * sizeof(double), textures);

            Gl.glDrawElements(Gl.GL_QUADS, indices.Length, Gl.GL_UNSIGNED_INT, indices);

            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_LIGHTING);

            //this.DibujarNormales(vertices, normales);

            Gl.glPopMatrix();
        }

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
        
    }
}
