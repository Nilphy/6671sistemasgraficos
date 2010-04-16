using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Rueda
    {
        public static Vertice verticeInicial = new Vertice(-7, 3);
        public static double radio = 2d;
        public static double radioInterno = 1.9d;
        public static double omegaInicial = 0d;

        public double RadioExterno { get; set; }
        public double RadioInterno { get; set; }
        public Vertice Centro { get; set; }
        public double Omega { get; set; }
        public double VelocidadX { get; set; } // m / s
        public double VelocidadY { get; set; } // m / s
        public double Masa { get; set; }

        public double VelocidadTraslacion { get; set; }

        public double VelocidadAngular
        {
            get
            {
                return VelocidadTraslacion / RadioExterno;
            }
        }

        public double MomentoInercia
        {
            get { return Masa * RadioExterno * RadioExterno; }
        }

        public Rueda()
        {
            this.Centro = verticeInicial;
            this.RadioExterno = radio;
            this.RadioInterno = RadioInterno;
            this.Omega = omegaInicial;
        }
    }
}
