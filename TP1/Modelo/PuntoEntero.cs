﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class PuntoEntero : Punto
    {
        private int x;
        private int y;

        public int X
        {
            set { this.x = value; }
            get { return this.x; }
        }
        public int Y
        {
            set { this.y = value; }
            get { return this.y; }
        }

        public PuntoEntero(int x, int y, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public PuntoEntero(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override float GetXFlotante()
        {
            return (float)this.X;
        }

        public override float GetYFlotante()
        {
            return (float)this.Y;
        }

        public override int GetXEntero()
        {
            return this.X;
        }

        public override int GetYEntero()
        {
            return this.Y;
        }
    }
}