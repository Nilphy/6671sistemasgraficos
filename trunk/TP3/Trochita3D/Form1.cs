using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.FreeGlut;

using Trochita3D.Core;
using Trochita3D.Curvas;
using Trochita3D.Entidades;
using Trochita3D;


namespace TPAlgoritmos3D
{
    public partial class Form1 : Form
    {
        #region Atributos y Propiedades

        Controlador controlador = new Controlador();

        #region ids de display lists

        private int dl_handle;

        public int DL_AXIS
        {
            get { return (dl_handle + 0); }
        }

        public int DL_GRID
        {
            get { return (dl_handle + 1); }
        }

        public int W_WIDTH
        {
            get { return glControl.Width; }
        }

        public int W_HEIGHT
        {
            get { return glControl.Height; }
        }

        #endregion

        #endregion

        // Constructor de la ventana, se ejecuta una sola vez cuando se habre la misma 
        public Form1()
        {
            // Se inicializan los controles de la ventana
            InitializeComponent();
            glControl.InitializeContexts();

            // Se inicializan los componentes de la escena en OpenGL.
            Init();

            // Se configura el Timer para la simulación.
            controlador.InicializarTimer(TimerEventProcessor);
        }

        #region Inicialización

        /// <summary>
        /// Inicializa los controles básicos de la escena a generar en OpenGL.
        /// </summary>
        private void Init()
        {
            dl_handle = Gl.glGenLists(2);

            Gl.glClearColor(0.02f, 0.02f, 0.04f, 0.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            // Crea objetos e inicializa luces 
            // TODO poner la creación de objetos en display lists
            controlador.Escena.Inicializar();

            // Generación de las Display Lists
            Gl.glNewList(DL_AXIS, Gl.GL_COMPILE);
            DrawAxis();
            Gl.glEndList();
            Gl.glNewList(DL_GRID, Gl.GL_COMPILE);
            DrawXYGrid();
            Gl.glEndList();
        }

        #endregion
        #region Eventos

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            this.Set3DEnv();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Glu.gluLookAt(controlador.Camara.eye[0], controlador.Camara.eye[1], controlador.Camara.eye[2], controlador.Camara.at[0], controlador.Camara.at[1], controlador.Camara.at[2], controlador.Camara.up[0], controlador.Camara.up[1], controlador.Camara.up[2]);
            
            // Si corresponde se dibujan los ejes
            if (controlador.view_axis) Gl.glCallList(DL_AXIS);
            // Se corresponde se dibuja la grilla
            if (controlador.view_grid) Gl.glCallList(DL_GRID);

            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_AUTO_NORMAL);

            controlador.Escena.Dibujar();
        }

        protected void RefreshEye() 
        {
            glControl.Refresh();
            return;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    {
                        controlador.Camara.AvanzarAdelante();
                        this.RefreshEye();
                        break;
                    }
                case Keys.Down:
                    {
                        controlador.Camara.AvanzarAtraz();
                        this.RefreshEye();
                        break;
                    }
                case Keys.Left:
                    {
                        controlador.Camara.AvanzarIzquierda();
                        this.RefreshEye();
                        break;
                    }
                case Keys.Right:
                    {
                        controlador.Camara.AvanzarDerecha();
                        this.RefreshEye();
                        break;
                    }
                default: break;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Maneja los eventos del teclado del usuario
        /// 
        /// g = muestra/oculta la grilla
        /// a = muestra/oculta los ejes
        /// l = baja la velocidad de la cámara
        /// h = sube la velocidad de la cámara
        /// r = limpia todos los valores generados por la simulación, inicializando nuevamente la escena
        /// s = inicia la simulación
        /// p = limpia la lista de puntos del camino (detiene la simulación)
        /// o = limpia la lista de puntos del recorrido de la cámara (setea al modo de recorrido de la cámara como fijo)
        /// esc = salir de la aplicación
        /// m = cambia el modo de vista de la camara
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)27:
                    Application.Exit();
                    break;
                case 'g':
                    controlador.view_grid = !controlador.view_grid;
                    glControl.Refresh();
                    break;
                case 'a':
                    controlador.view_axis = !controlador.view_axis;
                    glControl.Refresh();
                    break;
                case 'w':
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
                    glControl.Refresh();
                    break;
                case 'W':
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                    glControl.Refresh();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Evento que se llama cada <code>DELTA_TIEMPO</code>. Dentro de este
        /// evento se simula el paso de DELTA_TIEMPO en el modelo físico.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            glControl.Refresh();
            glControl.Invalidate();
        }

        #endregion
        #region Axis y Grid

        private void DrawAxis()
        {
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glBegin(Gl.GL_LINES);

            // X
            Gl.glColor3d(1, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(15, 0, 0);

            // Y
            Gl.glColor3d(0, 1, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, 15, 0);

            // Z
            Gl.glColor3d(0, 0, 1);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, 0, 15);

            Gl.glEnd();

            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void DrawXYGrid()
        {
            int i;
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glColor3d(0.15d, 0.1d, 0.1d);
            Gl.glBegin(Gl.GL_LINES);
            for (i = -20; i < 21; i++)
            {
                Gl.glVertex3d(i, -20, 0);
                Gl.glVertex3d(i, 20, 0);
                Gl.glVertex3d(-20, i, 0);
                Gl.glVertex3d(20, i, 0);
            }
            Gl.glEnd();
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        #endregion
        #region Configuración de Escena (viewports)

        private void Set3DEnv()
        {
            Gl.glViewport(0, 0, W_WIDTH, W_HEIGHT);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60.0, (float)W_WIDTH / (float)W_HEIGHT, 0.10, 100.0);
        }

        #endregion
    }
}
