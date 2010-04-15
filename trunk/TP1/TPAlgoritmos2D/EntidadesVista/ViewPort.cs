using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modelo;

namespace SistemasGraficos.Entidades
{
    public class ViewPort
    {
        public float XIzq { set; get; }
        public float XDer { set; get; }
        public float YArriba { set; get; }
        public float YAbajo { set; get; }

        public ViewPort(Rueda rueda)
        {
            XIzq = rueda.Centro.X - rueda.RadioExterno * 2;
            XDer = rueda.Centro.X + rueda.RadioExterno * 2;
            YArriba = rueda.Centro.Y + rueda.RadioExterno * 2;
            YAbajo = rueda.Centro.Y - rueda.RadioExterno * 2;
        }
    }
}
