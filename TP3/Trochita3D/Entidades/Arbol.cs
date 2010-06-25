using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Trochita3D.Core;
using Trochita3D.Curvas;


namespace Trochita3D.Entidades
{

    class Arbol
    {
        private double longitudTronco;
        private double radioBaseTronco;
        private PuntoFlotante[][] matrizPuntosCopa;

        private static int CANTIDAD_CARAS = 15;
        private static double FACTOR_TRONCO = 6.0f;
        private static double PASO_BEZIER = .1f;
        private static bool DIBUJAR_NORMALES = false;
        private static double MINIMO_RADIO = .3f;

        private Glu.GLUquadric quadraticCylinder = Glu.gluNewQuadric();

        private static double LONGITUD_TRONCO_DEFAULT = 2.0f;
        private static PuntoFlotante[] FORMA_COPA_DEFAULT = new PuntoFlotante[] 
            {
                new PuntoFlotante ( 0,   0, 0 ),
                new PuntoFlotante ( .4,   .5, 0 ),
                new PuntoFlotante ( .8, 1, 0 ),
                new PuntoFlotante ( 1.0,   1.5, 0 ), 
                new PuntoFlotante ( .8, 2, 0 ), 
                new PuntoFlotante ( 1.1,   2.5, 0 ), 
                new PuntoFlotante ( 1.0,   3, 0 ), 
                new PuntoFlotante ( .9,   3.5, 0 ), 
                new PuntoFlotante ( .8,   4.0, 0 ), 
                new PuntoFlotante ( .5,   4.5, 0 ), 
                new PuntoFlotante ( 0,   5, 0 )
            };

