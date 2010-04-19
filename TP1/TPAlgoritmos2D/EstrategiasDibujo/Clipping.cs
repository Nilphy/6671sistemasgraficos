using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using SistemasGraficos.Entidades;
using System.Collections;

namespace SistemasGraficos.EstrategiasDibujo
{
    public static class Clipping
    {
        #region Clipping Poligono
        public static IList RecortarPoligono(IList puntos, ViewPort viewport)
        {
            IList puntosClippeados;

            Punto pia = new PuntoFlotante(viewport.XIzq, viewport.YAbajo);
            Punto pda = new PuntoFlotante(viewport.XDer, viewport.YAbajo);
            Punto pdb = new PuntoFlotante(viewport.XDer, viewport.YArriba);
            Punto pib = new PuntoFlotante(viewport.XIzq, viewport.YArriba);

            puntosClippeados = Recortar(puntos, pia, pda);
            puntosClippeados = Recortar(puntosClippeados, pda, pdb);
            puntosClippeados = Recortar(puntosClippeados, pdb, pib);
            puntosClippeados = Recortar(puntosClippeados, pib, pia);

            return puntosClippeados;
        }

        private static IList Recortar(IList listado, Punto vertice1, Punto vertice2)
        {
            IList listadoClip = new ArrayList();

            if (listado.Count == 0)
                return listadoClip;

            Punto s = (Punto)listado[listado.Count - 1];

            foreach (Punto p in listado)
            {
                if (EsInterior(p, vertice1, vertice2))
                {
                    if (EsInterior(s, vertice1, vertice2))
                    {
                        listadoClip.Add(p);
                    }
                    else
                    {
                        listadoClip.Add(Interseccion(s, p, vertice1, vertice2));
                        listadoClip.Add(p);
                    }
                }
                else if (EsInterior(s, vertice1, vertice2))
                {
                    listadoClip.Add(Interseccion(s, p, vertice1, vertice2));
                }

                s = p;
            }

            return listadoClip;
        }

        private static bool EsInterior(Punto p, Punto vertice1, Punto vertice2)
        {
            if (vertice2.GetXFlotante() > vertice1.GetXFlotante())	//inferior
            {
                if (p.GetYFlotante() >= vertice1.GetYFlotante())
                {
                    return true;
                }
            }
            if (vertice2.GetXFlotante() < vertice1.GetXFlotante())  //superior
            {
                if (p.GetYFlotante() <= vertice1.GetYFlotante())
                {
                    return true;
                }
            }
            if (vertice2.GetYFlotante() > vertice1.GetYFlotante()) //derecha
            {
                if (p.GetXFlotante() <= vertice2.GetXFlotante())
                {
                    return true;
                }
            }
            if (vertice2.GetYFlotante() < vertice1.GetYFlotante()) //izquierda
            {
                if (p.GetXFlotante() >= vertice2.GetXFlotante())
                {
                    return true;
                }
            }
            return false;
        }

        private static Punto Interseccion(Punto s, Punto p, Punto vertice1, Punto vertice2)
        {
            double x, y;
            if (vertice1.GetYFlotante() == vertice2.GetYFlotante()) //Limite vertical
            {
                y = vertice1.GetYFlotante();
                x = s.GetXFlotante() + (vertice1.GetYFlotante() - s.GetYFlotante()) * (p.GetXFlotante() - s.GetXFlotante()) / (p.GetYFlotante() - s.GetYFlotante());
            }
            else //Limite vertical
            {
                x = vertice1.GetXFlotante();
                y = s.GetYFlotante() + (vertice1.GetXFlotante() - s.GetXFlotante()) * (p.GetYFlotante() - s.GetYFlotante()) / (p.GetXFlotante() - s.GetXFlotante());
            }
            return new PuntoFlotante(x, y);
        }

        #endregion
    }
}
