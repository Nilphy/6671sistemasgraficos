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

namespace TPAlgoritmos2D
{
    public partial class Form1 : Form
    {

        private const int DELTA_TIEMPO = 50;
        
        public Escena escena = new Escena();
        public Vista vista = new Vista();
        private Timer timer;

        #region Propiedades

        private int dl_handle;

        private int W_WIDTH = 1024;
        private int W_HEIGHT = 768;

        private bool view_grid = true;
        private bool view_axis = true;

        // Variables que controlan la ubicación de la cámara en la Escena 3D
        private float[] eye = new float[3] { 15.0f, 15.0f, 5.0f };
        private float[] at = new float[3] { 0.0f, 0.0f, 0.0f };
        private float[] up = new float[3] { 0.0f, 0.0f, 1.0f };

        // Variables asociadas a única fuente de luz de la escena
        private float[] light_color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] light_position = new float[3] { 10.0f, 10.0f, 8.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        private int TOP_VIEW_POSX
        {
            get { return ((int)((float)W_WIDTH * 0.70f)); }
        }

        private int TOP_VIEW_POSY
        {
            get { return ((int)((float)W_HEIGHT * 0.70f)); }
        }

        private int TOP_VIEW_W
        {
            get { return ((int)((float)W_WIDTH * 0.30f)); }
        }

        private int TOP_VIEW_H
        {
            get { return ((int)((float)W_HEIGHT * 0.30f)); }
        }

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

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            ///////////////////////////////////////////////////
            // Escena 3D
            this.SetSceneWindow();

            this.vista.DibujarEscena(this.escena);

            if (view_axis)
                Gl.glCallList(DL_AXIS);

            if (view_grid)
                Gl.glCallList(DL_GRID);

            
            //this.DibujarPoligonos(false);
          
            ///////////////////////////////////////////////////
            // Panel 2D para la vista de la camara
            SetCamera();
            
            // Acá se dibuja solo la parte de la pelota... etonces tomamos centro pelota como centro de la ventana
            // y dos radios para todos lados y movemos eso por matriz de transformación a la cámara... 

            // Matriz de transformación!!!
            // 
            // poner en 0,0 -> (centox - 2radiox, centroy - 2 radio y)
            // escalar para que entre en 0 1 0 1
            
            // clipping
            //this.DibujarPoligonos(true);
            // Se obtienen todos los polígonos que forman parte del dibujo
            // (dibujarlos teniendo en cuenta que tienen que estar entre -10 10 -5 5)
            // Se dibujan, se pintan y listo 
            // La posición de la rueda tiene que depender de una variable global


            //
            ///////////////////////////////////////////////////

            // Se setea el temporizador para actualizar los datos del modelo 30 frames por segundo
            // osea cada 6000/30 milisegundos = 200 milisegundos
            //Glut.glutTimerFunc(200, actualizarModelo, 0);
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
            this.escena.Rueda.RadioExterno = 5;

            this.escena.Rueda.Centro.X = 20;
            this.escena.Rueda.Centro.Y = escena.Terreno.GetAltura(20) + this.escena.Rueda.RadioExterno;
        }

        /// <summary>
        /// convierte en polígonos todos los cuerpos del modelo
        /// si aplicar clipping es true aplica clipping a todos los polígonos
        /// dibuja todos los polígonos resultantes y los pinta del color que corresponda
        /// </summary>
        /// <param name="aplicarClipping"></param>
        private void DibujarPoligonos(bool aplicarClipping)
        {
            //if (vista.PoligonosTerreno.Count == 0) this.GenerarPoligonosTerreno();
            // this.GenerarPoligonosRueda(); // siempre se generan porque cambian

            if (aplicarClipping) this.AplicarClipping();

            foreach (Poligono poligono in vista.PoligonosTerreno)
            {
                Gl.glPushMatrix();

                // Se rellena el polígono
                Pintar.RellenarPoligonoScanLine(poligono.Puntos, poligono.ColorRelleno);
                
                Gl.glColor3f(poligono.ColorLinea.Red, poligono.ColorLinea.Green, poligono.ColorLinea.Blue);

                // Todos los puntos van a ser unidos por segmentos y el último se une al primero
                Gl.glBegin(Gl.GL_LINE_LOOP);

                foreach (Punto punto in poligono.Puntos) 
                {
                    Gl.glVertex2d(punto.GetXFlotante(), punto.GetYFlotante());
                }
                
                Gl.glEnd();

                Gl.glPopMatrix();
            }

            // Estos se van a dibujar -> mover al origen -> rotar -> mover adonde estaban
            /*
            foreach (Poligono poligono in vista.PoligonosRueda)
            {
                Gl.glPushMatrix();

                Gl.glTranslatef(escena.Rueda.Centro.X, escena.Rueda.Centro.Y, 0);
                Gl.glRotatef(escena.Rueda.Omega, 0, 0, 1);
                Gl.glTranslatef(-escena.Rueda.Centro.X, -escena.Rueda.Centro.Y, 0);

                Gl.glColor3f(poligono.ColorLinea.Red, poligono.ColorLinea.Green, poligono.ColorLinea.Blue);

                // Todos los puntos van a ser unidos por segmentos y el último se une al primero
                Gl.glBegin(Gl.GL_LINE_LOOP);

                foreach (Punto punto in poligono.Puntos)
                {
                    Gl.glVertex2f(punto.GetXFlotante(), punto.GetYFlotante());
                }

                Gl.glEnd();

                // Se rellena el polígono
                Pintar.RellenarPoligonoScanLine(poligono.Puntos, poligono.ColorRelleno);

                Gl.glPopMatrix();
            }*/
        }

        private void AplicarClipping()
        {
            foreach (Poligono poligono in vista.PoligonosTerreno)
            {
                poligono.Puntos = Clipping.RecortarPoligono(poligono.Puntos, new ViewPort(escena.Rueda));
            }

            foreach (Poligono poligono in vista.PoligonosRueda)
            {
                poligono.Puntos = Clipping.RecortarPoligono(poligono.Puntos, new ViewPort(escena.Rueda));
            }
        }

        private void DrawAxis()
        {
            Gl.glDisable(Gl.GL_LIGHTING);

            Gl.glBegin(Gl.GL_LINE_STRIP);
            // X
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(vista.X_MIN, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(vista.X_MAX, 0, 0);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            // Y
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, vista.Y_MIN, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(0, vista.Y_MAX, 0);
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
                default:
                    break;
            }
        }

    }
}
