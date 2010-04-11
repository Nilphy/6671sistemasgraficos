using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Modelo
{
    public class Terreno
    {
        public IList Vertices { get; set; }
        public ColorRGB Color { get; set; }

        public Terreno()
        {
            Vertices = new ArrayList();

            Vertices.Add(new Vertice(-10,1));
            Vertices.Add(new Vertice(-8,0.5f));
            Vertices.Add(new Vertice(-5,-1));
            Vertices.Add(new Vertice(-3.5f,-2));
            Vertices.Add(new Vertice(-2.3f,-2.5f));
            Vertices.Add(new Vertice(-1.8f, -1.8f));
            Vertices.Add(new Vertice(-1, -1.8f));
            Vertices.Add(new Vertice(1, -3));
            Vertices.Add(new Vertice(2, -3.2f));
            Vertices.Add(new Vertice(4, -2.8f));
            Vertices.Add(new Vertice(5, -1.8f));
            Vertices.Add(new Vertice(7, -1));
            Vertices.Add(new Vertice(10, 1));

            Color = new ColorRGB(200, 0, 50);
        }
    }
}
