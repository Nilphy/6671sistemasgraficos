using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.FreeGlut;

using Modelo;
using SistemasGraficos.EstrategiasDibujo;
using TPAlgoritmos3D;
using TPAlgoritmos3D.EntidadesVista;

namespace SistemasGraficos.Entidades
{
    public class Vista
    {
        // Para los calculos y los límites de las cosas
        public double X_MIN_MUNDO = 0;
        public double X_MAX_MUNDO = 100;
        public double Y_MIN_MUNDO = 0;
        public double Y_MAX_MUNDO = 100;

        public double X_MIN_CAMARA = -10;
        public double X_MAX_CAMARA = 10;
        public double Y_MIN_CAMARA = -10;
        public double Y_MAX_CAMARA = 10;

        // Creo que es 20 pero con 15 seguro queda mejor :S
        public double X_MIN_VIEWPORT_ESCENA3D = -10;
        public double X_MAX_VIEWPORT_ESCENA3D = 10;
        public double Y_MIN_VIEWPORT_ESCENA3D = -5;
        public double Y_MAX_VIEWPORT_ESCENA3D = 5;

        public Escena escena { get; set; }

        private IList<PuntoFlotante> puntosBzier = new List<PuntoFlotante>();
        private IList<PuntoFlotante> puntosBspline = new List<PuntoFlotante>();
        private IList<PuntoFlotante> puntosCamino = null;
        private IList<PuntoFlotante> puntosRecorridoCamara = null;

        public Vista(Escena escena)
        {
            this.escena = escena;
        }

        #region Funciones de escalado que están acá por no tener un mejor lugar

        public void EscalarMundoToEscena3D()
        {
            Gl.glTranslated(0,X_MIN_VIEWPORT_ESCENA3D, Y_MIN_VIEWPORT_ESCENA3D);
            Gl.glScaled(1,(X_MAX_VIEWPORT_ESCENA3D - X_MIN_VIEWPORT_ESCENA3D) / (X_MAX_MUNDO - X_MIN_MUNDO), (Y_MAX_VIEWPORT_ESCENA3D - Y_MIN_VIEWPORT_ESCENA3D) / (Y_MAX_MUNDO - Y_MIN_MUNDO));
            Gl.glTranslated(0,-X_MIN_MUNDO, -Y_MIN_MUNDO);
        }

        public IList<PuntoFlotante> EscalarPuntosVentanitas(IList<PuntoFlotante> puntosBzier, Boolean mundoOcamara)
        {
            // Obtengo los límites de los puntos y los escalo a los límites del mundo

            float maxX = this.GetMaxX(puntosBzier);
            float maxY = this.getMaxY(puntosBzier);
            float minX = this.GetMinX(puntosBzier);
            float minY = this.GetMinY(puntosBzier);
            

            IList<PuntoFlotante>result = this.TrasladarPuntosAlOrigen(puntosBzier, minX, minY);
            result = this.EscalarPuntos(result, maxX, maxY, minX, minY, mundoOcamara);
            this.TrasladarPuntosAlComienzoDelMundo(result, mundoOcamara);

            return result;
        }

