using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Trochita3D.Core;

namespace Trochita3D.Entidades
{
    /// <summary>
    /// Contiene los datos principales para dibujar un riel.
    /// </summary>
    public class Riel : SuperficieBarrido
    {
        private IList<Punto> puntosControl = new List<Punto>();

        public Riel()
        {
            this.RENDER_MODE = Gl.GL_QUAD_STRIP;

            // Puntos del contorno del riel
            puntosControl.Add(new Punto(0, -0.5, 0));
            puntosControl.Add(new Punto(0, 0.5, 0));
            puntosControl.Add(new Punto(0, 0.5, 0.3));
            puntosControl.Add(new Punto(0, 0.3, 0.3));
            puntosControl.Add(new Punto(0, 0.3, 0.65));
            puntosControl.Add(new Punto(0, 0.5, 0.65));
            puntosControl.Add(new Punto(0, 0.5, 1));
            puntosControl.Add(new Punto(0, -0.5, 1));
            puntosControl.Add(new Punto(0, -0.5, 0.65));
            puntosControl.Add(new Punto(0, -0.3, 0.65));
            puntosControl.Add(new Punto(0, -0.3, 0.3));
            puntosControl.Add(new Punto(0, -0.5, 0.3));
            puntosControl.Add(new Punto(0, -0.5, 0));
        }

        protected override void LoadMaterialProperties()
        {
            // Silver
            //Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.19225f, 0.19225f, 0.19225f, 1.0f });
            //Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.50754f, 0.50754f, 0.50754f, 1.0f });
            //Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.508273f, 0.508273f, 0.508273f, 1.0f });
            //Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 51.2f });

            // Polished Silver
            /*
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.23125f, 0.23125f, 0.23125f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2775f, 0.2775f, 0.2775f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.773911f, 0.773911f, 0.773911f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 0.896f });
            */

            // Chrome
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.25f, 0.25f, 0.25f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.4f, 0.4f, 0.4f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.774597f, 0.774597f, 0.774597f, 1.0f });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, new float[] { 76.8f });
        }

        protected override void BuildTextureCoordBuffer()
        {
            double v = 0;
            double distV = 0;
            IList<double> textCoord = new List<double>();

            for (int i = 0; i < secciones.Count; i++)
            {
                Seccion seccion = secciones[i];

                if (i != 0)
                {
                    Punto verticeAnt = secciones[i - 1].Vertices[secciones[i - 1].Vertices.Count / 2];
                    Punto verticeActual = seccion.Vertices[seccion.Vertices.Count / 2];
                    distV = (verticeActual - verticeAnt).Modulo();
                    v += distV * VTextureAspectRatio;
                }

                textCoord.Add(0);
                textCoord.Add(v);
                textCoord.Add(0.01);
                textCoord.Add(v);
                textCoord.Add(0.1);
                textCoord.Add(v);
                textCoord.Add(0.2);
                textCoord.Add(v);
                textCoord.Add(0.3);
                textCoord.Add(v);
                textCoord.Add(0.35);
                textCoord.Add(v);
                
                textCoord.Add(0.40);
                textCoord.Add(v);
                textCoord.Add(0.55);
                textCoord.Add(v);

                textCoord.Add(0.6);
                textCoord.Add(v);
                textCoord.Add(0.7);
                textCoord.Add(v);
                textCoord.Add(0.8);
                textCoord.Add(v);
                textCoord.Add(0.9);
                textCoord.Add(v);
                textCoord.Add(1);
                textCoord.Add(v);
            }

            this.textures = textCoord.ToArray<double>();
        }

        /// <summary>
        /// Obtiene la seccion representada por los puntos de control para dibujar
        /// la seccion del riel.
        /// </summary>
        /// <returns>
        /// Seccion con los puntos de control para dibujar un riel.
        /// </returns>
        public override Seccion GetSeccion()
        {
            return new Seccion(puntosControl);
        }

    }
}
