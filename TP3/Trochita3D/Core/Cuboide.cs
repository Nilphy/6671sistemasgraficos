using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Common.Utils;

namespace Trochita3D.Core
{
    public class Cuboide : Figura
    {
        public Cuboide(
            double ancho, 
            double largo, 
            double alto, 
            int cantidadDivisionesAncho, 
            int cantidadDivisionesLargo, 
            int cantidadDivisionesAlto, 
            Punto posicion,
            float[] luzAmbiente, 
            float[] luzBrillo,
            float[] luz,
            int shininess)
        {
            this.Ancho = ancho;
            this.Alto = alto;
            this.Largo = largo;
            this.CantidadDivisionesAlto = cantidadDivisionesAlto;
            this.CantidadDivisionesAncho = cantidadDivisionesAncho;
            this.CantidadDivisionesLargo = cantidadDivisionesLargo;
            this.Posicion = posicion;
            this.Luz = luz;
            this.LuzAmbiente = luzAmbiente;
            this.LuzBrillo = luzBrillo;
            this.Shininess = shininess;
            this.Generar();
        }

        public override void Generar()
        {
            caras = new List<CaraFigura>();

            caras.Add(new CaraCuboide(this, OrientacionesCara.Abajo, 0, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));
            caras.Add(new CaraCuboide(this, OrientacionesCara.Adelante, 1, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));
            caras.Add(new CaraCuboide(this, OrientacionesCara.Arriba, 2, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));
            caras.Add(new CaraCuboide(this, OrientacionesCara.Atraz, 3, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));
            caras.Add(new CaraCuboide(this, OrientacionesCara.Derecha, 4, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));
            caras.Add(new CaraCuboide(this, OrientacionesCara.Izquierda, 5, this.Luz, this.LuzBrillo, this.LuzAmbiente, this.Shininess));

            foreach (CaraCuboide cara in caras)
            {
                cara.Generar();
            }
        }
    }
}
