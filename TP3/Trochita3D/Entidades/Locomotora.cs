using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Trochita3D.Entidades;
using Tao.FreeGlut;

namespace Trochita3D.Core
{
    public class Train
    {
        #region Atributos y propiedades

        #region Medidas

        private static double ALTO_VIAS = 2.4d;

        #region  Rectángulo conductor

        private static double ANCHO_RECTANGULO = 1d;
        private static double LARGO_RECTANULO = 1d;
        private static double ALTO_RECTANGULO = 1.7d;

        #endregion
        #region Trompa

        private static double RADIO_TROMPA = (ANCHO_RECTANGULO - ANCHO_RECTANGULO/4d)/2d;
        private static double LARGO_TROMPA = LARGO_RECTANULO * 2d;

        #endregion
        #region Base

        private static double ANCHO_BASE = ANCHO_RECTANGULO + ANCHO_RECTANGULO / 10d;
        private static double ALTO_BASE = ALTO_RECTANGULO/20d;
        private static double LARGO_BASE = LARGO_RECTANULO*2d;

        #endregion
        #region Paragolpe

        private static double ANCHO_PARAGOLPE = ANCHO_BASE;
        private static double ALTO_PARAGOLPE = ALTO_RECTANGULO / 5d;
        private static double LARGO_PARAGOLPE = LARGO_RECTANULO / 40d;

        #endregion
        #region Agarrador

        private static double RADIO_AGARRADOR = RADIO_TROMPA / 5d;
        private static double LARGO_AGARRADOR = LARGO_TROMPA / 4d;

        #endregion
        #region Techo

        private static double ANCHO_TECHO = ANCHO_RECTANGULO + 0.5d;
        private static double LARGO_TECHO = LARGO_RECTANULO + 0.5d;
        private static double ALTO_TECHO = ALTO_RECTANGULO / 10d;

        #endregion
        #region Guardabarros

        private static double ALTO_GUARDABARROS = ALTO_PARAGOLPE;
        private static double LARGO_GUARDABARROS = ANCHO_BASE / 2.5d;
        private static double ANCHO_GUARDABARROS = ANCHO_PARAGOLPE;

        #endregion
        #region Ruedas

        private static double ESPACIADO_RUEDAS =  0.4d;
        private static double RADIO_INTERNO_RUEDAS = RADIO_EXTERNO_RUEDAS * 0.2d;
        private static double RADIO_EXTERNO_RUEDAS = (ALTO_PARAGOLPE + ALTO_GUARDABARROS) / 2d;
        private static double ANCHO_RUEDAS = 0.2d;

        #endregion
        #region Luz

        private static double ALTO_SOSTENEDOR_LUZ = RADIO_TROMPA * 0.2d;
        private static double RADIO_SOSTENEDOR_LUZ = ANCHO_BASE * 0.03d;
        private static double RADIO_CONTENEDOR_LUZ = RADIO_SOSTENEDOR_LUZ * 2.5d;
        private static double ANCHO_CONTENEDOR_LUZ = LARGO_BASE * 0.05d;
        private static double RADIO_LUZ = RADIO_CONTENEDOR_LUZ * 0.5;

        private float[] luzEmicionPrendida = { 0.8f, 1.0f, 0.2f, 1.0f };
        private float[] luzEmicionApagada = { 0.0f, 0.0f, 0.0f, 1.0f };
        private float[] light_linterna_position = new float[4] { 0.0f, 0.0f, 0.3f, 1.0f};
        private float[] light_linterna_direction = new float[4] { 0.0f, 1.0f, -0.3f, 1.0f};

        #endregion
        #endregion
        #region Partes de la locomotora

        private Cuboide rectanguloTechoConductor;
        private Cuboide rectanguloConductor;
        private Cuboide rectanguloBase;
        private Cuboide rectanguloParagolpe;
        private IList<Rueda> ruedas;
        private GuardabarroTren guardabarroTren;
        
        #endregion
        #region Parámetros que se setean de afuera del tren 

        public Punto Posicion { set; get; }
        public double AnguloRotacionRuedas { set; get; }
        public double InclinaciónLocomotora { set; get; }

        public float[] LuzBrillo { set; get; }
        public int Shininess { set; get; }

        #endregion
        #region Colores

        private static float[] LUZ_ROJO = new float[] { 0.5f, 0.1f, 0.1f, 1 };
        private static float[] LUZ_VERDE = new float[] { 0.2f, 0.5f, 0.2f, 1 };
        private static float[] LUZ_AZUL = new float[] { 0.2f, 0.3f, 0.5f, 1 };
        private static float[] LUZ_NEGRO = new float[] { 0.2f, 0.2f, 0.2f, 1 };

        #endregion

        #endregion

