using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Trochita3D.Entidades;

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

        // Guardabarros
        private static double ALTO_GUARDABARROS = ALTO_PARAGOLPE;
        private static double LARGO_GUARDABARROS = ANCHO_BASE / 2.5d;
        private static double ANCHO_GUARDABARROS = ANCHO_PARAGOLPE;

        // Ruedas
        private static double ESPACIADO_RUEDAS = ANCHO_BASE / 2d; // Esto tiene que ir con la distancia entre los rieles
        private static double RADIO_INTERNO_RUEDAS = (ALTO_PARAGOLPE + ALTO_GUARDABARROS) / 2d;
        private static double RADIO_EXTERNO_RUEDAS = RADIO_INTERNO_RUEDAS + 0.1d;
        private static double ANCHO_RUEDAS = 0.8d;

        #endregion
        #region Partes de la locomotora

        private Cuboide rectanguloTechoConductor;
        private Cuboide rectanguloConductor;
        private Cuboide rectanguloBase;
        private Cuboide rectanguloParagolpe;
        private IList<Rueda> ruedas;
        private GuardabarroTren guardabarroTren;
        
        #endregion

        private double AnguloRotacionRuedas { set; get; }
        public double InclinaciónLocomotora { set; get; }
        public float[] LuzAmbiente { set; get; }
        public float[] LuzBrillo { set; get; }
        public float[] Luz { set; get; }
        public int Shininess { set; get; }

        public Punto Posicion { set; get; }
        public double AnguloRotacion { set; get; }

        #endregion

        public Train(float[] luzAmbiente, float[] luzBrillo, float[] luz, int shininess)
        {
            this.Luz = luz;
            this.LuzAmbiente = luzAmbiente;
            this.LuzBrillo = luzBrillo;
            this.Shininess = shininess;

            rectanguloTechoConductor = new Cuboide(ANCHO_TECHO, LARGO_TECHO, ALTO_TECHO, 32, 32, 32, new Punto(-ANCHO_TECHO/2d, -0.25, ALTO_RECTANGULO), this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess);
            rectanguloBase = new Cuboide(ANCHO_BASE, LARGO_BASE, ALTO_BASE, 32, 32, 32, new Punto(-ANCHO_BASE / 2d, 0, -ALTO_BASE), this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess);
            rectanguloConductor = new Cuboide(ANCHO_RECTANGULO, LARGO_RECTANULO, ALTO_RECTANGULO, 32, 32, 32, new Punto(-ANCHO_RECTANGULO / 2d, 0, 0), this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess);
            rectanguloParagolpe = new Cuboide(ANCHO_PARAGOLPE, LARGO_PARAGOLPE, ALTO_PARAGOLPE, 32, 32, 32, new Punto(-ANCHO_PARAGOLPE / 2d, LARGO_BASE, -ALTO_PARAGOLPE), this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess);

            ruedas = new List<Rueda>();

            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(true, true), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(true, false), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(false, true), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(false, false), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));

            guardabarroTren = new GuardabarroTren(ANCHO_GUARDABARROS, LARGO_GUARDABARROS, ALTO_GUARDABARROS, 32, 32, 32, new Punto(-ANCHO_GUARDABARROS / 2d, LARGO_BASE + LARGO_PARAGOLPE, -ALTO_PARAGOLPE - ALTO_GUARDABARROS + 0.2), this.Luz, this.LuzAmbiente, this.LuzBrillo, this.Shininess);
        }

        #region Dibujadores

        public void Draw()
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glTranslated(this.Posicion.X, this.Posicion.Y, this.Posicion.Z);
            Gl.glRotated(this.AnguloRotacion, 0, 0, 1);

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
            this.DrawRuedas();

            // Triangulo de adelante
            this.guardabarroTren.Draw();
        }

        private void DrawRuedas()
        {
            foreach (Rueda rueda in ruedas)
            {
                rueda.Draw();
            }
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
            
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, this.LuzAmbiente);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, this.Luz);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, this.LuzBrillo);
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, this.Shininess);

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

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, this.LuzAmbiente);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, this.Luz);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, this.LuzBrillo);
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, this.Shininess);

            Glu.gluCylinder(quad, RADIO_TROMPA, RADIO_TROMPA, LARGO_TROMPA, 20, 20);
            Gl.glRotated(180, 1, 0, 0);
            Glu.gluDisk(quad, 0, RADIO_TROMPA, 20, 20);

            Glu.gluDeleteQuadric(quad);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }
        
        #endregion
        #region utilitarios

        private Punto CalcularPuntoCentroRueda(bool esDerecha, bool esDelantera)
        {
            double coordenadaX;
            double coordenadaY;
            double coordenadaZ;

            coordenadaZ = -(RADIO_EXTERNO_RUEDAS + ALTO_BASE);

            if (esDerecha) coordenadaX = (ESPACIADO_RUEDAS / 2d);
            else coordenadaX = - ((ESPACIADO_RUEDAS / 2d) + ANCHO_RUEDAS);

            if (esDelantera) coordenadaY = (LARGO_BASE / 4d) * 3d;
            else coordenadaY = LARGO_BASE / 4d;

            return new Punto(coordenadaX, coordenadaY, coordenadaZ); 
        }

        #endregion
    }
}