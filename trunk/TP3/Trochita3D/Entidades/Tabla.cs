using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Tao.OpenGl;

namespace Trochita3D.Entidades
{
    /// <summary>
    /// Clase que representa las tablas que van debajo de los rieles para soportarlos.
    /// </summary>
    public class Tabla : Superficie
    {
        private double distTablas;
        private double alturaTablas;
        private IList<Punto> camino;
        private IList<Punto> puntosControl = new List<Punto>();

        public Tabla(double distTablas, double alturaTablas)
        {
            this.RENDER_MODE = Gl.GL_QUADS;
            this.distTablas = distTablas;
            this.alturaTablas = alturaTablas;

            puntosControl.Add(new Punto(0, -1, 0));
            puntosControl.Add(new Punto(0, -1, 0.5));
            puntosControl.Add(new Punto(0, 1, 0.5));
            puntosControl.Add(new Punto(0, 1, 0));
        }

        protected override void LoadSecciones()
        {
            Punto puntoAnterior = camino[camino.Count - 1];
            Punto puntoActual;
            Punto vectorPuntoAnteriorActual;
            Seccion seccion1, seccion2;
            double dx, dy, angulo;
            double distancia = 0;

            for (int i = 0; i < camino.Count; i++)
            {
                puntoActual = camino[i];
                vectorPuntoAnteriorActual = puntoActual - puntoAnterior;
                distancia += vectorPuntoAnteriorActual.Modulo();

                if (distancia > distTablas)
                {
                    seccion1 = this.GetSeccion();
                    seccion2 = this.GetSeccion();
                    distancia = 0;
                    dx = puntoActual.X - puntoAnterior.X;
                    dy = puntoActual.Y - puntoAnterior.Y;

                    angulo = Math.Atan(dy / dx);

                    int cuadrante = vectorPuntoAnteriorActual.GetCuadrante();
                    if (cuadrante.Equals(1) || cuadrante.Equals(4))
                        angulo -= Math.PI;

                    seccion1.Escalar(1, 1, 0.2);
                    seccion1.Rotar(angulo);
                    seccion1.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + this.alturaTablas - seccion1.Vertices[1].Z);

                    seccion2.Escalar(1, 1, 0.2);
                    seccion2.Trasladar(0.5, 0, 0);
                    seccion2.Rotar(angulo);
                    seccion2.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z + this.alturaTablas - seccion2.Vertices[1].Z);

                    secciones.Add(seccion1);
                    secciones.Add(seccion2);
                }

                puntoAnterior = puntoActual;
            }

            this.BuildSurfaceDataBuffers();
        }

        protected override void BuildSurfaceDataBuffers()
        {
            Seccion seccion1, seccion2;
            int indexCount = 0;
            IList<double> vertices = new List<double>();
            IList<int> indices = new List<int>();
            IList<double> normales = new List<double>();

            for (int i = 0; i < secciones.Count; i += 2)
            {
                seccion1 = secciones[i];
                seccion2 = secciones[i + 1];

                for (int j = 0; j < seccion1.Vertices.Count; j++)
                {
                    // Vertices
                    vertices.Add(seccion1.Vertices[j].X);
                    vertices.Add(seccion1.Vertices[j].Y);
                    vertices.Add(seccion1.Vertices[j].Z);

                    // Vertices
                    vertices.Add(seccion2.Vertices[j].X);
                    vertices.Add(seccion2.Vertices[j].Y);
                    vertices.Add(seccion2.Vertices[j].Z);

                    // Normales
                    Punto normal = this.CalculateNormalForPunto(j, seccion1.Vertices[j], i, seccion1);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);

                    normal = this.CalculateNormalForPunto(j, seccion2.Vertices[j], i, seccion2);
                    normales.Add(normal.X);
                    normales.Add(normal.Y);
                    normales.Add(normal.Z);
                }

                // Q1
                indices.Add(indexCount);
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 1);

                // Q2
                indices.Add(indexCount + 1);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 7);

                // Q3
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 7);
                indices.Add(indexCount + 6);
                indices.Add(indexCount + 4);

                // Q4
                indices.Add(indexCount);
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 4);
                indices.Add(indexCount + 6);

                // Q5
                indices.Add(indexCount + 2);
                indices.Add(indexCount + 3);
                indices.Add(indexCount + 5);
                indices.Add(indexCount + 4);

                // Q6
                indices.Add(indexCount);
                indices.Add(indexCount + 1);
                indices.Add(indexCount + 7);
                indices.Add(indexCount + 6);

                indexCount += 8;
            }

            this.vertex = vertices.ToArray<double>();
            this.normals = normales.ToArray<double>();
            this.indexes = indices.ToArray<int>();
        }

        protected override void BuildTextureCoordBuffer()
        {
            IList<double> textCoord = new List<double>();

            for (int i = 0; i < this.secciones.Count; i += 2)
            {
                Seccion seccionAbajo = this.secciones[i];
                Seccion seccionArriba = this.secciones[i + 1];

                textCoord.Add(0.1);
                textCoord.Add(0);
            }

            this.textures = textCoord.ToArray<double>();
        }

        protected override void LoadMaterialProperties()
        {
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 128f / 255f, 64f / 255f, 0.0f, 0.25f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 128f / 255f, 64f / 255f, 0.0f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0, 0, 0, 1 });
        }

        public void SetCamino(IList<Punto> camino)
        {
            this.camino = camino;
            this.LoadSecciones();
        }

        public Seccion GetSeccion()
        {
            return new Seccion(puntosControl);
        }
    }
}
