using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using Trochita3D.Entidades;
using Tao.OpenGl;

namespace Trochita3D
{
    public class Escena
    {
        #region Luces del tren

        private static float[] TREN_LUZ = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_AMBIENTE = new float[] { 0.2f, 0.25f, 0.3f, 1 };
        private static float[] TREN_LUZ_BRILLO = new float[] { 0.2f, 0.2f, 0.2f, 1 };
        private static int TREN_SHININESS = 180;

        #endregion

        // Partes de la escena
        private SurfaceInitializer surfaceInitializer = new SurfaceInitializer();
        private TerrainInitializer terrainInitializer = new TerrainInitializer();
        private WaterInitializer waterInitializer = new WaterInitializer();
        private Train Tren = new Train(TREN_LUZ_AMBIENTE, TREN_LUZ_BRILLO, TREN_LUZ, TREN_SHININESS);

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
        
        private float[] light_color = new float[4] { 0.80f, 0.80f, 0.80f, 1.0f };
        private float[] light_position = new float[4] { 7.0f, 7.0f, 10.0f, 0.0f };
        private float[] light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        private float[] secondary_light_color = new float[4] { 0.20f, 0.20f, 0.20f, 1.0f };
        private float[] secondary_light_ambient = new float[4] { 0.05f, 0.05f, 0.05f, 1.0f };

        #endregion

        public void Inicializar()
        {
            this.surfaceInitializer = new SurfaceInitializer();
            this.surfaceInitializer.BuildSurface();
            
            this.InicializarLuces();
        }

        /// <summary>
        /// Inicialización de la iluminación de la escena.
        /// </summary>
        private void InicializarLuces()
        {
            // Fuente de luz principal
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, light_color);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, light_position);
            Gl.glEnable(Gl.GL_LIGHT0);

            // Fuentes de luz secundarias
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[4] { -10.0f, -10.0f, 1.0f, 0.0f });
            Gl.glEnable(Gl.GL_LIGHT1);

            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[4] { -10.0f, 10.0f, 1.0f, 0.0f });
            Gl.glEnable(Gl.GL_LIGHT2);

            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[4] { 10.0f, -10.0f, 1.0f, 0.0f });
            Gl.glEnable(Gl.GL_LIGHT3);

            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_DIFFUSE, secondary_light_color);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_AMBIENT, secondary_light_ambient);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_POSITION, new float[4] { 10.0f, 10.0f, 1.0f, 0.0f });
            Gl.glEnable(Gl.GL_LIGHT4);

            Gl.glEnable(Gl.GL_LIGHTING);
        }

        public void Dibujar()
        {
            //surfaceInitializer.DrawSurface();
            //terrainInitializer.DrawTerrain();
            //waterInitializer.DrawPlaneOfWater();
            Tren.Draw();
            /*
            for (int i = 0; i < arboles.Length; ++i)
            {
                Gl.glPushMatrix();
                Punto posicion = posicionArboles[i];
                Gl.glTranslated(posicion.X, posicion.Y, posicion.Z);
                arboles[i].Dibujar();
                Gl.glPopMatrix();
            } 
             * */
        }
    }
}
