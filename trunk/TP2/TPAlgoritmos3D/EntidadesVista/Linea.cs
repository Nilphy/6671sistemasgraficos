using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class Linea
    {
        private Punto inicio;
        private Punto fin;

        public Punto Inicio
        {
            set { this.inicio = value; }
            get { return this.inicio; }
        }
        public Punto Fin
        {
            set { this.fin = value; }
            get { return this.fin; }
        }

        public Linea(Punto inicio, Punto fin)
        {
            this.Inicio = inicio;
            this.Fin = fin;
        }

        public bool TienePendientePositiva()
        {
            if (this.GetPendiente() > 0) return true;
            else return false;
        }

        public double GetPendiente()
        {
            return ( (this.Fin.GetYFlotante() - this.Inicio.GetYFlotante()) / 
                (this.Fin.GetXFlotante() - this.Inicio.GetXFlotante()) );
        }


    }
}
