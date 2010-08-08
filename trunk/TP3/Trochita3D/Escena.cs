using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Trochita3D.Entidades;
using Tao.OpenGl;
using System.Drawing;
using System.Drawing.Imaging;
using Common.Utils;
using Trochita3D.Curvas;

namespace Trochita3D
{
    public class Escena
    {
        public Camara Camara { get; set; }

        private const double DELTA_U = 0.05;
        private const double DELTA_U2 = 0.0005;
        private const double ALTURA_TERRAPLEN = 2.2;
        private const double DIST_RIELES = 0.8;
        private const int DIST_TABLA = 2; // Distancia entre tablas de la vía.

        #region Constantes del tren

        
        private static float[] TREN_LUZ_BRILLO = new float[] { 0.2f, 0.2f, 0.2f, 1 };
        private static int TREN_SHININESS = 180;

        private static double VELOCIDAD_TREN = 1000;
        private static double RADIO_RUEDA_TREN = 1.7d / 5d; 
        private static double VELOCIDAD_ANGULAR_RUEDAS = VELOCIDAD_TREN / RADIO_RUEDA_TREN;
        private static double tiempo = 0;


        #endregion

        #region Variables asociadas a las fuentes de luz de la escena

        private float[] day_light_color = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] day_light_ambient = new float[4] { 0.15f, 0.15f, 0.15f, 1.0f };
        private float[] night_light_color = new float[4] { 64f / 255f, 156f / 255f, 1.0f, 1.0f };
        private float[] night_light_ambient = new float[4] { 0.05f, 0.05f, 0.15f, 1.0f };
        private float[] light_position = new float[4] { 7.0f, 7.0f, 100.0f, 0.0f };

