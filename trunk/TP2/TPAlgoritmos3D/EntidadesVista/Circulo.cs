using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class Circulo
    {
        private Punto centro;
        private double radio;

        public Punto Centro { set { this.centro = value; } get { return this.centro; } }
        public double Radio { set { this.radio = value; } get { return this.radio; } }

        public Circulo(Punto centro, double radio)
        {
            this.Centro = centro;
            this.Radio = radio;
        }
    }
}
