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

        #region Medidas

        // Rectángulo conductor
        private static double ANCHO_RECTANGULO = 3d;
        private static double LARGO_RECTANULO = 3d;
        private static double ALTO_RECTANGULO = 4.5d;

        // trompa
        private static double RADIO_TROMPA = (ANCHO_RECTANGULO - ANCHO_RECTANGULO/4d)/2d;
        private static double LARGO_TROMPA = LARGO_RECTANULO * 2d;
        
        // Base
        private static double ANCHO_BASE = ANCHO_RECTANGULO + 1;
        private static double ALTO_BASE = ALTO_RECTANGULO/20d;
        private static double LARGO_BASE = LARGO_RECTANULO*2d + 0.2d;
            
        // Paragolpe        
        private static double ANCHO_PARAGOLPE = ANCHO_BASE;
        private static double ALTO_PARAGOLPE = ALTO_RECTANGULO / 5d;
        private static double LARGO_PARAGOLPE = LARGO_RECTANULO / 40d;

        // Agarrador
        private static double RADIO_AGARRADOR = RADIO_TROMPA / 5d;
        private static double LARGO_AGARRADOR = 1;

        // Techo
        private static double ANCHO_TECHO = ANCHO_RECTANGULO + 0.5;
        private static double LARGO_TECHO = LARGO_RECTANULO + 0.5;
        private static double ALTO_TECHO = 0.6;

        #endregion

        // Todo, agrega inclinación
        public Punto PosicionLocomotora { set; get; }

        #region Partes de la locomotora

        private Cuboide rectanguloTechoConductor;
        private Cuboide rectanguloConductor;
        private Cuboide rectanguloBase;
        private Cuboide rectanguloParagolpe;

        #endregion


        #endregion

        /// <summary>
        /// Se debe construir estáticamente porque acá se crea la 
        /// display list, despues se va redibujando en el paint
        /// en cada simulación
        /// </summary>
        public Train()
        {
            this.PosicionLocomotora = new Punto(0, 0, 0);

            rectanguloTechoConductor = new Cuboide(ANCHO_TECHO, LARGO_TECHO, ALTO_TECHO, 32, 32, 32, this.PosicionLocomotora.SumarPunto(new Punto(-ANCHO_TECHO/2d, -0.25, ALTO_RECTANGULO)));
            rectanguloBase = new Cuboide(ANCHO_BASE, LARGO_BASE, ALTO_BASE, 32, 32, 32, this.PosicionLocomotora.SumarPunto(new Punto(-ANCHO_BASE / 2d, 0, -ALTO_BASE)));
            rectanguloConductor = new Cuboide(ANCHO_RECTANGULO, LARGO_RECTANULO, ALTO_RECTANGULO, 32, 32, 32, this.PosicionLocomotora.SumarPunto(new Punto(-ANCHO_RECTANGULO / 2d, 0, 0)));
            rectanguloParagolpe = new Cuboide(ANCHO_PARAGOLPE, LARGO_PARAGOLPE, ALTO_PARAGOLPE, 32, 32, 32, this.PosicionLocomotora.SumarPunto(new Punto(-ANCHO_PARAGOLPE/2d, LARGO_BASE, - ALTO_PARAGOLPE)));
        }

        #region Dibujadores

        public void Draw()
        {
            // Techo del conductor
            this.rectanguloTechoConductor.Draw();

            // Cuadrado del conductor
            rectanguloConductor.Draw();

            // Cilindro
            this.DrawTrompa();

            // Base
            rectanguloBase.Draw();

            // Rectangulo paragolpe
            rectanguloParagolpe.Draw();

            // Agarradores
            this.DrawAgarradores();

            // Ruedas
            // this.DrawRuedas();

            // Triangulo de adelante
            
        }

        private void DrawRuedas()
        {
            throw new NotImplementedException();
        }

        private void DrawAgarradores()
        {            
            this.DrawAgarrador(true);
            this.DrawAgarrador(false);
        }

        private void DrawAgarrador(bool esIzquierdo)
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();

            Gl.glTranslated((esIzquierdo ? -1 : 1) * ANCHO_RECTANGULO / 3d, LARGO_BASE + LARGO_PARAGOLPE + (LARGO_AGARRADOR / 2d), -RADIO_AGARRADOR);
            Gl.glRotated(90, 1, 0, 0);
            
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.20f, 0.20f, 0.35f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.5f, 0.5f, 0.5f, 1 });

            Glu.gluCylinder(quad, RADIO_AGARRADOR, RADIO_AGARRADOR, LARGO_AGARRADOR, 20, 20);
            Gl.glRotated(180, 1, 0, 0);
            
            Glu.gluDisk(quad, 0, RADIO_AGARRADOR , 20, 20);            
            Glu.gluCylinder(quad, RADIO_AGARRADOR * 2d, RADIO_AGARRADOR * 2d, LARGO_AGARRADOR * 0.2, 20, 20);

            Gl.glTranslated(0, 0, LARGO_AGARRADOR * 0.2d);
            Glu.gluDisk(quad, 0, RADIO_AGARRADOR * 2d, 20, 20);
            
            Glu.gluDeleteQuadric(quad);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }

        private void DrawTrompa()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();

            Gl.glTranslated(0, ANCHO_RECTANGULO + (LARGO_TROMPA / 2d), RADIO_TROMPA);
            Gl.glRotated(90, 1, 0, 0);

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, new float[] { 0.20f, 0.20f, 0.35f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.3f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, new float[] { 0.5f, 0.5f, 0.5f, 1 });

            Glu.gluCylinder(quad, RADIO_TROMPA, RADIO_TROMPA, LARGO_TROMPA, 20, 20);
            Gl.glRotated(180, 1, 0, 0);
            Glu.gluDisk(quad, 0, RADIO_TROMPA, 20, 20);

            Glu.gluDeleteQuadric(quad);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }

        
        #endregion
        #region utilitarios

        #endregion
    }
}



