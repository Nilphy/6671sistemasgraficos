using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public abstract class Figura
    {
        public Punto Posicion { set; get; }
        public double LongitudX { set; get; }
        public double LongitudY { set; get; }
        public double LongitudZ { set; get; }
        public int CantidadDivisionesX { set; get; }
        public int CantidadDivisionesY { set; get; }
        public int CantidadDivisionesZ { set; get; }
        public double PasoX { set; get; }
        public double PasoY { set; get; }
        public double PasoZ { set; get; }
        
        public float[] Luz { set; get; }
        public float[] LuzAmbiente { set; get; }
        public float[] LuzBrillo { set; get; }
        public int Shininess { set; get; }

        protected IList<CaraFigura> caras;

        public Figura(double ancho, double largo, double alto, Punto posicion, float[] luz, float[] luzAmbiente, float[] luzBrillo, int shininess)
        {
            this.LongitudX = ancho;
            this.LongitudZ = alto;
            this.LongitudY = largo;
            this.Posicion = posicion;
            this.Luz = luz;
            this.LuzAmbiente = luzAmbiente;
            this.LuzBrillo = luzBrillo;
            this.Shininess = shininess;

            this.CalcularDivisionesYPasos();

            this.Generar();
        }

        #region Métodos abstractos 

        protected abstract void CalcularDivisionesYPasos();

        public abstract void Generar();

        #endregion

        public void Draw()
        {
            foreach (CaraFigura cara in caras)
            {
                cara.Draw();
            }
        }
    }
}
