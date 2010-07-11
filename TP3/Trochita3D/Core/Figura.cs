using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public abstract class Figura
    {
        public Punto Posicion { set; get; }
        public double Ancho { set; get; }
        public double Largo { set; get; }
        public double Alto { set; get; }
        public int CantidadDivisionesAncho { set; get; }
        public int CantidadDivisionesAlto { set; get; }
        public int CantidadDivisionesLargo { set; get; }
        public float[] Luz { set; get; }
        public float[] LuzAmbiente { set; get; }
        public float[] LuzBrillo { set; get; }
        public int Shininess { set; get; }

        // Calculan la distancia entre dos vértices dada la cantidad de divisiones
        public double PasoX
        {
            get
            {
                return Ancho / this.CantidadDivisionesAncho;
            }
        }
        public double PasoY
        {
            get
            {
                return Largo / this.CantidadDivisionesLargo;
            }
        }
        public double PasoZ
        {
            get
            {
                return Alto / this.CantidadDivisionesAlto;
            }
        }

        protected IList<CaraFigura> caras;

        public abstract void Generar();

        public void Draw()
        {
            foreach (CaraFigura cara in caras)
            {
                cara.Draw();
            }
        }
    }
}
