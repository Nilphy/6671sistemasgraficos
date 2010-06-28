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


namespace TPAlgoritmos3D
{
    public partial class Form1 : Form
    {
        #region Atributos y Propiedades

        /// <summary>
        /// Constante global que indica el tamaño del paso de tiempo en la simulación 
        /// </summary>
        private const int DELTA_TIEMPO = 5;

        private Timer timer;

        /// <summary>
        /// Variable global que indica si se ve la grilla en la pantalla 
        /// </summary>
        private bool view_grid = true;

        /// <summary>
        /// Variable global que indica si se ven los ejes en la pantalla 
        /// </summary>
        private bool view_axis = true;
                
        private SurfaceInitializer surfaceInitializer = new SurfaceInitializer();
        //private TerrainInitializer terrainInitializer = new TerrainInitializer();

        #region Variables asociadas a única fuente de luz de la escena 

        private float[] light_color = new float[4] { 0.80f, 0.80f, 0.80f, 1.0f };
        private float[] light_position = new float[4] { 0.0f, 0.0f, 5.0f, 0.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        #endregion
        #region Variables internas al proceso de generación de la superficie

        private float[] surface_buffer = null;
        private float[] normals_buffer = null;
        private int curve_points = 0;
        private int dl_handle;

        #endregion 
        #region Mugre de constantes de configuración de la pantalla

        public int TOP_VIEW_POSX
        {
            get { return (int)((float)W_WIDTH * 0.70f); }
        }

        public int TOP_VIEW_POSY
        {
            get { return (int)((float)W_HEIGHT * 0.70f); }
        }

        public int TOP_VIEW_W
        {
            get { return (int)((float)W_WIDTH * 0.30f); }
        }

        public int TOP_VIEW_H
        {
            get { return (int)((float)W_HEIGHT * 0.30f); }
        }

        public int HEIGHT_VIEW_POSX
        {
            get { return (int)((float)W_WIDTH * 0.00f); }
        }

        public int HEIGHT_VIEW_POSY
        {
            get { return (int)((float)W_HEIGHT * 0.70f); }
        }

        public int HEIGHT_VIEW_W
        {
            get { return (int)((float)W_WIDTH * 0.30f); }
        }

        public int HEIGHT_VIEW_H
        {
            get { return (int)((float)W_HEIGHT * 0.30f); }
        }

        public int DL_AXIS
        {
            get { return (dl_handle + 0); }
        }

        public int DL_GRID
        {
            get { return (dl_handle + 1); }
        }

        public int DL_AXIS2D_TOP
        {
            get { return (dl_handle + 2); }
        }

        public int DL_AXIS2D_HEIGHT
        {
            get { return (dl_handle + 3); }
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
        #region Posición defecto de la cámara

        private float[] eye = new float[3] { 13.0f, 0.0f, 20.0f };
        private float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        private float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        #endregion

        Arbol[] arboles = Arbol.GenerarArbolesAleatorios(10);
        PuntoFlotante[] posicionArboles = new PuntoFlotante[10] {
            new PuntoFlotante(0, 0, 0),
            new PuntoFlotante(6, 6, 0),
            new PuntoFlotante(-4, -4, 0),
            new PuntoFlotante(6, 12, 0),
            new PuntoFlotante(-10, 4, 0),
            new PuntoFlotante(8, 15, 0),
            new PuntoFlotante(-15, 7, 0),
            new PuntoFlotante(1, -10, 0),
            new PuntoFlotante(3, -15, 0),
            new PuntoFlotante(15, 15, 0)
        };

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
            InitTimerSimulacion();
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

            // Inicialización de iluminación
            this.InitializeLighting();

            // Inicialización de escena
            this.InitializeScene();

            // Generación de las Display Lists
            Gl.glNewList(DL_AXIS, Gl.GL_COMPILE);
            DrawAxis();
            Gl.glEndList();
            Gl.glNewList(DL_GRID, Gl.GL_COMPILE);
            DrawXYGrid();
            Gl.glEndList();
        }

        private void InitializeScene()
        {
            this.surfaceInitializer = new SurfaceInitializer();
            this.surfaceInitializer.BuildSurface();
        }

        /// <summary>
        /// Inicialización de la iluminación de la escena.
        /// </summary>
        private void InitializeLighting()
        {
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, light_color);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_position);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        /// <summary>
        /// Crea el Timer para realizar la simulación física del modelo.
        /// </summary>
        private void InitTimerSimulacion()
        {
            this.timer = new Timer();
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Interval = DELTA_TIEMPO;
        }

        #endregion
        #region Eventos

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            this.Set3DEnv();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Glu.gluLookAt(eye[0], eye[1], eye[2], at[0], at[1], at[2], up[0], up[1], up[2]);
            
            // Si corresponde se dibujan los ejes
            if (view_axis) Gl.glCallList(DL_AXIS);
            // Se corresponde se dibuja la grilla
            if (view_grid) Gl.glCallList(DL_GRID);

            
            surfaceInitializer.DrawSurface();