        private float[] secondary_day_light_color = new float[4] { 0.20f, 0.20f, 0.20f, 1.0f };
        private float[] secondary_day_light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };
        private float[] secondary_night_light_color = new float[4] { 0.20f, 0.20f, 0.20f, 1.0f };
        private float[] secondary_night_light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        private float[] light_linterna_position = new float[4] { 10.0f, 10.0f, 10.0f, 1.0f };
        private float[] light_linterna_direction = new float[3] { 1.0f, 1.0f, 1.0f };

        #endregion

        private IList<Punto> path;
        private IList<Punto> detailPath;

        private Terraplen terraplen;
        private Riel riel1;
        private Riel riel2;
        private Tabla tabla;

        // Partes de la escena
        //private SurfaceInitializer surfaceInitializer;
        private TerrainInitializer terrainInitializer;
        private WaterInitializer waterInitializer;
        private Skybox skybox;
        private Train Tren = new Train(TREN_LUZ_BRILLO, TREN_SHININESS);
        private bool daylight = true;

        Punto[] posicionArboles = new Punto[] {
            new Punto(20, 5, 0),
            new Punto(38, 0, 0),
            new Punto(30, 6, 0),
            new Punto(38, 12, 0),
            new Punto(24, 20, 0),
            new Punto(24, -15, 0),
            new Punto(25, 30, 0),
            new Punto(-20, -5, 0),
            new Punto(-38, 0, 0),
            new Punto(-30, -6, 0),
            new Punto(-38, -12, 0),
            new Punto(-24, -20, 0),
            new Punto(-24, 30, 0),
            new Punto(-40, 35, 0),
            new Punto(-35, 25, 0),
            new Punto(-25, -35, 0),
            new Punto(-35, -35, 0),
            new Punto(-15, -40, 0)
        };
        Arbol[] arboles = null;

        public void Inicializar(int width, int height)
        {
            // Crea los puntos del camino a seguir por el terraplen y la vía.
            this.path = GetBsplineControlPoints(DELTA_U);

            // Crea los puntos del camino a seguir por el terraplen y la vía 
            // con mayor definición de puntos. 
            this.detailPath = GetBsplineControlPoints(DELTA_U2);

            this.InicializarLuces();

            // Se crean las estructuras de la escena.
            this.skybox = new Skybox(width);
            this.skybox.CargarTexturas(daylight);

            this.terraplen = new Terraplen(ALTURA_TERRAPLEN);
            this.terraplen.SetCamino(path);

            this.riel1 = new Riel();
            this.riel1.Escalar(1, 0.2, 0.2);
            this.riel1.Trasladar(0, DIST_RIELES / 2d, ALTURA_TERRAPLEN);
            this.riel1.SetCamino(path);

            this.riel2 = new Riel();
            this.riel2.Escalar(1, 0.2, 0.2);
            this.riel2.Trasladar(0, -DIST_RIELES / 2d, ALTURA_TERRAPLEN);
            this.riel2.SetCamino(path);

            this.tabla = new Tabla(DIST_TABLA, ALTURA_TERRAPLEN);
            this.tabla.SetCamino(detailPath);

            //this.surfaceInitializer = new SurfaceInitializer();
            //this.surfaceInitializer.BuildSurface();

            this.terrainInitializer = new TerrainInitializer();
            this.waterInitializer = new WaterInitializer();
            Tren.Posicion = this.terraplen.GetPositionByDistancia(0);
            arboles = Arbol.GenerarArbolesAleatorios(posicionArboles.Count());
        }

        /// <summary>
        /// Obtiene los puntos discretos del trayecto a realizar por el terraplen
        /// a partir de los puntos de control definidos en esta misma funcion.
        /// </summary>
        /// <param name="du">Delta U</param>
        /// <returns>
        /// Lista de vertices que corresponden a la curva que representa el trayecto.
        /// </returns>
        private IList<Punto> GetBsplineControlPoints(double du)
        {
            IList<Punto> ptsControl = new List<Punto>();

            ptsControl.Add(new Punto(55, 80, 0));
            ptsControl.Add(new Punto(72, 60, 0));
            ptsControl.Add(new Punto(76, 35, 0));
            ptsControl.Add(new Punto(46, 11, 0));
            ptsControl.Add(new Punto(46, -15, 0));
            ptsControl.Add(new Punto(60, -30, 0));
            ptsControl.Add(new Punto(75, -35, 0));
            ptsControl.Add(new Punto(83, -50, 0));
            ptsControl.Add(new Punto(83, -65, 0));
            ptsControl.Add(new Punto(77, -79, 0));
            ptsControl.Add(new Punto(43, -71, 0));
            ptsControl.Add(new Punto(21, -63, 0));
            ptsControl.Add(new Punto(1, -76, 0));
            ptsControl.Add(new Punto(-8, -74, 0));
            ptsControl.Add(new Punto(-25, -69, 0));
            ptsControl.Add(new Punto(-52, -64, 0));
            ptsControl.Add(new Punto(-63, -45, 0));
            ptsControl.Add(new Punto(-57, -14, 0));
            ptsControl.Add(new Punto(-40, 10, 0));
            ptsControl.Add(new Punto(-45, 30, 0));
            ptsControl.Add(new Punto(-67, 47, 0));
            ptsControl.Add(new Punto(-57, 70, 0));
            ptsControl.Add(new Punto(-23, 80, 0));

            CurvaBsplineSegmentosCubicos path = new CurvaBsplineSegmentosCubicos(ptsControl);

            return path.GetPuntosDiscretos(du);
        }

        /// <summary>
        /// Inicialización de la iluminación de la escena.
        /// </summary>
        private void InicializarLuces()
        {
            Gl.glPushMatrix();
            float[] light_color;
            float[] light_ambient;
            float[] secondary_light_color;
            float[] secondary_light_ambient;

            light_color = (daylight) ? day_light_color : night_light_color;
            light_ambient = (daylight) ? day_light_ambient : night_light_ambient;
            secondary_light_color = (daylight) ? secondary_day_light_color : secondary_night_light_color;
            secondary_light_ambient = (daylight) ? secondary_day_light_ambient : secondary_night_light_ambient;
            
            // Fuente de luz principal
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[4] {0f, 0f, 0f, 1f});
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, light_color);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_position);
         
              

            // Fuentes de luz secundarias
            
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, new float[4] {0f, 0f, 0f, 1f});
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, new float[4] {0f, 0f, 0f, 1f});
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[4] { -100.0f, -100.0f, 50.0f, 1.0f });
            Gl.glEnable(Gl.GL_LIGHT1);
            
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[4] { -100.0f, 100.0f, 1.0f, 0.0f });
            Gl.glLightf(Gl.GL_LIGHT2, Gl.GL_SPOT_CUTOFF, 180.0f);
            Gl.glEnable(Gl.GL_LIGHT2);

            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[4] { 100.0f, -100.0f, 1.0f, 0.0f });
            Gl.glLightf(Gl.GL_LIGHT3, Gl.GL_SPOT_CUTOFF, 180.0f);
            Gl.glEnable(Gl.GL_LIGHT3);

            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_POSITION, new float[4] { 100.0f, 100.0f, 1.0f, 0.0f });
            Gl.glLightf(Gl.GL_LIGHT4, Gl.GL_SPOT_CUTOFF, 180.0f);
            Gl.glEnable(Gl.GL_LIGHT4);
            
            /* no se para que era esto... avisen
            Gl.glLightf(Gl.GL_LIGHT6, Gl.GL_SPOT_CUTOFF, 10.0f);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_DIFFUSE, day_light_color);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_AMBIENT, day_light_ambient);
            //Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPOT_DIRECTION, this.light_linterna_direction);
            //Gl.glLighti(Gl.GL_LIGHT6, Gl.GL_QUADRATIC_ATTENUATION, 1);
             **/

            if (daylight)
            {
                Gl.glDisable(Gl.GL_LIGHT6);
                Gl.glEnable(Gl.GL_LIGHT0);
            }
            else
            {
                Gl.glEnable(Gl.GL_LIGHT6);
                Gl.glDisable(Gl.GL_LIGHT0);
            }

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glPopMatrix();
        }

        public void DibujarSkybox()
        {

            skybox.Dibujar();
        }

        public void Dibujar()
        {
            this.InicializarLuces();
            //this.surfaceInitializer.DrawSurface();
            this.terraplen.Dibujar();
            this.riel1.Dibujar();
            this.riel2.Dibujar();
            this.tabla.Dibujar();

            terrainInitializer.DrawTerrain();
            waterInitializer.DrawPlaneOfWater();

            if (Camara is CamaraLocomotora)
                Tren.Draw(!this.daylight, (CamaraLocomotora)Camara);
            else
                Tren.Draw(!this.daylight, null);

            for (int i = 0; i < arboles.Count(); ++i)
            {
                Gl.glPushMatrix();
                Gl.glScaled(2, 2, 2);
                Punto posicion = posicionArboles[i];
                Gl.glTranslated(posicion.X, posicion.Y, posicion.Z);
                arboles[i].Dibujar();
                Gl.glPopMatrix();
            }
        }

        public void Simular(double deltaTiempo)
        {
            // Falta actualizar el ángulo de rotación de la rueda.
            tiempo += deltaTiempo;
            double distancia = VELOCIDAD_TREN * tiempo;

            Punto posicion = this.terraplen.GetPositionByDistancia(distancia);
            double inclinacion = this.terraplen.GetInclinacionByDistancia(distancia);

            Tren.Posicion = posicion;
            Tren.InclinaciónLocomotora = inclinacion;
            Tren.AnguloRotacionRuedas -= (VELOCIDAD_ANGULAR_RUEDAS / 1000) * tiempo;
            Tren.AnguloRotacionRuedas %= (2 * Math.PI);
        }

        public void SwitchDayLight()
        {
            this.daylight = !this.daylight;
            this.skybox.CargarTexturas(daylight);
            this.InicializarLuces();
        }
    }
}
