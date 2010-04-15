using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class PuntoFlotante : Punto
    {
        private float x;
        private float y;
        
        public float X
        {
            set { this.x = value; }
            get { return this.x; }
        }
        public float Y
        {
            set { this.y = value; }
            get { return this.y; }
        }

        public PuntoFlotante(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public PuntoFlotante(float x, float y, Punto origenCoordenadas)
        {
            this.X = x;
            this.Y = y;
            this.OrigenCoordenadas = origenCoordenadas;
        }

        public override float GetXFlotante()
        {
            return this.X;
        }

        public override float GetYFlotante()
        {
            return this.Y;
        }

        public override int GetXEntero()
        {
            return (int)Math.Round(this.x, MidpointRounding.ToEven);
        }

        public override int GetYEntero()
        {
            return (int)Math.Round(this.y, MidpointRounding.ToEven);
        }
    }
}