        public Train(float[] luzBrillo, int shininess)
        {
            this.LuzBrillo = luzBrillo;
            this.Shininess = shininess;

            rectanguloTechoConductor = new Cuboide(ANCHO_TECHO, LARGO_TECHO, ALTO_TECHO, new Punto(-ANCHO_TECHO/2d, -0.25, ALTO_RECTANGULO), LUZ_ROJO, this.LuzBrillo, LUZ_ROJO, this.Shininess);
            rectanguloBase = new Cuboide(ANCHO_BASE, LARGO_BASE, ALTO_BASE, new Punto(-ANCHO_BASE / 2d, 0, -ALTO_BASE), LUZ_ROJO, this.LuzBrillo, LUZ_ROJO, this.Shininess);
            rectanguloConductor = new Cuboide(ANCHO_RECTANGULO, LARGO_RECTANULO, ALTO_RECTANGULO, new Punto(-ANCHO_RECTANGULO / 2d, 0, 0), LUZ_AZUL, this.LuzBrillo, LUZ_AZUL, this.Shininess);
            rectanguloParagolpe = new Cuboide(ANCHO_PARAGOLPE, LARGO_PARAGOLPE, ALTO_PARAGOLPE, new Punto(-ANCHO_PARAGOLPE / 2d, LARGO_BASE, -ALTO_PARAGOLPE), LUZ_ROJO, this.LuzBrillo, LUZ_ROJO, this.Shininess);

            ruedas = new List<Rueda>();

            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(true, true), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, LUZ_NEGRO, this.LuzBrillo, LUZ_NEGRO, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(true, false), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, LUZ_NEGRO, this.LuzBrillo, LUZ_NEGRO, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(false, true), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, LUZ_NEGRO, this.LuzBrillo, LUZ_NEGRO, this.Shininess));
            ruedas.Add(new Rueda(this.CalcularPuntoCentroRueda(false, false), AnguloRotacionRuedas, RADIO_INTERNO_RUEDAS, RADIO_EXTERNO_RUEDAS, ANCHO_RUEDAS, LUZ_NEGRO, this.LuzBrillo, LUZ_NEGRO, this.Shininess));

            guardabarroTren = new GuardabarroTren(ANCHO_GUARDABARROS, LARGO_GUARDABARROS, ALTO_GUARDABARROS, new Punto(-ANCHO_GUARDABARROS / 2d, LARGO_BASE + LARGO_PARAGOLPE, -ALTO_PARAGOLPE - ALTO_GUARDABARROS + 0.2), LUZ_ROJO, LUZ_ROJO, this.LuzBrillo, this.Shininess);
        }

        #region Dibujadores

        public void Draw(bool linternaPrendida, CamaraLocomotora camara)
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, ALTO_BASE + (RADIO_EXTERNO_RUEDAS * 2d) + ALTO_VIAS);
            if (this.Posicion != null) Gl.glTranslated(this.Posicion.X, this.Posicion.Y, this.Posicion.Z);
            if (this.InclinaciónLocomotora != 0) Gl.glRotated(this.InclinaciónLocomotora, 0, 0, 1);

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

            // Guardabarro
            this.guardabarroTren.Draw();

            // Linterna
            this.DrawLinterna(linternaPrendida);


            if (camara != null)
            {
                camara.Eye = this.Posicion.SumarPunto(new Punto(3, 0, 5).RotarProyeccionXY(this.InclinaciónLocomotora + 90));
                camara.At = this.Posicion.SumarPunto(new Punto(5, 0, 4.5).RotarProyeccionXY(this.InclinaciónLocomotora + 90));
                camara.Look();
            }

            Gl.glPopMatrix();
        }
       
        private void DrawLinterna(bool prenderLinterna)
        {   
            float[] locomotora_light_color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();

            Gl.glTranslated(0, LARGO_BASE - LARGO_BASE * 0.1d, RADIO_TROMPA * 2d);

            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_POSITION, light_linterna_position);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPOT_DIRECTION, light_linterna_direction);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_DIFFUSE, locomotora_light_color);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_AMBIENT, new float[4] {0f, 0f, 0f, 1f});
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPECULAR, new float[4] { 1f, 1f, 1f, 1f });
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_LINEAR_ATTENUATION, new float[] {.01f});
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPOT_CUTOFF, new float[] { 30.0f });
            
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, LUZ_ROJO);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, LUZ_ROJO);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, this.LuzBrillo);
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, this.Shininess);

            Glu.gluCylinder(quad, RADIO_SOSTENEDOR_LUZ, RADIO_SOSTENEDOR_LUZ, ALTO_SOSTENEDOR_LUZ, 20, 20);

            Gl.glTranslated(0, ANCHO_CONTENEDOR_LUZ * 0.5d, ALTO_SOSTENEDOR_LUZ + RADIO_CONTENEDOR_LUZ);
            Gl.glRotated(90, 1, 0, 0);            
            Glu.gluCylinder(quad, RADIO_CONTENEDOR_LUZ, RADIO_CONTENEDOR_LUZ, ANCHO_CONTENEDOR_LUZ, 20, 20);

            Gl.glTranslated(0, 0, ANCHO_CONTENEDOR_LUZ);
            Glu.gluDisk(quad, 0, RADIO_CONTENEDOR_LUZ, 20, 20);

            if (prenderLinterna) Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, this.luzEmicionPrendida);
            Gl.glTranslated(0, 0, -(ANCHO_CONTENEDOR_LUZ * 0.8d));
            Gl.glRotated(180, 1, 0, 0);
            //Gl.glScaled(0.5d, 1, 2);
            Glu.gluDisk(quad, 0, RADIO_CONTENEDOR_LUZ, 20, 20);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, this.luzEmicionApagada);

            Glu.gluDeleteQuadric(quad);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }

        private void DrawRuedas()
        {
            foreach (Rueda rueda in ruedas)
            {
                rueda.AnguloRotacion = this.AnguloRotacionRuedas;
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
            
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, LUZ_AZUL);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, LUZ_AZUL);
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

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, LUZ_VERDE);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, LUZ_VERDE);
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