            /*
            for (int i = 0; i < arboles.Length; ++i)
            {
                Gl.glPushMatrix();
                PuntoFlotante posicion = posicionArboles[i];
                Gl.glTranslated(posicion.X, posicion.Y, posicion.Z);
                arboles[i].Dibujar();
                Gl.glPopMatrix();
            }
            */
            //TerrainInitializer terrainInitializer = new TerrainInitializer();
            //terrainInitializer.DrawTerrain();
            
        }

        protected void RefreshEye() 
        {
            glControl.Refresh();
            return;
            this.Set3DEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(eye[0], eye[1], eye[2], at[0], at[1], at[2], up[0], up[1], up[2]);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    {
                        eye[0]--;
                        //at[0]--;
                        this.RefreshEye();
                        break;
                    }
                case Keys.Down:
                    {
                        eye[0]++;
                        //at[0]++;
                        this.RefreshEye();
                        break;
                    }
                case Keys.Left:
                    {
                        eye[1]--;
                        //at[1]--;
                        this.RefreshEye();
                        break;
                    }
                case Keys.Right:
                    {
                        eye[1]++;
                        //at[1]++;
                        this.RefreshEye();
                        break;
                    }
                case Keys.Add:
                    {
                        eye[2]--;
                        this.RefreshEye();
                        break;
                    }
                case Keys.Subtract:
                    {
                        eye[2]++;
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
                    view_grid = !view_grid;
                    glControl.Refresh();
                    break;
                case 'a':
                    view_axis = !view_axis;
                    glControl.Refresh();
                    break;
                case 'h':
                    glControl.Refresh();
                    break;
                case 'l':
                    glControl.Refresh();
                    break;
                case 'r':
                    glControl.Refresh();
                    break;
                case 's':
                    glControl.Refresh();
                    break;
                case 'o':
                    glControl.Refresh();
                    break;
                case 'm':
                    glControl.Refresh();
                    break;
                case 'p':
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

        private void glControl_MouseClick(object sender, MouseEventArgs e)
        {

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
        #region Armado y Dibujo de Superficie

        private void BuildSurface(float[] vertex_buffers, int nr_points)
        {
            float dx, dy, dz;
            curve_points = nr_points;

            if (surface_buffer != null)
                surface_buffer = null;
            surface_buffer = new float[nr_points * 6];
            if (normals_buffer != null)
                normals_buffer = null;
            normals_buffer = new float[nr_points * 3];

            for (int i = 0; i < nr_points; i++)
            {
                surface_buffer[i * 3 + 0] = 1.0f;
                surface_buffer[i * 3 + 1] = vertex_buffers[i * 2 + 0];
                surface_buffer[i * 3 + 2] = vertex_buffers[i * 2 + 1];

                surface_buffer[nr_points * 3 + i * 3 + 0] = -1.0f;
                surface_buffer[nr_points * 3 + i * 3 + 1] = vertex_buffers[i * 2 + 0];
                surface_buffer[nr_points * 3 + i * 3 + 2] = vertex_buffers[i * 2 + 1];
            }

            for (int i = 0; i < (nr_points - 1); i++)
            {
                dx = 0.0f;
                dy = vertex_buffers[(i + 1) * 2 + 0] - vertex_buffers[i * 2 + 0];
                dz = vertex_buffers[(i + 1) * 2 + 1] - vertex_buffers[i * 2 + 1];
                normals_buffer[i * 3 + 0] = dx;
                normals_buffer[i * 3 + 1] = -dz;
                normals_buffer[i * 3 + 2] = dy;
            }

            normals_buffer[(nr_points - 1) * 3 + 0] = normals_buffer[(nr_points - 2) * 3 + 0];
            normals_buffer[(nr_points - 1) * 3 + 1] = normals_buffer[(nr_points - 2) * 3 + 1];
            normals_buffer[(nr_points - 1) * 3 + 2] = normals_buffer[(nr_points - 2) * 3 + 2];
        }

        private void DrawSurface()
        {
            if (surface_buffer != null && normals_buffer != null)
            {
                Gl.glEnable(Gl.GL_LIGHT0);
                Gl.glPushMatrix();
                Gl.glBegin(Gl.GL_QUAD_STRIP);
                for (int i = 0; i < curve_points; i++)
                {
                    Gl.glNormal3fv(ref normals_buffer[i * 3]);
                    Gl.glVertex3fv(ref surface_buffer[i * 3]);
                    Gl.glVertex3fv(ref surface_buffer[3 * curve_points + i * 3]);
                }
                Gl.glEnd();
                Gl.glPopMatrix();
            }
        }

        #endregion
        #region Dibujado de Viewports (marcos y axis)

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

        private void SetSceneWindow()
        {
            Gl.glViewport(0, 0, W_WIDTH, W_WIDTH / 2);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-10, 10, -5.0, 5.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
        }

        private void SetCamera()
        {
            Gl.glViewport(TOP_VIEW_POSX, TOP_VIEW_POSY, TOP_VIEW_W, TOP_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.0, 1.0, -0.0, 1.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_TOP);
        }

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
