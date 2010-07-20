using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Trochita3D.Core;
using Trochita3D.Curvas;
using Common.Utils;


namespace Trochita3D.Entidades
{

    class Arbol
    {
        private double longitudTronco;
        private double radioBaseTronco;
        private Punto[][] matrizPuntosCopa;

        private static int CANTIDAD_CARAS = 10;
        private static double FACTOR_TRONCO = 6.0f;
        private static double PASO_BEZIER = .4f;
        private static bool DIBUJAR_NORMALES = false;
        private static double MINIMO_RADIO = .3f;

        private Glu.GLUquadric quadraticCylinder = Glu.gluNewQuadric();

        private static double LONGITUD_TRONCO_DEFAULT = 2.0f;
        private static Punto[] FORMA_COPA_DEFAULT = new Punto[] 
            {
                new Punto ( 0,   0, 0 ),
                new Punto ( .4,   .5, 0 ),
                new Punto ( .8, 1, 0 ),
                new Punto ( 1.0,   1.5, 0 ), 
                new Punto ( .8, 2, 0 ), 
                new Punto ( 1.1,   2.5, 0 ), 
                new Punto ( 1.0,   3, 0 ), 
                new Punto ( .9,   3.5, 0 ), 
                new Punto ( .8,   4.0, 0 ), 
                new Punto ( .5,   4.5, 0 ), 
                new Punto ( .2,   4.7, 0 ),
                new Punto ( .1,   4.8, 0 ),
                new Punto ( 0,   5, 0 )
            };

        public static Arbol[] GenerarArbolesAleatorios(int cantidad)
        {
            Arbol[] arboles = new Arbol[cantidad];
            for (int i = 0; i < cantidad; ++i)
            {
                Punto[] formaCopa = new Punto[FORMA_COPA_DEFAULT.Length];
                for (int j = 0; j < formaCopa.Length; ++j)
                {
                    if (j == 0 || j == formaCopa.Length - 1) // siempre terminan y empiezan en el mismo lugar
                    {
                        formaCopa[j] = new Punto(
                            Arbol.FORMA_COPA_DEFAULT[j].X,
                            Arbol.FORMA_COPA_DEFAULT[j].Y,
                            Arbol.FORMA_COPA_DEFAULT[j].Z
                            );
                    }
                    else
                    {
                        double nuevoX = Arbol.FORMA_COPA_DEFAULT[j].X + Arbol.RandomEntre(0f, +2.0f);
                        double nuevoY = Arbol.FORMA_COPA_DEFAULT[j].Y;
                        double nuevoZ = Arbol.FORMA_COPA_DEFAULT[j].Z;

                        formaCopa[j] = new Punto(
                                nuevoX > MINIMO_RADIO ? nuevoX : MINIMO_RADIO,
                                nuevoY,
                                nuevoZ
                            );
                    }
                }

                double longitudTronco = LONGITUD_TRONCO_DEFAULT + Arbol.RandomEntre(0f, +2.5f);

                Arbol arbol = new Arbol(longitudTronco, formaCopa);
                arboles[i] = arbol;
            }
            return arboles;
        }

        public Arbol()
        {
            this.longitudTronco = LONGITUD_TRONCO_DEFAULT;
            this.radioBaseTronco = this.longitudTronco / FACTOR_TRONCO;
            this.matrizPuntosCopa = this.generarMatrizVertices(FORMA_COPA_DEFAULT, 360.0f, CANTIDAD_CARAS);
        }

        public Arbol(double longitudTronco, Punto[] formaCopa)
        {
            this.longitudTronco = longitudTronco;
            this.radioBaseTronco = this.longitudTronco / FACTOR_TRONCO;
            this.matrizPuntosCopa = this.generarMatrizVertices(formaCopa, 360.0f, CANTIDAD_CARAS);
        }

        public void Dibujar()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glEnable(Gl.GL_LIGHTING);
            this.DibujarTronco();

            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, this.longitudTronco * .6);
            this.DibujarCopa();

