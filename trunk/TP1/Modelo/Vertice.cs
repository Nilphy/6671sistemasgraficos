using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Vertice
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vertice(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
