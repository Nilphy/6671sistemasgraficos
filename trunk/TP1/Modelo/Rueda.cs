using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Rueda
    {
        public static Vertice verticeInicial = new Vertice(-7, 3);
        public static float radio = 2;
        public static float radioInterno = 1.9f;
        public static float omegaInicial = 0;

        public float RadioExterno { get; set; }
        public float RadioInterno { get; set; }
        public Vertice Centro { get; set; }
        public float Omega { get; set; }

        public Rueda()
        {
            this.Centro = verticeInicial;
            this.RadioExterno = radio;
            this.RadioInterno = RadioInterno;
            this.Omega = omegaInicial;
        }
    }
}
