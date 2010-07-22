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
        private int skybox_left;
        private int skybox_front;
        private int skybox_right;
        private int skybox_back;
        private int skybox_up;

        public Skybox()
        {
            this.LoadImage(out skybox_left, @"../../Imagenes/Texturas/Skybox/mpa23lf.bmp");
            this.LoadImage(out skybox_front, @"../../Imagenes/Texturas/Skybox/mpa23ft.bmp");
            this.LoadImage(out skybox_right, @"../../Imagenes/Texturas/Skybox/mpa23rt.bmp");
            this.LoadImage(out skybox_back, @"../../Imagenes/Texturas/Skybox/mpa23bk.bmp");
            this.LoadImage(out skybox_up, @"../../Imagenes/Texturas/Skybox/mpa23up.bmp");
        }

        private void LoadImage(out int idTexture, string fileName)
        {
            Gl.glGenTextures(1, out idTexture);
            Bitmap bmp = new Bitmap(fileName);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, idTexture);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, bmp.Width, bmp.Height, 0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bmpData.Scan0);

            //Unlock the bits.
            bmp.UnlockBits(bmpData);
            //Get rid of the image.
            bmp.Dispose();
        }

        public void Dibujar()
        {
            Gl.glPushMatrix();
            //Gl.glLoadIdentity();
            //Gl.glPushAttrib(Gl.GL_ENABLE_BIT);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_LIGHTING);
            //Gl.glDisable(Gl.GL_BLEND);
            Gl.glColor4d(1, 1, 1, 1);
            double compNorm = 90 / (new Punto(90, 90, 90)).Modulo();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skybox_left);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glNormal3d(-compNorm, compNorm, compNorm); Gl.glVertex3d(90, -90, -90);
            Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(90, 90, -90);
            Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, -compNorm, -compNorm); Gl.glVertex3d(90, 90, 90);
            Gl.glTexCoord2d(0, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(90, -90, 90);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skybox_front);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, compNorm, compNorm); Gl.glVertex3d(-90, -90, -90);
            Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, compNorm, compNorm); Gl.glVertex3d(90, -90, -90);
            Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(90, -90, 90);
            Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-90, -90, 90);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skybox_right);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, -compNorm, compNorm); Gl.glVertex3d(-90, 90, -90);
            Gl.glTexCoord2d(1, 0); Gl.glNormal3d(compNorm, compNorm, compNorm); Gl.glVertex3d(-90, -90, -90);
            Gl.glTexCoord2d(1, 1); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-90, -90, 90);
            Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, -compNorm, -compNorm); Gl.glVertex3d(-90, 90, 90);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skybox_back);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(90, 90, -90);
            Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, -compNorm, compNorm); Gl.glVertex3d(-90, 90, -90);
            Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, -compNorm, -compNorm); Gl.glVertex3d(-90, 90, 90);
            Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, -compNorm, -compNorm); Gl.glVertex3d(90, 90, 90);
            Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, skybox_up);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-90, -90, 90);
            Gl.glTexCoord2d(1, 0); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(90, -90, 90);
            Gl.glTexCoord2d(1, 1); Gl.glNormal3d(-compNorm, compNorm, -compNorm); Gl.glVertex3d(90, -90, 90);
            Gl.glTexCoord2d(0, 1); Gl.glNormal3d(compNorm, compNorm, -compNorm); Gl.glVertex3d(-90, -90, 90);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_LIGHTING);
            //Gl.glPopAttrib();
            Gl.glPopMatrix();
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
