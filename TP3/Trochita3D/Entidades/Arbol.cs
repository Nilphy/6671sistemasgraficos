using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Trochita3D.Core;


namespace Trochita3D.Entidades
{

    class Arbol
    {
        private double longitudTronco;
        private double longitudCopa;

        private PuntoFlotante[][] matrizPuntosCopa;

        public Arbol()
        {
            // Default
            PuntoFlotante[] formaCopa = new PuntoFlotante[] 
            {
                new PuntoFlotante ( 0, 0, 0 ),
                new PuntoFlotante ( 1, 1, 0 ),
                new PuntoFlotante ( 1.5, 2, 0 ),
                new PuntoFlotante ( 2, 3, 0 ), 
                new PuntoFlotante ( 1.5, 4, 0 ), 
                new PuntoFlotante ( 1, 5, 0 ), 
                new PuntoFlotante ( 0, 6, 0 ), 
            };

            this.longitudTronco = 4;
            this.longitudCopa = 2;
            this.matrizPuntosCopa = this.generarMatrizVertices(formaCopa, 360, 15);
        }

        public Arbol(double longitudTronco, double longitudCopa, PuntoFlotante[] formaCopa)
        {
            this.longitudTronco = longitudTronco;
            this.longitudCopa = longitudCopa;
            this.matrizPuntosCopa = this.generarMatrizVertices(formaCopa, 360, 24);
        }

        public void Dibujar()
        {/*
            Gl.glColor3f(0, 0, 1);
            for (int i = 0; i < matrizPuntosCopa.Length; i++)
            {
                Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                for (int j = 0; j < matrizPuntosCopa[i].Length; j++)
                {
                    Gl.glVertex3d(matrizPuntosCopa[i][j].X, matrizPuntosCopa[i][j].Y, matrizPuntosCopa[i][j].Z);
                    System.Console.Out.WriteLine(matrizPuntosCopa[i][j].X + ", " + matrizPuntosCopa[i][j].Y + ", " + matrizPuntosCopa[i][j].Z + ") ");

                }
                Gl.glEnd();
            }
            return;*/

            for (int i = 0; i < matrizPuntosCopa.Length - 2; i++) // por columnas duplicadas
            {
                Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                Gl.glColor3d(0, 1, 0);
                for (int j = 0; j < matrizPuntosCopa[i].Length; j++)
                {
                    Gl.glNormal3d(matrizPuntosCopa[i][j].NormalX, matrizPuntosCopa[i][j].NormalY, matrizPuntosCopa[i][j].NormalZ);
                    Gl.glVertex3d(matrizPuntosCopa[i][j].X, matrizPuntosCopa[i][j].Y, matrizPuntosCopa[i][j].Z);
                    System.Console.Out.WriteLine(matrizPuntosCopa[i][j].X + ", " + matrizPuntosCopa[i][j].Y + ", " + matrizPuntosCopa[i][j].Z + ") ");

                    Gl.glNormal3d(matrizPuntosCopa[i + 1][j].NormalX, matrizPuntosCopa[i + 1][j].NormalY, matrizPuntosCopa[i + 1][j].NormalZ);
                    Gl.glVertex3d(matrizPuntosCopa[i + 1][j].X, matrizPuntosCopa[i + 1][j].Y, matrizPuntosCopa[i + 1][j].Z);
                    System.Console.Out.WriteLine(matrizPuntosCopa[i + 1][j].X + ", " + matrizPuntosCopa[i + 1][j].Y + ", " + matrizPuntosCopa[i + 1][j].Z + ") ");

                }
                Gl.glEnd();
            }
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private PuntoFlotante[][] generarMatrizVertices(PuntoFlotante[] curva, double gradosRevolucion, int cantidadRotaciones)
        {
            double anguloRotacionRadianes = GradosARadianes(gradosRevolucion / cantidadRotaciones);
            PuntoFlotante[][] matriz = new PuntoFlotante[cantidadRotaciones + 2][];

            for (int iteradorRotacion = 0; iteradorRotacion < cantidadRotaciones; ++iteradorRotacion)
            {
                matriz[iteradorRotacion] = new PuntoFlotante[curva.Length];
                for (int iteradorCurva = 0; iteradorCurva < curva.Length; ++iteradorCurva)
                {
                    PuntoFlotante puntoCurva = curva[iteradorCurva];
                    matriz[iteradorRotacion][iteradorCurva] = new PuntoFlotante(puntoCurva);

                    // roto, al mismo tiempo, los puntos de la curva alrededor del eje Y
                    PuntoFlotante puntoCurvaBack = new PuntoFlotante(puntoCurva);
                    puntoCurva.X = puntoCurvaBack.X * Math.Cos(anguloRotacionRadianes) + puntoCurvaBack.Z * Math.Sin(anguloRotacionRadianes);
                    puntoCurva.Z = puntoCurvaBack.Z * Math.Cos(anguloRotacionRadianes) - puntoCurvaBack.X * Math.Sin(anguloRotacionRadianes);

                    curva[iteradorCurva] = puntoCurva;
                }
            }

            matriz[cantidadRotaciones] = matriz[0];
            matriz[cantidadRotaciones + 1] = matriz[1]; // OJO solo si los grados son 360

            // calculo las normales en toda la matriz
            for (int i = 0; i < matriz.Length - 1; ++i)
            {
                for (int j = 0; j < curva.Length - 1; ++j)
                {
                    PuntoFlotante normal = (matriz[i + 1][j] - matriz[i][j]) * (matriz[i][j + 1] - matriz[i][j]);
                    matriz[i][j].NormalX = normal.X;
                    matriz[i][j].NormalY = normal.Y;
                    matriz[i][j].NormalZ = normal.Z;
                }
            }

            return matriz;
        }

        private double RadianesAGrados(double radianes)
        {
            return radianes * 180.0f / Math.PI;
        }

        private double GradosARadianes(double grados)
        {
            return grados * Math.PI / 180.0f;
        }
    }


}