        public static Arbol[] GenerarArbolesAleatorios(int cantidad)
        {
            Arbol[] arboles = new Arbol[cantidad];
            for (int i = 0; i < cantidad; ++i)
            {
                PuntoFlotante[] formaCopa = new PuntoFlotante[FORMA_COPA_DEFAULT.Length];
                for (int j = 0; j < formaCopa.Length; ++j)
                {
                    if (j == 0 || j == formaCopa.Length - 1) // siempre terminan y empiezan en el mismo lugar
                    {
                        formaCopa[j] = new PuntoFlotante(
                            Arbol.FORMA_COPA_DEFAULT[j].X,
                            Arbol.FORMA_COPA_DEFAULT[j].Y,
                            Arbol.FORMA_COPA_DEFAULT[j].Z
                            );
                    }
                    else
                    {
                        double nuevoX = Arbol.FORMA_COPA_DEFAULT[j].X + Arbol.RandomEntre(-.5f, +1.0f);
                        double nuevoY = Arbol.FORMA_COPA_DEFAULT[j].Y + Arbol.RandomEntre(-.5f, +1.0f);
                        double nuevoZ = Arbol.FORMA_COPA_DEFAULT[j].Z;

                        formaCopa[j] = new PuntoFlotante(
                                nuevoX > MINIMO_RADIO ? nuevoX : MINIMO_RADIO,
                                nuevoY > MINIMO_RADIO ? nuevoY : MINIMO_RADIO,
                                nuevoZ > MINIMO_RADIO ? nuevoZ : MINIMO_RADIO
                            );
                    }
                }

                double longitudTronco = LONGITUD_TRONCO_DEFAULT + Arbol.RandomEntre(-.5f, +1.5f);

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

        public Arbol(double longitudTronco, PuntoFlotante[] formaCopa)
        {
            this.longitudTronco = longitudTronco;
            this.radioBaseTronco = this.longitudTronco / FACTOR_TRONCO;
            this.matrizPuntosCopa = this.generarMatrizVertices(formaCopa, 360.0f, CANTIDAD_CARAS);
        }

        public void Dibujar()
        {
            this.DibujarTronco();

            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, this.longitudTronco * .6);
            this.DibujarCopa();
            Gl.glPopMatrix();
        }

        private void DibujarTronco()
        {
            // Marron
            Gl.glColor3d(124.0f/255.0f, 87.0f/255.0f, 59.0f/255.0f);
            Glu.gluQuadricNormals(quadraticCylinder, Glu.GLU_SMOOTH);
            Glu.gluCylinder(quadraticCylinder, this.radioBaseTronco, this.radioBaseTronco, this.longitudTronco, CANTIDAD_CARAS, 1);
        }

        ~Arbol()
        {
            Glu.gluDeleteQuadric(quadraticCylinder);
        }

        private void DibujarCopa ()
        {
            Gl.glPushMatrix();

            Gl.glRotated(90.0f, 1, 0, 0);
            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
            for (int i = 0; i < matrizPuntosCopa.Length - 2; i++) // por columnas duplicadas
            {
                // Verde...
                Gl.glColor3d(0, 1, 0);
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
                for (int i = 0; i < matrizPuntosCopa.Length - 2; i++) // por columnas duplicadas
                {
                    for (int j = 0; j < matrizPuntosCopa[i].Length; j++)
                    {
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glVertex3d(matrizPuntosCopa[i][j].X, matrizPuntosCopa[i][j].Y, matrizPuntosCopa[i][j].Z);
                        Gl.glVertex3d(matrizPuntosCopa[i][j].NormalX, matrizPuntosCopa[i][j].NormalY, matrizPuntosCopa[i][j].NormalZ);
                        Gl.glEnd();
                        Gl.glColor3d(1, 0, 0);
                    }
                }
                
            }
            Gl.glPopMatrix();
        }

        private PuntoFlotante[][] generarMatrizVertices(PuntoFlotante[] curva, double gradosRevolucion, int cantidadCaras)
        {
            double anguloRotacionRadianes = GradosARadianes(gradosRevolucion / cantidadCaras);
            CurvaBzierSegmentosCubicos curvaBS = new CurvaBzierSegmentosCubicos(curva);
            PuntoFlotante[] curvaDiscretizada = curvaBS.GetPuntosDiscretos(PASO_BEZIER).ToArray<PuntoFlotante>();

            PuntoFlotante[][] matriz = new PuntoFlotante[cantidadCaras + 2][];

            for (int iteradorRotacion = 0; iteradorRotacion < cantidadCaras; ++iteradorRotacion)
            {
                matriz[iteradorRotacion] = new PuntoFlotante[curvaDiscretizada.Length];
                for (int iteradorBezier = 0; iteradorBezier < curvaDiscretizada.Length; ++iteradorBezier)
                {
                    PuntoFlotante puntoCurva = curvaDiscretizada[iteradorBezier];
                    matriz[iteradorRotacion][iteradorBezier] = new PuntoFlotante(puntoCurva);

                    // roto, al mismo tiempo, los puntos de la curva alrededor del eje Y
                    PuntoFlotante puntoCurvaBack = new PuntoFlotante(puntoCurva);
                    puntoCurva.X = puntoCurvaBack.X * Math.Cos(anguloRotacionRadianes) + puntoCurvaBack.Z * Math.Sin(anguloRotacionRadianes);
                    puntoCurva.Z = puntoCurvaBack.Z * Math.Cos(anguloRotacionRadianes) - puntoCurvaBack.X * Math.Sin(anguloRotacionRadianes);

                    curvaDiscretizada[iteradorBezier] = puntoCurva;
                }
            }

            matriz[cantidadCaras] = matriz[0];
            matriz[cantidadCaras + 1] = matriz[1]; // OJO solo si los grados son 360

            // calculo las normales en toda la matriz
            for (int i = 0; i < matriz.Length - 1; ++i)
            {
                for (int j = 0; j < curvaDiscretizada.Length - 1; ++j)
                {
                    PuntoFlotante normal = (matriz[i + 1][j] - matriz[i][j]) * (matriz[i][j + 1] - matriz[i][j]);
                    normal = normal - matriz[i][j];
                    normal = normal * (-1.0d);
                    normal = normal + matriz[i][j];

                    matriz[i][j].NormalX = normal.X;
                    matriz[i][j].NormalY = normal.Y;
                    matriz[i][j].NormalZ = normal.Z;
                }
            }

            return matriz;
        }

        private static double RadianesAGrados(double radianes)
        {
            return radianes * 180.0f / Math.PI;
        }

        private static double GradosARadianes(double grados)
        {
            return grados * Math.PI / 180.0f;
        }

        private static double RandomEntre(double numMin, double numMax)
        {
            Random RandomNumber = new Random((int)DateTime.Now.Ticks);
            double random = (RandomNumber.NextDouble() + numMin) % numMax;
            return random;
        }
    }


}
