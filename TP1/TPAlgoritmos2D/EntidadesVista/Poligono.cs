using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Modelo;

namespace SistemasGraficos.Entidades
{
    public class Poligono
    {
        private IList puntos;
        private ColorRGB colorRelleno;
        private ColorRGB colorLinea;

        public IList Puntos 
        {
            set { this.puntos = value; }
            get { return this.puntos; }
        }
        public ColorRGB ColorRelleno
        {
            set { this.colorRelleno = value; }
            get { return this.colorRelleno; }
        }
        public ColorRGB ColorLinea
        {
            set { this.colorLinea = value; }
            get { return this.colorLinea; }
        }

        public Poligono()
        {
            Puntos = new ArrayList(); 
        }
    }
}
