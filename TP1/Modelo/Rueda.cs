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
        public static double masa = 300d;

        public double RadioExterno { get; set; }
        public double RadioInterno
        {
            get { return this.RadioExterno * 0.9; }
        }
        public Vertice Centro { get; set; }
        public double VelocidadX { get; set; } // m / s
        public double VelocidadY { get; set; } // m / s
        public double Masa { get; set; }
        public double AnguloRotacion { get; set; }
        
        public int SentidoX
        {
            get { return (VelocidadX > 0) ? 1 : -1; }
        }

        public double VelocidadTraslacion
        {
            get { return Math.Sqrt(Math.Pow(VelocidadX, 2) + Math.Pow(VelocidadY, 2)); }
        }

        public double VelocidadAngular
        {
            get { return VelocidadTraslacion / RadioExterno; }
        }

        public double MomentoInercia
        {
            get { return Masa * Math.Pow(RadioExterno, 2); }
        }

        public Rueda()
        {
            this.Centro = verticeInicial;
            this.RadioExterno = radio;
            this.VelocidadX = 0;
            this.VelocidadY = 0;
            this.Masa = masa;
            this.AnguloRotacion = 0;
        }
    }
}