        private void TrasladarPuntosAlComienzoDelMundo(IList<PuntoFlotante> puntosBzier, Boolean isMundoOCamara)
        {
            double xmin; double ymin;

            if (isMundoOCamara)
            {
                xmin = X_MIN_MUNDO;
                ymin = Y_MIN_MUNDO;
            }
            else
            {
                xmin = X_MIN_CAMARA;
                ymin = Y_MIN_CAMARA;
            }


            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() + xmin);
                punto.SetYFlotante(punto.GetYFlotante() + ymin);
            }
        }

        private IList<PuntoFlotante> EscalarPuntos(IList<PuntoFlotante> puntosBzier, float maxX, float maxY, float minX, float minY, Boolean isMundoOcamara)
        {
            double xMin; double xMax; double yMin; double yMax;

            if (isMundoOcamara)
            {
                xMin = X_MIN_MUNDO;
                xMax = X_MAX_MUNDO;
                yMin = Y_MIN_MUNDO;
                yMax = Y_MAX_MUNDO;
            }
            else
            {
                xMin = X_MIN_CAMARA;
                xMax = X_MAX_CAMARA;
                yMin = Y_MIN_CAMARA;
                yMax = Y_MAX_CAMARA;
            }


            foreach (PuntoFlotante punto in puntosBzier)
            {
                punto.SetXFlotante(punto.GetXFlotante() * ((xMax - xMin) / (maxX - minX)));
                punto.SetYFlotante(punto.GetYFlotante() * ((yMax - yMin) / (maxY - minY)));
            }

            return puntosBzier;
        }

        private float GetMinY(IList<PuntoFlotante> puntosBzier)
        {
            float minY = float.MaxValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetYFlotante() < minY) minY = (float)punto.GetYFlotante();
            }

            return minY;
        }

        private float GetMinX(IList<PuntoFlotante> puntosBzier)
        {
            float minX = float.MaxValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetXFlotante() < minX) minX = (float)punto.GetXFlotante();
            }

            return minX;
        }

        private float getMaxY(IList<PuntoFlotante> puntosBzier)
        {
            float maxY = float.MinValue;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetYFlotante() > maxY) maxY = (float)punto.GetYFlotante();
            }

            return maxY;
        }

        private float GetMaxX(IList<PuntoFlotante> puntosBzier)
        {
            float maxX = 0;

            foreach (PuntoFlotante punto in puntosBzier)
            {
                if (punto.GetXFlotante() > maxX) maxX = (float)punto.GetXFlotante();
            }

            return maxX;
        }

        private IList<PuntoFlotante> TrasladarPuntosAlOrigen(IList<PuntoFlotante> puntosBzier, float minX, float minY)
        {
            IList<PuntoFlotante> result = new List<PuntoFlotante>();

            foreach (PuntoFlotante punto in puntosBzier)
            {
                result.Add(punto.SumarPunto(new PuntoFlotante(-minX, -minY)));
            }

            return result;
        }

        internal float[] ConvertirPuntos(IList puntosBzierEscalados)
        {
            // algo así para pasar de la lista al vector

            int i;
            int cantidadPuntos = puntosBzierEscalados.Count;
            float[] default_curve = new float[cantidadPuntos * 2];

            for (i = 0; i < cantidadPuntos; i++)
            {
                default_curve[i * 2 + 0] = (float)((PuntoFlotante)puntosBzierEscalados[i]).GetXFlotante();
                default_curve[i * 2 + 1] = (float)((PuntoFlotante)puntosBzierEscalados[i]).GetYFlotante();
            }

            return default_curve;
        }



        #endregion

        public void AddPuntosBzier(double x, double y)
        {
            puntosBzier.Add(new PuntoFlotante(x, y));
        }
        
        internal IList<PuntoFlotante> GetPuntosBzier()
        {
            return puntosBzier;
        }

        public void AddPuntosBspline(double x, double y)
        {
            this.puntosBspline.Add(new PuntoFlotante(x, y));
        }

        internal IList<PuntoFlotante> GetPuntosBspline()
        {
            return puntosBspline;
        }

        public void DibujarTerreno2D()
        {
            Terreno2DPlotter.DibujarTerreno2D(this.GetPuntosBzier());
        }

        public void DibujarCamaraPath()
        {
            CamaraPathPlotter.DibujarCamaraPath(this.GetPuntosBspline());
        }

        public void DibujarRueda()
        {
            Glu.GLUquadric quad = Glu.gluNewQuadric();
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glColor3d(1, 0, 0);

            Gl.glPushMatrix();

            Gl.glTranslated(0, escena.Rueda.Centro.X, escena.Rueda.Centro.Y);
            Gl.glRotated(escena.Rueda.AnguloRotacion * 180 / Math.PI, 1, 0, 0);

            Gl.glRotated(90, 0, 1, 0);

            Gl.glColor3d(1, 0, 0);
            Glu.gluDisk(quad, escena.Rueda.RadioInterno, escena.Rueda.RadioExterno, 20, 20);

            Gl.glColor3d(1, 1, 1);
            Glu.gluDisk(quad, 0, escena.Rueda.RadioInterno, 20, 20);

            Gl.glColor3d(1, 0, 0);
            Glu.gluCylinder(quad, escena.Rueda.RadioExterno, escena.Rueda.RadioExterno, 0.3, 20, 20);

            Gl.glTranslated(0, 0, 0.3);

            Gl.glColor3d(1, 0, 0);
            Glu.gluDisk(quad, escena.Rueda.RadioInterno, escena.Rueda.RadioExterno, 20, 20);
            Gl.glColor3d(1, 1, 1);
            Glu.gluDisk(quad, 0, escena.Rueda.RadioInterno, 20, 20);

            Gl.glPopMatrix();

            this.DibujarBarrita();

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glColor3d(1, 1, 1);

            Glu.gluDeleteQuadric(quad);
        }

        private void DibujarBarrita()
        {
            Gl.glColor3d(0, 0, 1);
            double size = escena.Rueda.RadioInterno / 2;

            Gl.glPushMatrix();

            // Centro la barra.
            Gl.glTranslated(-size/4, escena.Rueda.Centro.X, escena.Rueda.Centro.Y);
            Gl.glRotated(escena.Rueda.AnguloRotacion * 180 / Math.PI, 1, 0, 0);
            Gl.glTranslated(-size/2, -size/2, 0);
            Gl.glRotated(90, 0, 1, 0); // Rota para quedar acostada la barra

            // En lugar de dibujar una barra 3D, se puede dibujar simplemente un rectangulo\
            // estampado en la rueda.
            //Gl.glRectd(-size / 2, -size / 2, size / 2, size / 2); 

            
            Gl.glBegin(Gl.GL_QUADS);
            
            // Tapa de abajo
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(size, 0, 0);
            Gl.glVertex3d(size, size*4, 0);
            Gl.glVertex3d(0, size*4, 0);

            // Tapa de arriba
            Gl.glVertex3d(0, 0, size);
            Gl.glVertex3d(size, 0, size);
            Gl.glVertex3d(size, size*4, size);
            Gl.glVertex3d(0, size*4, size);

            // Tapa de atras
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(size, 0, 0);
            Gl.glVertex3d(size, 0, size);
            Gl.glVertex3d(0, 0, size);

            // Tapa de derecha
            Gl.glVertex3d(size, 0, 0);
            Gl.glVertex3d(size, size*4, 0);
            Gl.glVertex3d(size, size*4, size);
            Gl.glVertex3d(size, 0, size);

            // Tapa de adelante
            Gl.glVertex3d(size, size*4, 0);
            Gl.glVertex3d(size, size*4, size);
            Gl.glVertex3d(0, size*4, size);
            Gl.glVertex3d(0, size*4, 0);

            // Tapa de izquierda
            Gl.glVertex3d(0, size*4, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0, 0, size);
            Gl.glVertex3d(0, size*4, size);

            Gl.glEnd();
            
            Gl.glPopMatrix();
        }

        internal IList<PuntoFlotante> GetPuntosCurvaBzier()
        {
            return this.puntosCamino;
        }

        /// <summary>
        /// Escala los puntos ingresados por el usuario para poder usarlos como posición de la cámara en la escena
        /// </summary>
        /// <returns></returns>
        private IList<PuntoFlotante> GetPuntosBsplineEscalados()
        {
            return this.EscalarPuntosVentanitas(this.GetPuntosBspline(), false);
        }

        public IList<PuntoFlotante> GetPuntosPosicionCamara()
        {
            return this.puntosRecorridoCamara;
        }

        public IList<PuntoFlotante> GetPuntosCamino()
        {
            return this.puntosCamino;
        }

        public void CrearCamino()
        {
            // Se obtienen los puntos seleccionados por el usuario
            IList<PuntoFlotante> puntosPoligonoControlBzier = this.GetPuntosBzier();

            // Se escalan los puntos a las coordenadas de mundo, para poder controlar el paso de discretisación
            puntosPoligonoControlBzier = this.EscalarPuntosVentanitas(puntosPoligonoControlBzier, true);

            // Se crea la curva
            CurvaBzierSegmentosCubicos curva = new CurvaBzierSegmentosCubicos(puntosPoligonoControlBzier);

            // Se obtienen los puntos discretos de la curva
            this.puntosCamino = curva.GetPuntosDiscretos(0.01);
        }

        public void CrearRecorridoCamara()
        {
            // Se obtienen los puntos ingresados por el usuario escalados a las coordenadas de la escena
            IList<PuntoFlotante> puntosCamara = this.GetPuntosBsplineEscalados();

            // Se crea la curva Bspline
            CurvaBsplineSegmentosCubicos curva = new CurvaBsplineSegmentosCubicos(puntosCamara);

            // Se devuelven los puntos discretos de la curva bspline
            this.puntosRecorridoCamara = curva.GetPuntosDiscretos(0.001);
        }

        public void LimpiarRecorridoCamara()
        {
            this.puntosBspline = new List<PuntoFlotante>();
            this.puntosRecorridoCamara = null;
        }

        public void LimpiarCamino()
        {
            this.puntosBzier = new List<PuntoFlotante>();
            this.puntosCamino = null;
        }
    }
}
