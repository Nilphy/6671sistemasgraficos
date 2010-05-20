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
using Tao.FreeGlut;
using Modelo;
using SistemasGraficos.Entidades;
using System.Collections;
using SistemasGraficos.EstrategiasDibujo;

namespace TPAlgoritmos3D
{
    public partial class Form1 : Form
    {
        #region Atributos y Propiedades

        /// <summary>
        /// Constante global que indica el tamaño del paso de tiempo en la simulación 
        /// </summary>
        private const int DELTA_TIEMPO = 5;

        /// <summary>
        /// Variable global que representa al modelo 
        /// </summary>
        public Escena escena = new Escena();

        /// <summary>
        /// Variable global que representa a la vista 
        /// </summary>
        public Vista vista;

        private Timer timer;

        /// <summary>
        /// Variable global que indica si se ve la grilla en la pantalla 
        /// </summary>
        private bool view_grid = true;

        /// <summary>
        /// Variable global que indica si se ven los ejes en la pantalla 
        /// </summary>
        private bool view_axis = true;

        /// <summary>
        /// Variable que indica si la simulación debe estar detenida o ejecutándose 
        /// </summary>
        private bool escenaSimulando = false;

        /// <summary>
        /// Indica si la cámara se está moviendo siguiendo la curva bspline o está fija
        /// </summary>
        private bool camaraFija = true;

        #region Variables asociadas a única fuente de luz de la escena 

        private float[] light_color = new float[4] { 0.5f, 0.5f, 0.6f, 1.0f };
        private float[] light_position = new float[4] { 0.0f, 0.0f, 1.0f, 1.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        #endregion
        #region Variables internas al proceso de generación de la superficie

        private float[] default_curve;
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

        private float[] eye = new float[3] { 15.0f, 15.0f, 5.0f };
        private float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        private float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        #endregion

        #endregion

        // Constructor de la ventana, se ejecuta una sola vez cuando se habre la misma 
        public Form1()
        {
            // Se crea la vista, necesita conocer al modelo
            this.vista = new Vista(escena);

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
            dl_handle = Gl.glGenLists(3);

            Gl.glClearColor(0.02f, 0.02f, 0.04f, 0.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, light_color);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_position);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_LIGHTING);

            // Generación de las Display Lists
            Gl.glNewList(DL_AXIS, Gl.GL_COMPILE);
            DrawAxis();
            Gl.glEndList();
            Gl.glNewList(DL_GRID, Gl.GL_COMPILE);
            DrawXYGrid();
            Gl.glEndList();
            Gl.glNewList(DL_AXIS2D_TOP, Gl.GL_COMPILE);
            DrawAxis2DTopView();
            Gl.glEndList();
            Gl.glNewList(DL_AXIS2D_HEIGHT, Gl.GL_COMPILE);
            DrawAxis2DHeightView();
            Gl.glEndList();
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

        /// <summary>
        /// Reinicia todas las variables globales de la aplicación (incluido el modelo)
        /// </summary>
        private void InicializarEscena()
        {
            this.escena = new Escena();
            this.escenaSimulando = false;
            this.camaraFija = true;
            this.view_axis = true;
            this.view_grid = true;
        }

        #endregion
        #region Eventos

        // Idem a Display
        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            ///////////////////////////////////////////////////
            // Escena 3D
            this.Set3DEnv();

