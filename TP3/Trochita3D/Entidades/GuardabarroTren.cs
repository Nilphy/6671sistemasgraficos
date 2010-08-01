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
            Punto posicion,
            float[] luz,
            float[] luzAmbiente,
            float[] luzBrillo,
            int shininess)
            : base(ancho, largo, alto, posicion, luz, luzAmbiente, luzBrillo, shininess) { }

        #region Metodos heredados

        // la cantidad de divisiones tiene que ser impar, porque la cantidad de pixeles
        // es 1 + que la cantidad de divisiones y se divide en dos para ambas caras
        protected override void CalcularDivisionesYPasos()
        {
            this.CantidadDivisionesX = 8;
            this.CantidadDivisionesY = 8;
            this.CantidadDivisionesZ = 8;

            this.PasoX = this.LongitudX / this.CantidadDivisionesX;
            this.PasoY = this.LongitudY / this.CantidadDivisionesY;
            this.PasoZ = this.LongitudZ / this.CantidadDivisionesZ;
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

        #endregion
    }
}
