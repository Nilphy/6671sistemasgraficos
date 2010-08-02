﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Trochita3D.Entidades;
using Tao.OpenGl;
using System.Drawing;
using System.Drawing.Imaging;
using Common.Utils;

namespace Trochita3D
{
    public class Escena
    {
        #region Constantes del tren

        private static float[] TREN_LUZ = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_AMBIENTE = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_BRILLO = new float[] { 0.2f, 0.2f, 0.2f, 1 };
        private static int TREN_SHININESS = 180;

        private static double VELOCIDAD_TREN = 1000;
        private static double tiempo = 0;


        #endregion

        // Partes de la escena
        private SurfaceInitializer surfaceInitializer;
        private TerrainInitializer terrainInitializer;
        private WaterInitializer waterInitializer;
        private Skybox skybox;
        private Train Tren = new Train(TREN_LUZ_AMBIENTE, TREN_LUZ_BRILLO, TREN_LUZ, TREN_SHININESS);
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

        public void Inicializar(int width, int height)
        {
            this.skybox = new Skybox(width);
            this.surfaceInitializer = new SurfaceInitializer();
            this.surfaceInitializer.BuildSurface();
            this.terrainInitializer = new TerrainInitializer();
            this.waterInitializer = new WaterInitializer();
            Tren.Posicion = this.surfaceInitializer.GetPositionByDistancia(0);
            arboles = Arbol.GenerarArbolesAleatorios(posicionArboles.Count());
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

            Gl.glLightf(Gl.GL_LIGHT6, Gl.GL_SPOT_CUTOFF, 10.0f);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_DIFFUSE, day_light_color);
            Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_AMBIENT, day_light_ambient);
            //Gl.glLightfv(Gl.GL_LIGHT6, Gl.GL_SPOT_DIRECTION, this.light_linterna_direction);
            //Gl.glLighti(Gl.GL_LIGHT6, Gl.GL_QUADRATIC_ATTENUATION, 1);

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
            Tren.Draw(!this.daylight);
            
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

            Punto posicion = surfaceInitializer.GetPositionByDistancia(distancia);
            double inclinacion = surfaceInitializer.GetInclinacionByDistancia(distancia);

            Tren.Posicion = posicion;
            Tren.InclinaciónLocomotora = inclinacion;
        }

        public void SwitchDayLight()
        {
            this.daylight = !this.daylight;
            this.InicializarLuces();
            if (daylight) Gl.glDisable(Gl.GL_LIGHT6);
            else Gl.glEnable(Gl.GL_LIGHT6);
        }
    }
}
