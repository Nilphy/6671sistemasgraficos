using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.Platform.Windows;
using Tao.OpenGl;

namespace TPAlgoritmos2D
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            glControl.InitializeContexts();
            Initialize2DConfiguration();
        }

        private void Initialize2DConfiguration()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, 800, 0, 600, -1, 1);
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glBegin(Gl.GL_POINTS);

            // TODO: dibujar la escena.

            Gl.glEnd();
        }
    }
}
