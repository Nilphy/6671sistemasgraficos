using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class Train
    {
        #region Atributos y propiedades

        private IList<Punto> verticesRectanguloConductor;
        private double[] pVerticesRectanguloConductor;
        private IList<int> indicesRectanguloConductor;
        private int[] pIndicesRectanguloConductor;
        private IList<Punto> normalesRectanguloConductor;
        private double[] pNormalesRectanguloConductor;
        private double ANCHO_RECTANGULO = 2d;
        private double LARGO_RECTANULO = 1.5d;
        private double ALTO_RECTANGULO = 3.5d;
        private double RADIO_TROMPA = 1d;
        private double LARGO_TROMPA = 3d;
        private double ANCHO_BASE = 2.5d;
        private double ALTO_BASE = 0.3d;

        #endregion

        /// <summary>
        /// Se debe construir estáticamente porque acá se crea la 
        /// display list, despues se va redibujando en el paint
        /// en cada simulación
        /// </summary>
        public Train()
        {
            this.ConstruirRectanguloConductor();
            this.ConvertirListasAvectores();
        }

        public void Draw()
        {
            // Cuadrado del conductor
            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.20f, 0.20f, 0.35f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.2f, 0.2f, 0.3f, 1 });

            Gl.glVertexPointer(3, Gl.GL_DOUBLE, 3 * sizeof(double), pVerticesRectanguloConductor);
            Gl.glNormalPointer(Gl.GL_DOUBLE, 3 * sizeof(double), pNormalesRectanguloConductor);

            Gl.glDrawElements(Gl.GL_QUADS, pIndicesRectanguloConductor.Length, Gl.GL_UNSIGNED_INT, pIndicesRectanguloConductor);
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();

            // Cilindro
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();

            Gl.glTranslated(0, LARGO_RECTANULO + LARGO_TROMPA / 2d, RADIO_TROMPA);
            Gl.glRotated(90, 1, 0, 0);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.20f, 0.20f, 0.35f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.2f, 0.2f, 0.3f, 1 });

            Glu.gluCylinder(quad, RADIO_TROMPA, RADIO_TROMPA, LARGO_TROMPA, 20, 20);            
            Glu.gluDisk(quad, 0, RADIO_TROMPA, 20, 20);

            Glu.gluDeleteQuadric(quad);                        
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }

        #region utilitarios

        private void ConvertirListasAvectores()
        {
            int i = 0;
            pVerticesRectanguloConductor = new double[verticesRectanguloConductor.Count * 3];
            foreach (Punto vertice in verticesRectanguloConductor)
            {
                pVerticesRectanguloConductor[i++] = vertice.X;
                pVerticesRectanguloConductor[i++] = vertice.Y;
                pVerticesRectanguloConductor[i++] = vertice.Z;
            }

            i = 0;
            pNormalesRectanguloConductor = new double[normalesRectanguloConductor.Count * 3];
            foreach (Punto normal in normalesRectanguloConductor)
            {
                pNormalesRectanguloConductor[i++] = normal.X;
                pNormalesRectanguloConductor[i++] = normal.Y;
                pNormalesRectanguloConductor[i++] = normal.Z;
            }
            
            pIndicesRectanguloConductor = indicesRectanguloConductor.ToArray<int>();
        }

        #endregion
        #region Constructores de vertices, indices y matrices 
        
        private void ConstruirRectanguloConductor()
        {
            this.verticesRectanguloConductor = new List<Punto>();

            this.verticesRectanguloConductor.Add(new Punto(ANCHO_RECTANGULO / 2d, -LARGO_RECTANULO / 2d, 0));
            this.verticesRectanguloConductor.Add(new Punto(ANCHO_RECTANGULO / 2d, -LARGO_RECTANULO/2d, ALTO_RECTANGULO));
            this.verticesRectanguloConductor.Add(new Punto(-ANCHO_RECTANGULO / 2d, -LARGO_RECTANULO / 2d, ALTO_RECTANGULO));
            this.verticesRectanguloConductor.Add(new Punto(-ANCHO_RECTANGULO / 2d, -LARGO_RECTANULO / 2d, 0));
            this.verticesRectanguloConductor.Add(new Punto(ANCHO_RECTANGULO / 2d, LARGO_RECTANULO / 2d, 0));
            this.verticesRectanguloConductor.Add(new Punto(ANCHO_RECTANGULO / 2d, LARGO_RECTANULO / 2d, ALTO_RECTANGULO));
            this.verticesRectanguloConductor.Add(new Punto(-ANCHO_RECTANGULO / 2d, LARGO_RECTANULO / 2d, ALTO_RECTANGULO));
            this.verticesRectanguloConductor.Add(new Punto(-ANCHO_RECTANGULO / 2d, LARGO_RECTANULO / 2d, 0));

            this.indicesRectanguloConductor = new List<int>();

            this.indicesRectanguloConductor.Add(0);
            this.indicesRectanguloConductor.Add(1);
            this.indicesRectanguloConductor.Add(2);
            this.indicesRectanguloConductor.Add(3);
            this.indicesRectanguloConductor.Add(0);
            this.indicesRectanguloConductor.Add(3);
            this.indicesRectanguloConductor.Add(7);
            this.indicesRectanguloConductor.Add(4);
            this.indicesRectanguloConductor.Add(4);
            this.indicesRectanguloConductor.Add(7);
            this.indicesRectanguloConductor.Add(6);
            this.indicesRectanguloConductor.Add(5);
            this.indicesRectanguloConductor.Add(1);
            this.indicesRectanguloConductor.Add(5);
            this.indicesRectanguloConductor.Add(6);
            this.indicesRectanguloConductor.Add(2);
            this.indicesRectanguloConductor.Add(0);
            this.indicesRectanguloConductor.Add(4);
            this.indicesRectanguloConductor.Add(5);
            this.indicesRectanguloConductor.Add(1);
            this.indicesRectanguloConductor.Add(7);
            this.indicesRectanguloConductor.Add(3);
            this.indicesRectanguloConductor.Add(2);
            this.indicesRectanguloConductor.Add(6);

            this.normalesRectanguloConductor = new List<Punto>();

            #region otras formas de calcular normales
            /*
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[0], 
                    this.verticesRectanguloConductor[1], 
                    this.verticesRectanguloConductor[4], 
                    this.verticesRectanguloConductor[7], 
                    this.verticesRectanguloConductor[3]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[1],
                    this.verticesRectanguloConductor[6],
                    this.verticesRectanguloConductor[5],
                    this.verticesRectanguloConductor[0],
                    this.verticesRectanguloConductor[2]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[2],
                    this.verticesRectanguloConductor[5],
                    this.verticesRectanguloConductor[1],
                    this.verticesRectanguloConductor[3],
                    this.verticesRectanguloConductor[6]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[3],
                    this.verticesRectanguloConductor[2],
                    this.verticesRectanguloConductor[0],
                    this.verticesRectanguloConductor[4],
                    this.verticesRectanguloConductor[7]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[4],
                    this.verticesRectanguloConductor[5],
                    this.verticesRectanguloConductor[7],
                    this.verticesRectanguloConductor[3],
                    this.verticesRectanguloConductor[0]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[5],
                    this.verticesRectanguloConductor[2],
                    this.verticesRectanguloConductor[6],
                    this.verticesRectanguloConductor[1],
                    this.verticesRectanguloConductor[4]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[6],
                    this.verticesRectanguloConductor[1],
                    this.verticesRectanguloConductor[2],
                    this.verticesRectanguloConductor[5],
                    this.verticesRectanguloConductor[7]));
            this.normalesRectanguloConductor.Add(Punto.CalcularNormal(
                    this.verticesRectanguloConductor[7],
                    this.verticesRectanguloConductor[6],
                    this.verticesRectanguloConductor[3],
                    this.verticesRectanguloConductor[0],
                    this.verticesRectanguloConductor[4]));
            */
            /*
            this.normalesRectanguloConductor.Add(new Punto(0, 0, -1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, 1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, 1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, -1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, -1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, 1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, 1));
            this.normalesRectanguloConductor.Add(new Punto(0, 0, -1));
             * */
            #endregion
            
            this.normalesRectanguloConductor.Add(new Punto(1, -1, -1));
            this.normalesRectanguloConductor.Add(new Punto(1, -1, 1));
            this.normalesRectanguloConductor.Add(new Punto(-1, -1, 1));
            this.normalesRectanguloConductor.Add(new Punto(-1, -1, -1));
            this.normalesRectanguloConductor.Add(new Punto(1, 1, -1));
            this.normalesRectanguloConductor.Add(new Punto(1, 1, 1));
            this.normalesRectanguloConductor.Add(new Punto(-1, 1, 1));
            this.normalesRectanguloConductor.Add(new Punto(-1, 1, -1));
        }

        #endregion
    }
}


