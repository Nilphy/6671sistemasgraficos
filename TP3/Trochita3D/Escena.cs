using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Trochita3D.Entidades;
using Tao.OpenGl;
using System.Drawing;
using System.Drawing.Imaging;

namespace Trochita3D
{
    public class Escena
    {
        #region Constantes del tren

        private static float[] TREN_LUZ = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_AMBIENTE = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_BRILLO = new float[] { 0.2f, 0.2f, 0.2f, 1 };
        private static int TREN_SHININESS = 180;

        private static double VELOCIDAD_TREN = 5000;
        private static double tiempo = 0;


        #endregion

        // Partes de la escena
        private SurfaceInitializer surfaceInitializer;
        private TerrainInitializer terrainInitializer;
        private WaterInitializer waterInitializer;
        private Skybox skybox;
        private Train Tren = new Train(TREN_LUZ_AMBIENTE, TREN_LUZ_BRILLO, TREN_LUZ, TREN_SHININESS);
        private bool daylight = true;

        Arbol[] arboles = Arbol.GenerarArbolesAleatorios(10);
        Punto[] posicionArboles = new Punto[10] {
            new Punto(0, 0, 0),
            new Punto(6, 6, 0),
            new Punto(-4, -4, 0),
            new Punto(6, 12, 0),
            new Punto(-10, 4, 0),
            new Punto(8, 15, 0),
            new Punto(-15, 7, 0),
            new Punto(1, -10, 0),
            new Punto(3, -15, 0),
            new Punto(15, 15, 0)
        };

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

        #endregion

        public void Inicializar(int width, int height)
        {
            this.skybox = new Skybox(width);
            this.surfaceInitializer = new SurfaceInitializer();
            this.surfaceInitializer.BuildSurface();
            this.terrainInitializer = new TerrainInitializer();
            this.waterInitializer = new WaterInitializer();
            Tren.Posicion = this.surfaceInitializer.GetPositionByDistancia(0);
            this.InicializarLuces();
        }

        /// <summary>
        /// Inicialización de la iluminación de la escena.
        /// </summary>
        private void InicializarLuces()
        {
            float[] light_color;
            float[] light_ambient;
            float[] secondary_light_color;
            float[] secondary_light_ambient;

            light_color = (daylight) ? day_light_color : night_light_color;
            light_ambient = (daylight) ? day_light_ambient : night_light_ambient;
            secondary_light_color = (daylight) ? secondary_day_light_color : secondary_night_light_color;
            secondary_light_ambient = (daylight) ? secondary_day_light_ambient : secondary_night_light_ambient;

            // Fuente de luz principal
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, light_color);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_position);
            Gl.glEnable(Gl.GL_LIGHT0);

            // Fuentes de luz secundarias
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[4] { -100.0f, -100.0f, 1.0f, 0.0f });
            Gl.glLightf(Gl.GL_LIGHT1, Gl.GL_SPOT_CUTOFF, 180.0f);
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

            Gl.glEnable(Gl.GL_LIGHTING);
        }

        public void DibujarSkybox()
        {
            skybox.Dibujar();
        }

        public void Dibujar()
        {
            surfaceInitializer.DrawSurface();
            terrainInitializer.DrawTerrain();
            waterInitializer.DrawPlaneOfWater();
            Tren.Draw();

            for (int i = 0; i < arboles.Length; ++i)
            {
                Gl.glPushMatrix();
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

            Punto posicion = surfaceInitializer.GetPositionByDistancia(distancia);

            Tren.Posicion = posicion;
        }

        public void SwitchDayLight()
        {
            this.daylight = !this.daylight;
            this.InicializarLuces();
        }
    }
}
