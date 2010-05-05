using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemasGraficos.Entidades
{
    public class CurvaBzier
    {
        // Tienen que venir ordenados!!
        private IList<PuntoFlotante> puntosControl;

        public IList<PuntoFlotante> PuntosControl
        {
            set
            {
                if (value.Count % 4 != 0) throw new InvalidOperationException("Esto es para curvas de segmentos de cuatro puntos");

                this.puntosControl = value;
            }
            get
            {
                return this.puntosControl;
            }
        }
        
        public CurvaBzier(IList<PuntoFlotante> puntosControl)
        {
            this.PuntosControl = puntosControl;
        }

        public double GetAlturaAtU(double u)
        {
            throw new NotImplementedException();
        }

        public IList<PuntoFlotante> GetPuntosDiscretos(double deltaU)
        {
            IList<PuntoFlotante> puntosDiscretizados = new List<PuntoFlotante>();
            
            int numeroSegmento = 1;
            while (numeroSegmento <= this.GetCantidadSegmentos())
            {
                IList<PuntoFlotante> puntosParteCurva = new List<PuntoFlotante>();

                // TODO: Desharcodear el cuatro de acá
                for (int i = 0; i < 4; i++)
                {
                    puntosParteCurva.Add(this.PuntosControl[numeroSegmento - 1 + i]);
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
            if (puntosParteCurva.Count != 4) throw new InvalidOperationException("es una Bziel de 3 puntos de control");

            PuntoFlotante primerSumando = puntosParteCurva[0].MultiplicarEscalar(Math.Pow(1-u, 3));
            PuntoFlotante segundoSumando = puntosParteCurva[1].MultiplicarEscalar(3).MultiplicarEscalar(u).MultiplicarEscalar(Math.Pow(1 - u, 2));
            PuntoFlotante tercerSumando = puntosParteCurva[2].MultiplicarEscalar(3).MultiplicarEscalar(Math.Pow(u, 2)).MultiplicarEscalar(1 - u);
            PuntoFlotante cuartoSumando = puntosParteCurva[3].MultiplicarEscalar(Math.Pow(u, 3));

            return primerSumando.SumarPunto(segundoSumando.SumarPunto(tercerSumando.SumarPunto(cuartoSumando)));
        }

        private int GetCantidadSegmentos()
        {
            return this.PuntosControl.Count / 4;
        }
    }
}