            Gl.glPopMatrix();
            Gl.glDisable(Gl.GL_LIGHTING);
        }

        private void DibujarTronco()
        {
            float[] colorBrown      = new float[4] { 124.0f / 255.0f, 87.0f / 255.0f, 59.0f / 255.0f, .5f };
            float[] colorNone       =  new float [4] { 0.0f, 0.0f, 0.0f, 0.0f };

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, colorBrown);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, new float[] {50.0f});

            Glu.gluQuadricNormals(quadraticCylinder, Glu.GLU_SMOOTH);
            Glu.gluCylinder(quadraticCylinder, this.radioBaseTronco, this.radioBaseTronco, this.longitudTronco, CANTIDAD_CARAS, 1);

        }

        ~Arbol()
        {
            Glu.gluDeleteQuadric(quadraticCylinder);
        }

        private void DibujarCopa ()
        {
            float[] colorGreen = new float[4] { 0f, .4f, 0f, .5f };
            float[] colorNone = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, colorGreen);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SHININESS, new float[] { 10.0f });

            Gl.glPushMatrix();

            Gl.glRotated(90.0f, 1, 0, 0);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            for (int i = 0; i < matrizPuntosCopa.Length - 2; i++) // por columnas duplicadas
            {
                for (int j = 0; j < matrizPuntosCopa[i].Length; j++)
                {
                    Gl.glNormal3d(matrizPuntosCopa[i][j].NormalX, matrizPuntosCopa[i][j].NormalY, matrizPuntosCopa[i][j].NormalZ);
                    Gl.glVertex3d(matrizPuntosCopa[i][j].X, matrizPuntosCopa[i][j].Y, matrizPuntosCopa[i][j].Z);

                    Gl.glNormal3d(matrizPuntosCopa[i + 1][j].NormalX, matrizPuntosCopa[i + 1][j].NormalY, matrizPuntosCopa[i + 1][j].NormalZ);
                    Gl.glVertex3d(matrizPuntosCopa[i + 1][j].X, matrizPuntosCopa[i + 1][j].Y, matrizPuntosCopa[i + 1][j].Z);
                }
            }
            Gl.glEnd();

            if (DIBUJAR_NORMALES)
            {
                Gl.glDisable(Gl.GL_LIGHTING);
                for (int i = 0; i < matrizPuntosCopa.Length - 2; i++) // por columnas duplicadas
                {
                    for (int j = 0; j < matrizPuntosCopa[i].Length; j++)
                    {
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glColor3d(1, 0, 0); 
                        Gl.glVertex3d(matrizPuntosCopa[i][j].X, matrizPuntosCopa[i][j].Y, matrizPuntosCopa[i][j].Z);
                        Gl.glColor3d(.2, 0, 0); 
                        Gl.glVertex3d(matrizPuntosCopa[i][j].NormalX, matrizPuntosCopa[i][j].NormalY, matrizPuntosCopa[i][j].NormalZ);
                        Gl.glEnd();
                        
                    }
                }
                Gl.glEnable(Gl.GL_LIGHTING);
            }
            Gl.glPopMatrix();
        }

        private Punto[] ObtenerNormalesCurva2D(Punto[] curva)
        {
            // a(x,y)(ortogonal) = (-ay, ax)
            Punto[] normales = new Punto[curva.Length];
            for (int i = 0; i < curva.Length - 1; ++i)
            {
                Punto p = curva[i + 1] - curva[i];
                Punto normal = new Punto( -p.Y , p.X, p.Z);

                if (p.X != 0)
                {
                    double pendiente = p.Y / p.X;
                    if (pendiente < 0)
                    {
                        normal = new Punto(- normal.X, - normal.Y, - normal.Z);
                    }
                }
                normales[i] = normal + curva[i];
            }

            normales[curva.Length - 1] = normales[curva.Length - 2];
            return normales;
        }

        private Punto[][] generarMatrizVertices(Punto[] curva, double gradosRevolucion, int cantidadCaras)
        {
            double anguloRotacionRadianes = MathUtils.DegreeToRadian(gradosRevolucion / cantidadCaras);
            CurvaBzierSegmentosCubicos curvaBS = new CurvaBzierSegmentosCubicos(curva);
            Punto[] curvaDiscretizada = curvaBS.GetPuntosDiscretos(PASO_BEZIER).ToArray<Punto>();

            Punto[] normales = this.ObtenerNormalesCurva2D(curvaDiscretizada);

            Punto[][] matriz = new Punto[cantidadCaras + 2][];

            for (int iteradorRotacion = 0; iteradorRotacion < cantidadCaras; ++iteradorRotacion)
            {
                matriz[iteradorRotacion] = new Punto[curvaDiscretizada.Length];
                for (int iteradorBezier = 0; iteradorBezier < curvaDiscretizada.Length; ++iteradorBezier)
                {
                    Punto puntoCurva = curvaDiscretizada[iteradorBezier];
                    matriz[iteradorRotacion][iteradorBezier] = puntoCurva.Clone();

                    // roto, al mismo tiempo, los puntos de la curva alrededor del eje Y
                    Punto puntoCurvaBack = puntoCurva.Clone();
                    puntoCurva.X = puntoCurvaBack.X * Math.Cos(anguloRotacionRadianes) + puntoCurvaBack.Z * Math.Sin(anguloRotacionRadianes);
                    puntoCurva.Z = puntoCurvaBack.Z * Math.Cos(anguloRotacionRadianes) - puntoCurvaBack.X * Math.Sin(anguloRotacionRadianes);

                    curvaDiscretizada[iteradorBezier] = puntoCurva;


                    // roto, al mismo tiempo, los puntos de la normal alrededor del eje Y
                    Punto puntoNormal = normales[iteradorBezier];
                    matriz[iteradorRotacion][iteradorBezier].NormalX = puntoNormal.X;
                    matriz[iteradorRotacion][iteradorBezier].NormalY = puntoNormal.Y;
                    matriz[iteradorRotacion][iteradorBezier].NormalZ = puntoNormal.Z;

                    Punto puntoNormalBack = puntoNormal.Clone();
                    puntoNormal.X = puntoNormalBack.X * Math.Cos(anguloRotacionRadianes) + puntoNormalBack.Z * Math.Sin(anguloRotacionRadianes);
                    puntoNormal.Z = puntoNormalBack.Z * Math.Cos(anguloRotacionRadianes) - puntoNormalBack.X * Math.Sin(anguloRotacionRadianes);

                    normales[iteradorBezier] = puntoNormal; 

                }


            }

            matriz[cantidadCaras] = matriz[0];
            matriz[cantidadCaras + 1] = matriz[1]; // OJO solo si los grados son 360


            /*
            // calculo las normales en toda la matriz
            Boolean invertirNormal = true;
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;
            Punto puntoCentro = null;
            int cantidadPixelesAlto = curvaDiscretizada.Length;
            int cantidadPixelesAncho = matriz.Length;

            for (int indiceFila = 0; indiceFila < cantidadPixelesAncho; ++indiceFila)
            {
                for (int indiceColumna = 0; indiceColumna < cantidadPixelesAlto; ++indiceColumna)
                {
                    puntoCentro = matriz[indiceFila][indiceColumna];
                    
                    if (indiceFila + 1 < cantidadPixelesAncho - 1) // Hay punto norte
                        puntoNorte = matriz[indiceFila + 1][indiceColumna];
                    else puntoNorte = null;
                    
                    if (indiceFila - 1 >= 0) // Hay punto sur
                        puntoSur = matriz[indiceFila - 1][indiceColumna];
                    else puntoSur = null;
                    
                    if (indiceColumna - 1 >= 0) // Hay punto este
                        puntoOeste = matriz[indiceFila][indiceColumna - 1];
                    else puntoOeste = null;
                    
                    if (indiceColumna + 1 < cantidadPixelesAlto - 1) // Hay punto oeste
                        puntoEste = matriz[indiceFila][indiceColumna + 1];
                    else puntoEste = null;

                    Punto normal = Punto.CalcularNormal(puntoCentro, puntoNorte, puntoEste, puntoSur, puntoOeste, invertirNormal);
                    matriz[indiceFila][indiceColumna].NormalX = normal.X;
                    matriz[indiceFila][indiceColumna].NormalY = normal.Y;
                    matriz[indiceFila][indiceColumna].NormalZ = normal.Z;
                }
            }
            */
            return matriz;
        }

        private static double RandomEntre(double numMin, double numMax)
        {
            Random RandomNumber = new Random((int)DateTime.Now.Ticks);
            double random = numMax * (RandomNumber.NextDouble() + numMin) % numMax;
            return random;
        }
    }


}
