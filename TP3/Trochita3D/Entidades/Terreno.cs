using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using System.Drawing;
using Tao.OpenGl;

namespace Trochita3D.Entidades
{
    public class Terreno : Superficie
    {

        public int Height { get; set; }
        public int Width { get; set; }
        public int AlturaMaxima { get; set; }
        public double VertexRatio { get; set; }

        public Terreno()
        {
            this.Height = 200;
            this.Width = 200;
            this.AlturaMaxima = 2;
            this.VertexRatio = 1;
            this.RENDER_MODE = Gl.GL_QUADS;
        }

        protected override void LoadSecciones()
        {
            Bitmap mapa = new Bitmap(@"../../Imagenes/Bitmap.bmp");

            for (double i = 0; i < (this.Width * this.VertexRatio); i++)
            {
                Seccion seccion = new Seccion();

                for (double j = 0; j < (this.Height * this.VertexRatio); j++)
                {
                    double altura = (mapa.GetPixel(this.GetXBitmapCoord(i, mapa), this.GetYBitmapCoord(j, mapa)).B) * (this.AlturaMaxima / 255d);
                    seccion.Vertices.Add(new Punto((i / this.VertexRatio) - (this.Width / 2), (j / this.VertexRatio) - (this.Height / 2), altura));
                }

                secciones.Add(seccion);
            }
        }

        protected override void LoadMaterialProperties()
        {
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, new float[] { .5f, .5f, .5f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.0f, 0.0f, 0.0f, 1 });
        }

        protected override Punto CalculateNormalForPunto(int posVertexActual, Punto verticeActual, int posSeccionActual, Seccion seccionActual)
        {
            Punto puntoNorte = null;
            Punto puntoSur = null;
            Punto puntoEste = null;
            Punto puntoOeste = null;
            if (posSeccionActual + 1 < (this.Width * this.VertexRatio) - 1) puntoNorte = this.secciones[posSeccionActual + 1].Vertices[posVertexActual];
            if (posSeccionActual - 1 > 0) puntoSur = this.secciones[posSeccionActual - 1].Vertices[posVertexActual];
            if (posVertexActual + 1 < (this.Height * this.VertexRatio) - 1) puntoEste = this.secciones[posSeccionActual].Vertices[posVertexActual + 1];
            if (posVertexActual - 1 > 0) puntoOeste = this.secciones[posSeccionActual].Vertices[posVertexActual - 1];

            return Punto.CalcularNormal(this.secciones[posSeccionActual].Vertices[posVertexActual], puntoNorte, puntoEste, puntoSur, puntoOeste, true);
        }

        private int GetXBitmapCoord(double x, Bitmap mapa)
        {
            return Convert.ToInt32((x * mapa.Width) / (this.Width * this.VertexRatio));
        }

        private int GetYBitmapCoord(double y, Bitmap mapa)
        {
            return Convert.ToInt32((y * mapa.Height) / (this.Height * this.VertexRatio));
        }

    }
}
