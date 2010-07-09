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
        public Cuboide(double ancho, double largo, double alto, int cantidadDivisionesAncho, int cantidadDivisionesLargo, int cantidadDivisionesAlto, Punto posicion)
        {
            this.Ancho = ancho;
            this.Alto = alto;
            this.Largo = largo;
            this.CantidadDivisionesAlto = cantidadDivisionesAlto;
            this.CantidadDivisionesAncho = cantidadDivisionesAncho;
            this.CantidadDivisionesLargo = cantidadDivisionesLargo;
            this.Posicion = posicion;
            this.Generar();
        }
    }
}
