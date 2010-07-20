using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;

namespace Trochita3D.Entidades
{
    public class Tabla
    {

        private IList<Punto> puntosControl = new List<Punto>();

        public Tabla()
        {
            puntosControl.Add(new Punto(0, -1, 0));
            puntosControl.Add(new Punto(0, -1, 0.5));
            puntosControl.Add(new Punto(0, 1, 0.5));
            puntosControl.Add(new Punto(0, 1, 0));
        }

        public Seccion GetSeccion()
        {
            return new Seccion(puntosControl);
        }

    }
}
