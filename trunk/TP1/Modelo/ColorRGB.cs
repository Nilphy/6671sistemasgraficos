using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class ColorRGB
    {
        private float red;
        private float green;
        private float blue;

        public float Red
        {
            set { this.red = value; }
            get { return this.red; }
        }
        public float Green
        {
            set { this.green = value; }
            get { return this.green; }
        }
        public float Blue
        {
            set { this.blue = value; }
            get { return this.blue; }
        }

        public ColorRGB(float r, float g, float b)
        {
            this.Red = r;
            this.Blue = b;
            this.Green = g;
        }
    }
}
