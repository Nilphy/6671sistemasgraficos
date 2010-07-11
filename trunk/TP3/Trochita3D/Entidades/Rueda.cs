using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Tao.OpenGl;

namespace Trochita3D.Entidades
{
    public class Rueda
    {
        public Punto Centro { set; get; }
        public Double AnguloRotacion { set; get; }
        public Double RadioInterno { set; get; }
        public Double RadioExterno { set; get; }
        public Double Ancho { set; get; }
        public float[] Luz { set; get; }
        public float[] LuzAmbiente { set; get; }
        public float[] LuzBrillo { set; get; }
        public int Shininess { set; get; }

        public Rueda(
            Punto centro, 
            Double anguloRotacion, 
            double radioInterno, 
            double radioExterno, 
            double ancho,
            float[] luz,
            float[] luzBrillo, 
            float[] luzAmbiente,
            int shininess)
        {
            this.Centro = centro;
            this.AnguloRotacion = anguloRotacion;
            this.RadioExterno = radioExterno;
            this.RadioInterno = radioInterno;
            this.Luz = luz;
            this.LuzAmbiente = luzAmbiente;
            this.LuzBrillo = luzBrillo;
            this.Ancho = ancho;
            this.Shininess = shininess;
        }

        public void Draw()
        {
            Glu.GLUquadric quad = Glu.gluNewQuadric();
                        
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, LuzAmbiente);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, Luz);
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, LuzBrillo);
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, this.Shininess);

            Gl.glTranslated(this.Centro.X, this.Centro.Y, this.Centro.Z);
            Gl.glRotated(this.AnguloRotacion * 180 / Math.PI, 1, 0, 0);
            Gl.glRotated(90, 0, 1, 0);
            
            Glu.gluCylinder(quad, this.RadioExterno, this.RadioExterno, Ancho, 20, 20);

            Gl.glRotated(180, 0, 1, 0);   

            Glu.gluDisk(quad, this.RadioInterno, this.RadioExterno, 20, 20);
            Glu.gluDisk(quad, 0, this.RadioInterno, 20, 20);

            Gl.glRotated(180, 0, 1, 0);   

            Gl.glTranslated(0, 0, Ancho);
         
            Glu.gluDisk(quad, this.RadioInterno, this.RadioExterno, 20, 20);                        
            Glu.gluDisk(quad, 0, this.RadioInterno, 20, 20);

            this.DibujarBarra();

            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
            Glu.gluDeleteQuadric(quad);
        }

        private void DibujarBarra()
        {
            Gl.glColor3d(0, 0, 1);
            double size = this.RadioExterno;
            double height = size;
            double width = size / 2d;
            double depth = size / 2d;

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, new float[] { 0.3f, 0.2f, 0.1f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[] { 0.3f, 0.2f, 0.1f, 1 });
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[] { 0.2f, 0.2f, 0.2f, 1 });
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 10);

            // Centro la barra.
            Gl.glTranslated(-depth / 2d, -width / 2d, - (Ancho/2d) - (height /2d));

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glNormal3f(0, 0, -1);

            // Tapa de abajo
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(depth, 0, 0);
            Gl.glVertex3d(depth, width, 0);
            Gl.glVertex3d(0, width, 0);

            Gl.glNormal3f(0, 0, 1);

            // Tapa de arriba
            Gl.glVertex3d(0, 0, height);
            Gl.glVertex3d(depth, 0, height);
            Gl.glVertex3d(depth, width, height);
            Gl.glVertex3d(0, width, height);

            Gl.glNormal3f(-1, 0, 0);

            // Tapa de atras
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, width, 0);
            Gl.glVertex3d(0, width, height);
            Gl.glVertex3d(0, 0, height);

            Gl.glNormal3f(0, 1, 0);

            // Tapa de derecha
            Gl.glVertex3d(0, width, 0);
            Gl.glVertex3d(depth, width, 0);
            Gl.glVertex3d(depth, width, height);
            Gl.glVertex3d(0, width, height);

            Gl.glNormal3f(0, 1, 0);

            // Tapa de adelante
            Gl.glVertex3d(depth, 0, 0);
            Gl.glVertex3d(depth, width, 0);
            Gl.glVertex3d(depth, width, height);
            Gl.glVertex3d(depth, 0, height);

            Gl.glNormal3f(0, 1, 0);

            // Tapa de izquierda
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(depth, 0, 0);
            Gl.glVertex3d(depth, 0, height);
            Gl.glVertex3d(0, 0, height);
            
            Gl.glEnd();
        }
    }
}