            //
            // Se corresponde se dibujan los componentes de la simulación
            if (this.escenaSimulando)
            {
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                // Se modifica la cámara para que recorra los puntos de la cámara bspline
                if (camaraFija) Glu.gluLookAt(eye[0], eye[1], eye[2], at[0], at[1], at[2], up[0], up[1], up[2]);
                else
                {
                    IList<PuntoFlotante> puntosPosicionCamara = vista.GetPuntosPosicionCamara();
                    Glu.gluLookAt(puntosPosicionCamara[escena.iteradorCurva % puntosPosicionCamara.Count].GetXFlotante(), puntosPosicionCamara[escena.iteradorCurva % puntosPosicionCamara.Count].GetYFlotante(), 10, 0, 0, 0, up[0], up[1], up[2]);
                }

                // Si corresponde se dibujan los ejes
                if (view_axis) Gl.glCallList(DL_AXIS);
                // Se corresponde se dibuja la grilla
                if (view_grid) Gl.glCallList(DL_GRID);

                // Se aplica escalado para pasar de las coordenadas del mundo a las coordenadas de la escena
                vista.EscalarMundoToEscena3D();

                // Se dibuja la rueda
                this.vista.DibujarRueda();

                // Se dibuja la superficie
                DrawSurface();

                Gl.glPopMatrix();
            }
            //
            ///////////////////////////////////////////////////
            // Panel 2D para la vista del camino de la camara.
            Gl.glLoadIdentity();
            this.SetPanelTopEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_TOP);
            //
            this.vista.DibujarCamaraPath();
            //
            ///////////////////////////////////////////////////
            // Panel 2D para la vista del perfil del terreno.
            this.SetPanelHeightEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_HEIGHT);
            //
            this.vista.DibujarTerreno2D();
            //
            ///////////////////////////////////////////////////   
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
                    if (!camaraFija)
                    {
                        escena.AumentarVelocidad();
                        glControl.Refresh();
                    }
                    break;
                case 'l':
                    if (!camaraFija)
                    {
                        escena.ReducirVelocidad();
                        glControl.Refresh();
                    }
                    break;
                case 'r':
                    this.DetenerSimulacion();
                    this.InicializarEscena();
                    glControl.Refresh();
                    break;
                case 's':
                    this.Simular();
                    glControl.Refresh();
                    break;
                case 'o':
                    this.camaraFija = true;
                    this.vista.LimpiarRecorridoCamara();
                    glControl.Refresh();
                    break;
                case 'm':
                    if (this.vista.GetPuntosBspline().Count >= 4)
                    {
                        this.camaraFija = !this.camaraFija;
                        if (!this.camaraFija) this.vista.CrearRecorridoCamara();
                    }
                    glControl.Refresh();
                    break;
                case 'p':
                    this.DetenerSimulacion();
                    this.vista.LimpiarCamino();
                    this.escena.Terreno = new Terreno();
                    glControl.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void DetenerSimulacion()
        {
            this.escenaSimulando = false;
            this.timer.Stop();
        }

        private void Simular()
        {
            this.escenaSimulando = true;

            try
            {
                this.vista.CrearCamino();
                if (!camaraFija) this.vista.CrearRecorridoCamara();
                
                // Se crea el terreno con los puntos de la curva Bzier
                foreach (PuntoFlotante punto in this.vista.GetPuntosCamino())
                {
                    this.escena.Terreno.AddVertice(punto.GetXFlotante(), punto.GetYFlotante());
                }

                this.escena.PosicionarRuedaAlComienzoDelTerreno();
                
                // Se pasan al formato que pide el fwk
                default_curve = this.vista.ConvertirPuntos((IList)vista.GetPuntosCamino());

                // Construccion de la Superficie
                BuildSurface(default_curve, vista.GetPuntosCamino().Count);

                this.timer.Start();
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }
        }

        private void glControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsZona2(e.X, e.Y))
            {
                Vertice nuevoVertice = ConvertirVerticeAZona2(new Vertice(e.X, e.Y));

                this.vista.AddPuntosBzier(nuevoVertice.X, nuevoVertice.Y);
                glControl.Invalidate();
            }
            else if (IsZona3(e.X, e.Y))
            {
                Vertice nuevoVertice = ConvertirVerticeAZona3(new Vertice(e.X, e.Y));
                this.vista.AddPuntosBspline(nuevoVertice.X, nuevoVertice.Y);
                glControl.Invalidate();
            }

            glControl.Refresh();
        }

        /// <summary>
        /// Evento que se llama cada <code>DELTA_TIEMPO</code>. Dentro de este
        /// evento se simula el paso de DELTA_TIEMPO en el modelo físico.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            escena.Simular(DELTA_TIEMPO);
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

        private void DrawAxis2DTopView()
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glColor3d(1, 0.6, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(1, 0, 0);
            Gl.glVertex3d(1, 1, 0);
            Gl.glVertex3d(0, 1, 0);
            Gl.glEnd();
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void DrawAxis2DHeightView()
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glColor3d(1.0, 0.5, 1.0);
            Gl.glVertex3d(0.0, 0.0, 0.0);
            Gl.glVertex3d(1.0, 0.0, 0.0);
            Gl.glVertex3d(1.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, 1.0, 0.0);
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

        /// <summary>
        /// Panel 2D para la vista de la curva correspondiente a la trayectoria
        /// de la camara.
        /// </summary>
        private void SetPanelTopEnv()
        {
            Gl.glViewport(TOP_VIEW_POSX, TOP_VIEW_POSY, TOP_VIEW_W, TOP_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.001, 1.001, -0.001, 1.001);
        }

        /// <summary>
        /// Panel 2D para la vista de la curva correspondiente al terreno.
        /// </summary>
        private void SetPanelHeightEnv()
        {
            Gl.glViewport(HEIGHT_VIEW_POSX, HEIGHT_VIEW_POSY, HEIGHT_VIEW_W, HEIGHT_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.001, 1.001, -0.001, 1.001);
        }

        #endregion
        #region Métodos Auxiliares

        private bool IsZona2(int x, int y)
        {
            return (HEIGHT_VIEW_POSX <= x && x <= (HEIGHT_VIEW_POSX + HEIGHT_VIEW_W)) &&
                (0 <= y && y <= HEIGHT_VIEW_H);
        }

        private Vertice ConvertirVerticeAZona2(Vertice vertice)
        {
            double newX = (vertice.X - HEIGHT_VIEW_POSX) / HEIGHT_VIEW_W;
            double newY = ((W_HEIGHT - vertice.Y) - HEIGHT_VIEW_POSY) / HEIGHT_VIEW_H;

            System.Console.Out.WriteLine("offsetX: " + newX + " offsetY: " + newY);
            System.Console.Out.WriteLine("W_HEIGHT: " + W_HEIGHT + " HEIGHT_VIEW_POSY: " + HEIGHT_VIEW_POSY + " HEIGHT_VIEW_H:" + HEIGHT_VIEW_H);
            System.Console.Out.WriteLine("W_HEIGHT- vertice.Y: " + (W_HEIGHT - vertice.Y));

            return new Vertice(newX, newY);
        }

        private bool IsZona3(int x, int y)
        {
            return (TOP_VIEW_POSX <= x && x <= (TOP_VIEW_POSX + TOP_VIEW_W)) &&
                (0 <= y && y <= TOP_VIEW_H);
        }

        private Vertice ConvertirVerticeAZona3(Vertice vertice)
        {
            double newX = (vertice.X - TOP_VIEW_POSX) / TOP_VIEW_W;
            double newY = ((W_HEIGHT - vertice.Y) - TOP_VIEW_POSY) / TOP_VIEW_H;

            return new Vertice(newX, newY);
        }

        #endregion

    }
}
