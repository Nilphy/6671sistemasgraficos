using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modelo;

namespace SistemasGraficos.Entidades
{
    public class ViewPort
    {
        public double XIzq { set; get; }
        public double XDer { set; get; }
        public double YArriba { set; get; }
        public double YAbajo { set; get; }

        public ViewPort(Rueda rueda)
        {
            XIzq = rueda.Centro.X - rueda.RadioExterno * 2;
            XDer = rueda.Centro.X + rueda.RadioExterno * 2;
            YArriba = rueda.Centro.Y + rueda.RadioExterno * 2;
            YAbajo = rueda.Centro.Y - rueda.RadioExterno * 2;
        }
    }
}
