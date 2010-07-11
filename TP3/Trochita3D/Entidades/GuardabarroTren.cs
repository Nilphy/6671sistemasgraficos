using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Common.Utils;

namespace Trochita3D.Core
{
    public class GuardabarroTren : Figura
    {
        public GuardabarroTren(
            double ancho, 
            double largo, 
            double alto, 
            int cantidadDivisionesAncho, 
            int cantidadDivisionesLargo, 
            int cantidadDivisionesAlto, 
            Punto posicion,
            float[] luz,
            float[] luzAmbiente, 
            float[] luzBrillo,
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

            caras.Add(new CaraGuardabarro(this, OrientacionesCara.Derecha, 0, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));
            caras.Add(new CaraGuardabarro(this, OrientacionesCara.Izquierda, 1, this.LuzAmbiente, this.LuzBrillo, this.Luz, this.Shininess));

            foreach (CaraGuardabarro cara in caras)
            {
                cara.Generar();
            }
        }
    }
}
