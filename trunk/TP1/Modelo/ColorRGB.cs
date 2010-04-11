using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class ColorRGB
    {
        private int red;
        private int green;
        private int blue;

        public int Red
        {
            set { this.red = value; }
            get { return this.red; }
        }
        public int Green
        {
            set { this.green = value; }
            get { return this.green; }
        }
        public int Blue
        {
            set { this.blue = value; }
            get { return this.blue; }
        }

        public ColorRGB(int r, int g, int b)
        {
            this.Red = r;
            this.Blue = b;
            this.Green = g;
        }
    }
}
