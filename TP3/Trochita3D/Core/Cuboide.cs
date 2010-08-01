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
            Punto posicion,
            float[] luzAmbiente,
            float[] luzBrillo,
            float[] luz,
            int shininess)
            : base(ancho, largo, alto, posicion, luz, luzAmbiente, luzBrillo, shininess) { }

        #region Métodos heredados

        public override void Generar()
        {
            this.CalcularDivisionesYPasos();

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

        /// <summary>
        /// Inicialmente voy a poner 10 divisiones por cara y dps veré
        /// </summary>
        protected override void CalcularDivisionesYPasos()
        {
            this.PasoX = this.LongitudX / 10;
            this.PasoY = this.LongitudY / 10;
            this.PasoZ = this.LongitudZ / 10;

            this.CantidadDivisionesX = 10;
            this.CantidadDivisionesY = 10;
            this.CantidadDivisionesZ = 10;
        }

        #endregion 
    }
}
