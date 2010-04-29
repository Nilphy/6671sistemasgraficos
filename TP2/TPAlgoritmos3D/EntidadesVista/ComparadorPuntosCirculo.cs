using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SistemasGraficos.Entidades
{
    public class ComparadorPuntosCirculo : IComparer
    {
        #region IComparer Members

        public int Compare(object p1, object p2)
        {
            if (p1 is Punto && p2 is Punto)
            {
                Punto punto1 = (Punto)p1;
                Punto punto2 = (Punto)p2;

                if (punto1.GetXFlotante() == punto2.GetXFlotante() && 
                    punto1.GetYFlotante() == punto2.GetYFlotante()) // son iguales
                    return 0;

                if (punto1.GetCuadrante() == punto2.GetCuadrante()) // Están en el mismo cuadrante
                {
                    int cuadrante = punto1.GetCuadrante();

                    if (cuadrante == 1 || cuadrante == 2)
                    {
                        if (punto1.GetXFlotante() > punto2.GetXFlotante())
                            return -1;
                        else
                            return 1;
                    }
                    else if (cuadrante == 3 || cuadrante == 4)
                    {
                        if (punto1.GetXFlotante() > punto2.GetXFlotante())
                            return 1;
                        else
                            return -1;
                    }
                }
                else // no están en el mismo cuadrante
                {
                    if (punto1.GetCuadrante() < punto2.GetCuadrante())
                        return -1;
                    else
                        return 1;
                }
            }

            throw new InvalidOperationException("No se pueden comparar objetos distintos");
        }

        #endregion
    }
}
