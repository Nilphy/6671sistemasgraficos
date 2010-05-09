﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class CurvaBsplineSegmentosCubicos
    {
        // Esto no se puede cambiar si el tipo de curva sigue siendo de segmentos cúbicos
        public static int CANTIDAD_PUNTOS_SEGMENTO = 4;

        // Tienen que venir ordenados!! y con el del medio unido
        private IList<PuntoFlotante> puntosControl;

        // Acá se completan los puntos, usar siempre la propiedad!!!
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
        
        public CurvaBsplineSegmentosCubicos(IList<PuntoFlotante> puntosControl)
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
        /// Devuelve el valor de la curva Bspline cuyo polígono de control está formado
        /// por los cuatro puntos pasados por parámetro, para el parámetro u e (0,1)
        /// 
        /// Formula sacada de apuntes de la práctica
        /// C(u) = b-2 * P0 + b-1 * P1 + B0 * P2 + B1 *P3
        /// b-2 = 1/6 * (1 - 3*u - 3*u^2 - u^3)
        /// ... me aburrí
        /// </summary>       
        private PuntoFlotante GetValorEnSegmento(IList<PuntoFlotante> puntosParteCurva, double u)
        {
            if (puntosParteCurva.Count != CANTIDAD_PUNTOS_SEGMENTO) throw new InvalidOperationException("es una Bziel de " + CANTIDAD_PUNTOS_SEGMENTO + " puntos de control");

            double baseMenosDos = 1d / 6d * (1d - 3d * u + 3 * Math.Pow(u, 2d) - Math.Pow(u, 3d));
            double baseMenosUno = 1d / 6d * (4d - 6d * Math.Pow(u, 2d) + 3d * Math.Pow(u, 3d));
            double baseCero = 1d / 6d * (1d + 3d * u + 3d * Math.Pow(u, 2) - 3 * Math.Pow(u, 3d));
            double baseUno = 1d / 6d * Math.Pow(u, 3d);

            return puntosParteCurva[0].MultiplicarEscalar(baseMenosDos).SumarPunto(puntosParteCurva[1].MultiplicarEscalar(baseMenosUno).SumarPunto(puntosParteCurva[2].MultiplicarEscalar(baseCero).SumarPunto(puntosParteCurva[3].MultiplicarEscalar(baseUno))));
        }

        /// <summary>
        /// Es la cantidad de puntos
        /// </summary>
        /// <returns></returns>
        private int GetCantidadSegmentos()
        {
            return this.PuntosControl.Count / 4;
        }

        /// <summary>
        /// Si vienen P0 P1 P2 P3 P4 P5 P6 P7 P8
        /// la lista de puntos queda:
        /// P0 P1 P2 P3
        /// P1 P2 P3 P4
        /// P2 P3 P4 P5
        /// P3 P4 P5 P6
        /// P4 P5 P6 P7
        /// P5 P6 P7 P8
        /// P6 P7 P8 P0
        /// P7 P8 P0 P1
        /// P8 P0 P1 P2
        /// </summary>
        /// <param name="puntos"></param>
        /// <returns></returns>
        private IList<PuntoFlotante> CompletarPuntos(IList<PuntoFlotante> puntos)
        {
            IList<PuntoFlotante> retorno = new List<PuntoFlotante>();

            int cantidadSegmentos = puntos.Count;
            int numeroSegmento = 1;

            // Para que la curva termine con los puntos que empezó
            puntos.Add(puntos[0]);
            puntos.Add(puntos[1]);
            puntos.Add(puntos[2]);

            // Y ahora el efecto escalera :S
            int indiceInicioSegmento = 0;
            while (indiceInicioSegmento < cantidadSegmentos)
            {
                for (int i = indiceInicioSegmento; i < indiceInicioSegmento + CANTIDAD_PUNTOS_SEGMENTO; i++)
                {
                    retorno.Add(puntos[i]);
                }

                indiceInicioSegmento++;
            }

            return retorno;
        }
    }
}
