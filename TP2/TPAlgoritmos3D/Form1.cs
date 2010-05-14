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
    public partial class Form1 : Form, IWindowParameterProvider
    {

        private const int DELTA_TIEMPO = 50;
        
        public Escena escena = new Escena();
        public Vista vista;
        private Timer timer;
        

        #region Propiedades

        private int dl_handle;

        //private int W_WIDTH = 1024;
        //private int W_HEIGHT = 768;

        private bool view_grid = true;
        private bool view_axis = true;

        // Variables que controlan la ubicación de la cámara en la Escena 3D
        //private float[] eye = new float[3] { 15.0f, 15.0f, 5.0f };
        //private float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        private float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        // Variables asociadas a única fuente de luz de la escena
        private float[] light_color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] light_position = new float[4] { 0.0f, 0.0f, 1.0f, 1.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        // Color de la esfera en movimiento dentro de la escena
        private float[] color_esfera = new float[4] { 0.5f, 0.5f, 0.2f, 1.0f };

        private float[] default_curve;

        private float[] surface_buffer = null;
        private float[] normals_buffer = null;

        private int curve_points = 0;

        private bool simularEscenea = false;

        public int TOP_VIEW_POSX
        {
            get { return (int)((float)W_WIDTH * 0.70f); }
            set { this.TOP_VIEW_POSX = value; }
        }

        public int TOP_VIEW_POSY
        {
            get { return (int)((float)W_HEIGHT * 0.70f); }
            set { this.TOP_VIEW_POSY = value; }
        }

        public int TOP_VIEW_W
        {
            get { return (int)((float)W_WIDTH * 0.30f); }
            set { this.TOP_VIEW_W = value; }
        }

        public int TOP_VIEW_H
        {
            get { return (int)((float)W_HEIGHT * 0.30f); }
            set { this.TOP_VIEW_H = value; }
        }

        public int HEIGHT_VIEW_POSX
        {
            get { return (int)((float)W_WIDTH * 0.00f); }
            set { this.HEIGHT_VIEW_POSX = value; }
        }

        public int HEIGHT_VIEW_POSY
        {
            get { return (int)((float)W_HEIGHT * 0.70f); }
            set { this.HEIGHT_VIEW_POSY = value; }
        }

        public int HEIGHT_VIEW_W
        {
            get { return (int)((float)W_WIDTH * 0.30f); }
            set { this.HEIGHT_VIEW_W = value; }
        }

        public int HEIGHT_VIEW_H
        {
            get { return (int)((float)W_HEIGHT * 0.30f); }
            set { this.HEIGHT_VIEW_H = value; }
        }

        public int DL_AXIS
        {
            get { return (dl_handle + 0); }
            set { this.DL_AXIS = value; }
        }
        public int DL_GRID
        {
            get { return (dl_handle + 1); }
            set { this.DL_GRID = value; }
        }
        public int DL_AXIS2D_TOP
        {
            get { return (dl_handle + 2); }
            set { this.DL_AXIS2D_TOP = value; }
        }
        public int DL_AXIS2D_HEIGHT
        {
            get { return (dl_handle + 3); }
            set { this.DL_AXIS2D_HEIGHT = value; }
        }

        public int W_WIDTH
        {
            get { return this.Width; }
            set { this.W_WIDTH = value; }
        }

        public int W_HEIGHT
        {
            get { return this.Height; }
            set { this.W_HEIGHT = value; }
        }

        #endregion

        public Form1()
        {
            this.vista = new Vista(this, escena);
            InitializeComponent();
            glControl.InitializeContexts();
            Init();

            //CrearEscena();

            // Configuramos el Timer para la simulación.
            this.timer = new Timer();
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Interval = DELTA_TIEMPO;
            
            //timer.Start();
        }

        private void BuildSurface(float[] vertex_buffers, int nr_points)
        {
            float dx,dy,dz;
            curve_points = nr_points;

            if (surface_buffer != null)
	            surface_buffer = null;
            surface_buffer = new float[nr_points * 6];
            if (normals_buffer != null)
	            normals_buffer = null;
            normals_buffer = new float[nr_points * 3];

            for (int i=0; i < nr_points; i++)
            {
	            surface_buffer[i*3 + 0] = 1.0f;
	            surface_buffer[i*3 + 1] = vertex_buffers[i*2 + 0];
	            surface_buffer[i*3 + 2] = vertex_buffers[i*2 + 1];

	            surface_buffer[nr_points*3 + i*3 + 0] = -1.0f;
	            surface_buffer[nr_points*3 + i*3 + 1] = vertex_buffers[i*2 + 0];
	            surface_buffer[nr_points*3 + i*3 + 2] = vertex_buffers[i*2 + 1];
            }

            for (int i=0; i < (nr_points-1); i++)
            {
	            dx = 0.0f;
	            dy = vertex_buffers[(i+1)*2 + 0] - vertex_buffers[i*2 + 0];
	            dz = vertex_buffers[(i+1)*2 + 1] - vertex_buffers[i*2 + 1];
	            normals_buffer[i*3 + 0] = dx;
	            normals_buffer[i*3 + 1] = -dz;
	            normals_buffer[i*3 + 2] = dy;
            }

            normals_buffer[(nr_points - 1) * 3 + 0] = normals_buffer[(nr_points - 2) * 3 + 0];
            normals_buffer[(nr_points - 1) * 3 + 1] = normals_buffer[(nr_points - 2) * 3 + 1];
            normals_buffer[(nr_points - 1) * 3 + 2] = normals_buffer[(nr_points - 2) * 3 + 2];
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            ///////////////////////////////////////////////////
            // Escena 3D
            this.Set3DEnv();

            if (this.simularEscenea)
            {
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glLoadIdentity();

                IList<PuntoFlotante> puntosCamara = vista.GetPuntosBspline();

                // Se escalan los puntos a las coordenadas máximas de la cámara
                this.vista.EscalarPuntosVentanitas(puntosCamara, false);

                // Se crea la curva
                CurvaBsplineSegmentosCubicos curva = new CurvaBsplineSegmentosCubicos(puntosCamara);

                // Se obtienen los puntos discretos de la curva
                IList<PuntoFlotante> puntosBspline = curva.GetPuntosDiscretos(0.001);

                Glu.gluLookAt(puntosBspline[escena.iteradorCurva % puntosBspline.Count].GetXFlotante(), puntosBspline[escena.iteradorCurva % puntosBspline.Count].GetYFlotante(), 10, 0, 0, 0, up[0], up[1], up[2]);

                if (view_axis)
                    Gl.glCallList(DL_AXIS);

                if (view_grid)
                    Gl.glCallList(DL_GRID);

                vista.EscalarMundoToEscena3D();

                this.vista.DibujarRueda();

                // Dibujar la superficie generada a partir de la curva
                DrawSurface();

                Gl.glPopMatrix();
            }

            ///////////////////////////////////////////////////
            // Panel 2D para la vista superior derecha
            this.SetPanelTopEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_TOP);

            //
            ///////////////////////////////////////////////////

            // TODO: pasarme en limpio y a VISTA!!!
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glLineStipple(4, 0xAAAA);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glPushMatrix();
            foreach (PuntoFlotante punto in this.vista.GetPuntosBspline())
            {
                Gl.glVertex2d(punto.X, punto.Y);
            }
            Gl.glPopMatrix();
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
            Gl.glEnable(Gl.GL_LIGHTING);

            foreach (PuntoFlotante punto in this.vista.GetPuntosBspline())
            {
                DibujarPunto(punto.X, punto.Y);
            }

            ///////////////////////////////////////////////////
            // Panel 2D para la vista del perfil de altura
            this.SetPanelHeightEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_HEIGHT);
            //
            ///////////////////////////////////////////////////   

            // TODO: pasarme en limpio y a VISTA!!!
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glLineStipple(4, 0xAAAA);
            Gl.glColor3d(0, 0, 1);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glPushMatrix();
            foreach (PuntoFlotante punto in this.vista.GetPuntosBzier())
            {
                Gl.glVertex2d(punto.X, punto.Y);
            }
            Gl.glPopMatrix();
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
            Gl.glEnable(Gl.GL_LIGHTING);

            foreach (PuntoFlotante punto in this.vista.GetPuntosBzier())
            {
                DibujarPunto(punto.X, punto.Y);
            }
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            escena.Simular(DELTA_TIEMPO);

            glControl.Invalidate();
        }

        private void CrearEscena()
        {
            this.escena = new Escena();

            // Creo el terreno
            foreach (PuntoFlotante punto in vista.GetPuntosCurvaBzier())
            {
                this.escena.Terreno.AddVertice(punto.GetXFlotante(), punto.GetYFlotante());
            }

            // Creo la rueda            
            this.escena.Rueda.Centro.X = this.escena.Terreno.Vertices[10].X;
            double anguloTerreno = this.escena.Terreno.GetAnguloInclinacion(this.escena.Rueda.Centro.X);
            double dx = this.escena.Rueda.RadioExterno * Math.Sin(anguloTerreno);
            double nuevoX = this.escena.Rueda.Centro.X + dx;
            this.escena.Rueda.Centro.Y = this.escena.Terreno.GetAltura(nuevoX) + this.escena.Rueda.RadioExterno * Math.Cos(anguloTerreno);
        }

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

        private void Set3DEnv()
        {
            Gl.glViewport(0, 0, W_WIDTH, W_HEIGHT);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60.0, (float)W_WIDTH / (float)W_HEIGHT, 0.10, 100.0);
        }

        /// <summary>
        /// Panel 2D para la vista superior derecha
        /// </summary>
        private void SetPanelTopEnv()
        {
            Gl.glViewport(TOP_VIEW_POSX, TOP_VIEW_POSY, TOP_VIEW_W, TOP_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.10, 1.05, -0.10, 1.05);
        }

        /// <summary>
        /// Panel 2D para la vista superior izquierda
        /// </summary>
        private void SetPanelHeightEnv()
        {
            Gl.glViewport(HEIGHT_VIEW_POSX, HEIGHT_VIEW_POSY, HEIGHT_VIEW_W, HEIGHT_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.10, 1.05, -0.10, 1.05);
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

        private void Init()
        {
            if (this.simularEscenea)
            {
                IList puntosBzier = (IList)vista.GetPuntosCurvaBzier();

                // Se pasan al formato que pide el fwk
                default_curve = this.vista.ConvertirPuntos(puntosBzier);

                // Construccion de la Superficie
                BuildSurface(default_curve, puntosBzier.Count);
            }

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

        private void glControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char) 27:
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
                    escena.velocidadIteracionCurva*=2;
                    glControl.Refresh();
                    break;
                case 'l':
                    escena.velocidadIteracionCurva/=2;
                    glControl.Refresh();
                    break;
                case 'r':
                    escena = new Escena();
                    vista = new Vista(this, escena);
                    this.CrearEscena();
                    glControl.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void glControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsZona2(e.X, e.Y))
            {
                double offsetX = e.X - HEIGHT_VIEW_POSX;
                double offsetY = e.Y + 40; // TODO: VER COMO CUERNO HACER PARA ARREGLAR EL CORRIMIENTO

                offsetX = (offsetX / (HEIGHT_VIEW_POSX + HEIGHT_VIEW_W));
                offsetY = ((HEIGHT_VIEW_H - offsetY) / HEIGHT_VIEW_H);

                this.vista.AddPuntosBzier(offsetX, offsetY);
                glControl.Invalidate();
            }
            else if (IsZona3(e.X, e.Y))
            {
                double offsetX = e.X - TOP_VIEW_POSX;
                double offsetY = e.Y + 40; // TODO: VER COMO CUERNO HACER PARA ARREGLAR EL CORRIMIENTO

                offsetX = (offsetX / TOP_VIEW_W);
                offsetY = ((TOP_VIEW_H - offsetY) / TOP_VIEW_H);

                this.vista.AddPuntosBspline(offsetX, offsetY);
                glControl.Invalidate();
            }
        }

        private bool IsZona2(int x, int y)
        {
            return (HEIGHT_VIEW_POSX <= x && x <= (HEIGHT_VIEW_POSX + HEIGHT_VIEW_W)) &&
                (0 <= y && y <= HEIGHT_VIEW_H);
        }

        private bool IsZona3(int x, int y)
        {
            return (TOP_VIEW_POSX <= x && x <= (TOP_VIEW_POSX + TOP_VIEW_W)) &&
                (0 <= y && y <= TOP_VIEW_H);
        }

        // TODO: pasarme a VISTA!
        private void DibujarPunto(double x, double y)
        {
            double DELTA = 0.01;
            Gl.glPushMatrix();

            Gl.glDisable(Gl.GL_LIGHTING);
            Glu.GLUquadric quad = Glu.gluNewQuadric();
            Gl.glPushMatrix();
            Gl.glColor3d(1, 0, 0);
            Gl.glTranslated(x, y, 0);
            Glu.gluDisk(quad, 0, DELTA, 20, 20);
            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glColor3d(1, 1, 1);
            Glu.gluDeleteQuadric(quad);
        }

    }
}
