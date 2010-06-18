using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace Trochita3D.Entidades
{
    public class Riel
    {

        public Riel()
        {

        }

        public void Dibujar()
        {
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glPushMatrix();

            Gl.glColor3d(0.5, 0.5, 0.5);

            Gl.glVertex3d(0, -0.5, 0);
            Gl.glVertex3d(0, 0.5, 0);
            Gl.glVertex3d(0, 0.5, 0.3);
            Gl.glVertex3d(0, 0.3, 0.3);
            Gl.glVertex3d(0, 0.3, 0.65);
            Gl.glVertex3d(0, 0.5, 0.65);
            Gl.glVertex3d(0, 0.5, 1);
            Gl.glVertex3d(0, -0.5, 1);
            Gl.glVertex3d(0, -0.5, 0.65);
            Gl.glVertex3d(0, -0.3, 0.65);
            Gl.glVertex3d(0, -0.3, 0.3);
            Gl.glVertex3d(0, -0.5, 0.3);
            Gl.glVertex3d(0, -0.5, 0);

            Gl.glPopMatrix();
            Gl.glEnd();
        }

    }
}
