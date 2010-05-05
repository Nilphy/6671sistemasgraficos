using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public abstract class Punto
    {
        private Punto origenCoordenadas;
        public Punto OrigenCoordenadas
        {
            set { this.origenCoordenadas = value; }
            get {
                if (this.origenCoordenadas == null) this.origenCoordenadas = new PuntoEntero(0, 0);
                return this.origenCoordenadas; 
            }
        }

        public abstract float GetXFlotante();
        public abstract float GetYFlotante();
        public abstract int GetXEntero();
        public abstract int GetYEntero();
        
        public int GetCuadrante()
        {
            if ((this.GetXFlotante() - OrigenCoordenadas.GetXFlotante()) >= 0 &&
                (GetYFlotante() - OrigenCoordenadas.GetYFlotante()) >= 0)
                return 1;

            if ((this.GetXFlotante() - OrigenCoordenadas.GetXFlotante()) <= 0 &&
                (GetYFlotante() - OrigenCoordenadas.GetYFlotante()) >= 0)
                return 2;

            if ((this.GetXFlotante() - OrigenCoordenadas.GetXFlotante()) <= 0 &&
                (this.GetYFlotante() - OrigenCoordenadas.GetYFlotante()) <= 0)
                return 3;

            if ((this.GetXFlotante() - OrigenCoordenadas.GetXFlotante()) >= 0 &&
                (this.GetYFlotante() - OrigenCoordenadas.GetYFlotante()) <= 0)
                return 4;

            throw new InvalidProgramException("Está mál hecho el GetCuadrante");
        }
    }
}
