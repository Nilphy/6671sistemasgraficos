﻿using System;
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
using Common.Utils;


namespace TPAlgoritmos3D
{
    public partial class Form1 : Form
    {
        #region Atributos y Propiedades

        Controlador controlador = new Controlador();

        public int DELTA_TIEMPO = 15;
        public Double PASO_TIEMPO = 0.001;
        private Timer timer;

        private const int WIDTH_ESCENA = 400;
        private const int HEIGHT_ESCENA = 400;

        private int MAXIMA_COORDENADA = 100;
        private bool mousing = false;

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
        }

        #region Inicialización

        /// <summary>
        /// Inicializa los controles básicos de la escena a generar en OpenGL.
        /// </summary>
        private void Init()
        {
            // Capturar movimiento del mouse
            this.glControl.MouseDown += new MouseEventHandler(this.glControl_OnMouseDown);
            this.glControl.MouseUp += new MouseEventHandler(this.glControl_OnMouseUp);

            dl_handle = Gl.glGenLists(2);

            Gl.glClearColor(0.02f, 0.02f, 0.04f, 0.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_NORMALIZE);

            this.Set3DEnv();

            // Crea objetos e inicializa luces 
            // TODO poner la creación de objetos en display lists
            controlador.Escena.Inicializar(480, 480);

            // Se configura el Timer para la simulación.
            this.InicializarTimer(TimerEventProcessor);

            // Generación de las Display Lists
            Gl.glNewList(DL_AXIS, Gl.GL_COMPILE);
            DrawAxis();
            Gl.glEndList();
            Gl.glNewList(DL_GRID, Gl.GL_COMPILE);
            DrawXYGrid();
            Gl.glEndList();
        }

        public void InicializarTimer(EventHandler TimerEventProcessor)
        {
            this.timer = new Timer();
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Interval = DELTA_TIEMPO;
            timer.Start();
        }

        #endregion
        #region Eventos

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            controlador.Escena.Camara.UpdateCameraByMouse(mousing);
            controlador.Escena.Camara.Look();

            controlador.Escena.DibujarSkybox();
            
            // Si corresponde se dibujan los ejes
            if (controlador.view_axis) Gl.glCallList(DL_AXIS);
            // Se corresponde se dibuja la grilla
            if (controlador.view_grid) Gl.glCallList(DL_GRID);

            controlador.Escena.Dibujar();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    {
                        controlador.Escena.Camara.MoveCamera(1);
                        break;
                    }
                case Keys.Down:
                    {
                        controlador.Escena.Camara.MoveCamera(-1);
                        break;
                    }
                case Keys.Left:
                    {
                        controlador.Escena.Camara.SlideCamera(1, 0);
                        break; 
                    }
                case Keys.Right:
                    {
                        controlador.Escena.Camara.SlideCamera(-1, 0);
                        break;
                    }
                case Keys.Add:
                    {
                        controlador.Escena.Camara.SlideCamera(0, 1);
                        break;
                        
                    }
                case Keys.Subtract:
                    {
                        controlador.Escena.Camara.SlideCamera(0, -1);
                        break;
                        
                    }
                default: break;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

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
                case 'm':
                    controlador.Escena.SwitchDayLight();
                    glControl.Refresh();
                    break;
                case 'v':
                    controlador.PonerCamaraAerea();
                    glControl.Refresh();
                    break;
                case 'u':
                    controlador.PonerCamaraTerreno();
                    glControl.Refresh();
                    break;
                case 'l':
                    controlador.PonerCamaraLocomotora();
                    glControl.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void glControl_OnMouseDown(object sender, MouseEventArgs e)
        {
            mousing = true;
        }

        private void glControl_OnMouseUp(object sender, MouseEventArgs e)
        {
            mousing = false;
        }

        /// <summary>
        /// Evento que se llama cada <code>DELTA_TIEMPO</code>. Dentro de este
        /// evento se simula el paso de DELTA_TIEMPO en el modelo físico.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            controlador.Escena.Simular(this.PASO_TIEMPO);
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
            for (i = -MAXIMA_COORDENADA; i < MAXIMA_COORDENADA+1; i++)
            {
                Gl.glVertex3d(i, -MAXIMA_COORDENADA, 0);
                Gl.glVertex3d(i, MAXIMA_COORDENADA, 0);
                Gl.glVertex3d(-MAXIMA_COORDENADA, i, 0);
                Gl.glVertex3d(MAXIMA_COORDENADA, i, 0);
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
            Glu.gluPerspective(60.0, (float)W_WIDTH / (float)W_HEIGHT, 0.10, 1000.0);
        }

        #endregion
        
    }
}