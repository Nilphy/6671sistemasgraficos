using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class Laguna : Superficie
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double Altura { get; set; }

        public Laguna()
        {
            this.RENDER_MODE = Gl.GL_QUADS;
        }

        protected override void LoadSecciones()
        {
            Seccion seccion = new Seccion();
            seccion.Vertices.Add(new Punto(-this.Width / 2, -this.Height / 2, this.Altura));
            seccion.Vertices.Add(new Punto(this.Width / 2, -this.Height / 2, this.Altura));
            this.secciones.Add(seccion);

            seccion = new Seccion();
            seccion.Vertices.Add(new Punto(-this.Width / 2, this.Height / 2, this.Altura));
            seccion.Vertices.Add(new Punto(this.Width / 2, this.Height / 2, this.Altura));
            this.secciones.Add(seccion);
        }

        protected override Punto CalculateNormalForPunto(int posVertexActual, Punto verticeActual, int posSeccionActual, Seccion seccionActual)
        {
            return new Punto(0, 0, 1);
        }

        protected override void LoadMaterialProperties()
        {
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, new float[] { 0.0f, 0.0f, 0.9f, 1f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { .3f, .3f, .3f, 1f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 50f });
        }

    }
}
