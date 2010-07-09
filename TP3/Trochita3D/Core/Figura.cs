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

        private IList<CaraFigura> caras;

        public void Generar()
        {
            caras = new List<CaraFigura>();

            caras.Add(new CaraFigura(this, OrientacionesCara.Abajo, 0));
            caras.Add(new CaraFigura(this, OrientacionesCara.Adelante, 1));
            caras.Add(new CaraFigura(this, OrientacionesCara.Arriba, 2));
            caras.Add(new CaraFigura(this, OrientacionesCara.Atraz, 3));
            caras.Add(new CaraFigura(this, OrientacionesCara.Derecha, 4));
            caras.Add(new CaraFigura(this, OrientacionesCara.Izquierda, 5));

            foreach (CaraFigura cara in caras)
            {
                cara.Generar();
            }
        }

        public void Draw()
        {
            foreach (CaraFigura cara in caras)
            {
                cara.Draw();
            }
        }
    }
}
