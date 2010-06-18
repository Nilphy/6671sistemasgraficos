using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Trochita3D.Core;

namespace Trochita3D.Curvas
{
    public class CurvaBzierSegmentosCubicos
    {
        // Esto no se puede cambiar si el tipo de curva sigue siendo de segmentos cúbicos
        public static int CANTIDAD_PUNTOS_SEGMENTO = 4;

        // Tienen que venir ordenados!!
        private IList<PuntoFlotante> puntosControl;

        public IList<PuntoFlotante> PuntosControl
        {
            set
            {
                this.puntosControl = this.CompletarPuntos(value);
            }
            get
            {
                return this.puntosControl;
            }
        }

        /// <summary>
        /// Si vienen PO P1 P2 P3 P4 P5 se completa así:
        /// 
        ///     P0 P1 P2 P3
        ///     P3 P4 P5 P5
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private IList<PuntoFlotante> CompletarPuntos(IList<PuntoFlotante> puntos)
        {
            IList<PuntoFlotante> retorno = new List<PuntoFlotante>();
            
            int numeroSegmento = 1;
            int cantidadSegmentos = puntos.Count / 4;
            while (numeroSegmento <= cantidadSegmentos)
            {
                int indiceInicioSegmento = ((numeroSegmento - 1) * CANTIDAD_PUNTOS_SEGMENTO - 1) > 0 ? (numeroSegmento - 1) * CANTIDAD_PUNTOS_SEGMENTO - 1 : 0;

                for (int i = indiceInicioSegmento; i < CANTIDAD_PUNTOS_SEGMENTO + indiceInicioSegmento; i++)
                {
                    retorno.Add(puntos[i]);
                }

                numeroSegmento++;
            }

            if (puntos.Count % CANTIDAD_PUNTOS_SEGMENTO != 0)
            {
                int cantidadPuntos = puntos.Count;
                int cantidadTramosCuatroPuntos = cantidadPuntos / CANTIDAD_PUNTOS_SEGMENTO;
                int cantidadPuntosAgregar = (cantidadTramosCuatroPuntos + 1) * CANTIDAD_PUNTOS_SEGMENTO - cantidadPuntos;

                // Se agregan los últimos puntos que no llegaron a completar un segmento de 4
                int indiceUltimoPuntoAgregado = ((cantidadTramosCuatroPuntos * CANTIDAD_PUNTOS_SEGMENTO - 1) < 0) ? 0 : cantidadTramosCuatroPuntos * CANTIDAD_PUNTOS_SEGMENTO - 1;
                for (int i = indiceUltimoPuntoAgregado; i < cantidadPuntos; i++)
                {
                    retorno.Add(puntos[i]);
                }

                // Se repite el último punto hasta que se complete el segmento de cuatro
                for (int i = 0; i < cantidadPuntosAgregar; i++)
                {
                    retorno.Add(retorno[retorno.Count - 1]);
                }
            }

            return retorno;
        }

        public CurvaBzierSegmentosCubicos(IList<PuntoFlotante> puntosControl)
        {
            this.PuntosControl = puntosControl;
        }

        public IList<PuntoFlotante> GetPuntosDiscretos(double deltaU)
        {
            IList<PuntoFlotante> puntosDiscretizados = new List<PuntoFlotante>();
            
            int numeroSegmento = 1;
            while (numeroSegmento <= this.GetCantidadSegmentos())
            {
                IList<PuntoFlotante> puntosParteCurva = new List<PuntoFlotante>();
               
                for (int i = 0; i < CANTIDAD_PUNTOS_SEGMENTO; i++)
                {
                    puntosParteCurva.Add(this.PuntosControl[(numeroSegmento - 1) * CANTIDAD_PUNTOS_SEGMENTO + i]);
                }

                for (double u = 0; u <= 1; u += deltaU)
                {
                    puntosDiscretizados.Add(this.GetValorEnSegmento(puntosParteCurva, u));
                }

                numeroSegmento++;
            }

            return puntosDiscretizados;
        }

        /// <summary>
        /// Devuelve el valor de la curva Bziel cuyo polígono de control está formado
        /// por los trés puntos pasados por parámetro, para el parámetro u e (0,1)
        /// 
        /// Formula sacada de la wikipedia 
        /// B(t) = p0*((1-t)^3) + 3*P1*t*((1-t)^2) + 3*p2*(t^2)*(1-t) + p3*t^3
        /// http://es.wikipedia.org/wiki/Curva_de_B%C3%A9zier
        /// </summary>       
        private PuntoFlotante GetValorEnSegmento(IList<PuntoFlotante> puntosParteCurva, double u)
        {
            if (puntosParteCurva.Count != CANTIDAD_PUNTOS_SEGMENTO) throw new InvalidOperationException("es una Bziel de " + CANTIDAD_PUNTOS_SEGMENTO + " puntos de control");

            PuntoFlotante primerSumando = puntosParteCurva[0].MultiplicarEscalar(Math.Pow(1-u, 3));
            PuntoFlotante segundoSumando = puntosParteCurva[1].MultiplicarEscalar(3).MultiplicarEscalar(u).MultiplicarEscalar(Math.Pow(1 - u, 2));
            PuntoFlotante tercerSumando = puntosParteCurva[2].MultiplicarEscalar(3).MultiplicarEscalar(Math.Pow(u, 2)).MultiplicarEscalar(1 - u);
            PuntoFlotante cuartoSumando = puntosParteCurva[3].MultiplicarEscalar(Math.Pow(u, 3));

            return primerSumando.SumarPunto(segundoSumando.SumarPunto(tercerSumando.SumarPunto(cuartoSumando)));
        }

        private int GetCantidadSegmentos()
        {
            return this.PuntosControl.Count / CANTIDAD_PUNTOS_SEGMENTO;
        }
    }
}
