using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using System.Drawing;
using System.Drawing.Imaging;

namespace Trochita3D.Core
{
    public class Skybox
    {
        private Textura left;
        private Textura front;
        private Textura right;
        private Textura back;
        private Textura up;
        private Textura down;

        private int size;

        public Skybox(int size)
        {
            this.size = size;
            this.left = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23lf.bmp");
            this.front = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23ft.bmp");
            this.right = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23rt.bmp");
            this.back = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23bk.bmp");
            this.up = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23up.bmp");
            this.down = new Textura(@"../../Imagenes/Texturas/Skybox/mpa23dn.bmp");
        }

        public void Dibujar()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glColor4d(1, 1, 1, 1);
            double compNorm = 90 / (new Punto(90, 90, 90)).Modulo();
            double dimension = size / 2;

            left.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(-compNorm, compNorm, compNorm); Gl.glVertex3d(dimension, -dimension, -dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(dimension, dimension, -dimension);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, -compNorm, -compNorm); Gl.glVertex3d(dimension, dimension, dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(dimension, -dimension, dimension);
            Gl.glEnd();

            front.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, compNorm, compNorm); Gl.glVertex3d(-dimension, -dimension, -dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, compNorm, compNorm); Gl.glVertex3d(dimension, -dimension, -dimension);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(dimension, -dimension, dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-dimension, -dimension, dimension);
            Gl.glEnd();

            right.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, -compNorm, compNorm); Gl.glVertex3d(-dimension, dimension, -dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(compNorm, compNorm, compNorm); Gl.glVertex3d(-dimension, -dimension, -dimension);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-dimension, -dimension, dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, -compNorm, -compNorm); Gl.glVertex3d(-dimension, dimension, dimension);
            Gl.glEnd();

            back.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(dimension, dimension, -dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(compNorm, -compNorm, compNorm); Gl.glVertex3d(-dimension, dimension, -dimension);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(compNorm, -compNorm, -compNorm); Gl.glVertex3d(-dimension, dimension, dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(-compNorm, -compNorm, -compNorm); Gl.glVertex3d(dimension, dimension, dimension);
            Gl.glEnd();

            up.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, -compNorm, -compNorm); Gl.glVertex3d(-dimension, dimension, dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-dimension, -dimension, dimension);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(dimension, -dimension, dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(-compNorm, -compNorm, -compNorm); Gl.glVertex3d(dimension, dimension, dimension);
            Gl.glEnd();

            down.Activate();
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(1, 1); Gl.glNormal3d(compNorm, compNorm, compNorm); Gl.glVertex3d(-dimension, -dimension, -dimension);
                Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, compNorm, compNorm); Gl.glVertex3d(dimension, -dimension, -dimension);
                Gl.glTexCoord2d(0, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(dimension, dimension, -dimension);
                Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, -compNorm, compNorm); Gl.glVertex3d(-dimension, dimension, -dimension);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void DibujarNormales()
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            double mod = (new Punto(90, 90, 90)).Modulo();

            Gl.glPushMatrix();
            Gl.glTranslated(90, -90, -90);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3d(1, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0.2, 0, 0);
            Gl.glVertex3d(-90 / mod, 90 / mod, 90 / mod);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(90, 90, -90);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3d(1, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0.2, 0, 0);
            Gl.glVertex3d(-90 / mod, -90 / mod, 90 / mod);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(90, 90, 90);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3d(1, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0.2, 0, 0);
            Gl.glVertex3d(-90 / mod, -90 / mod, -90 / mod);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glEnable(Gl.GL_LIGHTING);
        }

    }
}
