using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Vertice
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vertice(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
