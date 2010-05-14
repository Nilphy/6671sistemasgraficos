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

        private const int DELTA_TIEMPO = 50;
        
        public Escena escena = new Escena();
        public Vista vista = new Vista();
        private Timer timer;

        #region Propiedades

        private int dl_handle;

        private bool view_grid = true;
        private bool view_axis = true;

        // Variables que controlan la ubicación de la cámara en la Escena 3D
        private float[] eye = new float[3] { 15.0f, 15.0f, 5.0f };
        private float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        private float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        // Variables asociadas a única fuente de luz de la escena
        private float[] light_color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] light_position = new float[4] { 0.0f, 0.0f, 1.0f, 1.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        // Color de la esfera en movimiento dentro de la escena
        private float[] color_esfera = new float[4] { 0.5f, 0.5f, 0.2f, 1.0f };

        private float[] default_curve = new float[Vista.CURVE_POINTS * 2];

        private float[] surface_buffer = null;
        private float[] normals_buffer = null;

        private int curve_points = 0;
                
        private int DL_AXIS
        {
            get { return (dl_handle + 0); }
        }
        private int DL_GRID
        {
            get { return (dl_handle + 1); }
        }
        private int DL_AXIS2D_TOP
        {
            get { return (dl_handle + 2); }
        }
        private int DL_AXIS2D_HEIGHT
        {
            get { return (dl_handle + 3); }
        }



        #endregion

        public Form1()
        {
            InitializeComponent();
            glControl.InitializeContexts();
            Init();

            CrearEscenaInicial();

            // Configuramos el Timer para la simulación.
            this.timer = new Timer();
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Interval = DELTA_TIEMPO;
            timer.Start();
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
            normals_buffer[(nr_points-1)*3 + 0] = normals_buffer[(nr_points-2)*3 + 0];
            normals_buffer[(nr_points-1)*3 + 1] = normals_buffer[(nr_points-2)*3 + 1];
            normals_buffer[(nr_points-1)*3 + 2] = normals_buffer[(nr_points-2)*3 + 2];
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            ///////////////////////////////////////////////////
            // Escena 3D
            this.Set3DEnv();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(eye[0], eye[1], eye[2], at[0], at[1], at[2], up[0], up[1], up[2]);

            if (view_axis)
                Gl.glCallList(DL_AXIS);

            if (view_grid)
                Gl.glCallList(DL_GRID);

            // TODO Acá se dibuja el cilindro
            //this.vista.DibujarEscena(this.escena);

            //
            ///////////////////////////////////////////////////

            // Dibujar la superficie generada a partir de la curva
            DrawSurface();
          
            ///////////////////////////////////////////////////
            // Panel 2D para la vista superior
            this.SetPanelTopEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_TOP);

            //
            ///////////////////////////////////////////////////

            // TODO Dibujar acá la curva Bsiel

            ///////////////////////////////////////////////////
            // Panel 2D para la vista del perfil de altura
            this.SetPanelHeightEnv();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);
            Gl.glCallList(DL_AXIS2D_HEIGHT);
            //
            ///////////////////////////////////////////////////   

            // TODO Dibujar acá la curva del perfil
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            escena.Simular(DELTA_TIEMPO);

            glControl.Invalidate();
        }

        private void CrearEscenaInicial()
        {
            this.escena = new Escena();

            // Creo el terreno
            this.escena.Terreno.AddVertice(0, 500);
            this.escena.Terreno.AddVertice(100, 100);
            this.escena.Terreno.AddVertice(400, 250);
            this.escena.Terreno.AddVertice(600, 200);
            this.escena.Terreno.AddVertice(800, 600);

            // Creo la rueda
            this.escena.Rueda.RadioExterno = 10;
            this.escena.Rueda.Centro.X = 20;
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
            Gl.glColor3d(1, 1, 0);
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
            Gl.glViewport(0, 0, Vista.W_WIDTH, Vista.W_HEIGHT);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60.0, (float)Vista.W_WIDTH / (float)Vista.W_HEIGHT, 0.10, 100.0);
        }

        private void SetPanelTopEnv()
        {
            Gl.glViewport(Vista.TOP_VIEW_POSX, Vista.TOP_VIEW_POSY, Vista.TOP_VIEW_W, Vista.TOP_VIEW_H);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-0.10, 1.05, -0.10, 1.05);
        }

        private void SetPanelHeightEnv()
        {
            Gl.glViewport(Vista.HEIGHT_VIEW_POSX, Vista.HEIGHT_VIEW_POSY, Vista.HEIGHT_VIEW_W, Vista.HEIGHT_VIEW_H);
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
                //Gl.glScaled(10.0, 20.0, 1.0);
                //Gl.glTranslated(0.0, -0.5, 0.0);
                //Gl.glScaled(1.0, 1 / (double)curve_points, 1.0);
                vista.EscalarMundoToEscena3D();
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
            Gl.glViewport(0, 0, Vista.W_WIDTH, Vista.W_WIDTH / 2);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-10, 10, -5.0, 5.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0.5, 0, 0, 0, 0, 1, 0);

        }

        private void SetCamera()
        {
            Gl.glViewport(Vista.TOP_VIEW_POSX, Vista.TOP_VIEW_POSY, Vista.TOP_VIEW_W, Vista.TOP_VIEW_H);
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
            // Se obtienen los puntos seleccionados por el usuario
            IList<PuntoFlotante> puntosPoligonoControlBzier = this.vista.GetPuntosBzier();

            // Se escalan los puntos a las coordenadas de mundo, para poder controlar el paso de discretisación
            this.vista.EscalarPuntosBzier(puntosPoligonoControlBzier);

            // Se crea la curva
            CurvaBzierSegmentosCubicos curva = new CurvaBzierSegmentosCubicos(puntosPoligonoControlBzier);
            
            // Se obtienen los puntos discretos de la curva
            IList puntosBzier = (IList)curva.GetPuntosDiscretos(0.1);
            
            // Se pasan al formato que pide el fwk
            default_curve = this.vista.ConvertirPuntos(puntosBzier);

            // Construccion de la Superficie
            BuildSurface(default_curve, puntosBzier.Count);

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
                case 'r':
                    escena = new Escena();
                    vista = new Vista();
                    this.CrearEscenaInicial();
                    glControl.Refresh();
                    break;
                default:
                    break;
            }
        }

    }
}
