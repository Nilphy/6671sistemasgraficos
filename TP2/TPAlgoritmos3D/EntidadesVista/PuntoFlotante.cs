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

        /// <summary>
        /// No modifica el valor de this... devuelve un nuevo punto flotante con el valor multiplicado
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        public PuntoFlotante MultiplicarEscalar(double p)
        {
            return new PuntoFlotante((float)(this.GetXFlotante() * p), (float)(this.GetYFlotante() *p));
        }
        
        /// <summary>
        /// No modifica el valor de this, ni del otro, devuelve un nuevo punto flotante con el valor de la suma de ambos
        /// </summary>
        /// <param name="punto"></param>
        /// <returns></returns>
        public PuntoFlotante SumarPunto(PuntoFlotante punto)
        {
            return new PuntoFlotante(this.GetXFlotante() + punto.GetXFlotante(), this.GetYFlotante() + punto.GetYFlotante());
        }
    }
}
