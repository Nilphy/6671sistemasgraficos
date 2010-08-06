using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public abstract class SuperficieBarrido : Superficie
    {
        // Escalado a ser aplicado a la superficie original.
        protected Punto escaladoOriginal;

        // Traslado a ser aplicado a la superficie original. Las superficies
        // se dibujan por convención centradas en el origen.
        protected Punto trasladoOriginal;

        protected IList<Punto> camino;

        /// <summary>
        /// Carga las secciones que representan la superficie de barrido a partir
        /// de la sección de corte y el camino definido para el barrido.
        /// </summary>
        protected override void LoadSecciones()
        {
            Punto puntoAnterior = camino[camino.Count - 1];
            Punto puntoActual;
            Punto vectorPuntoAnteriorActual;
            Seccion seccion;
            double dx, dy, angulo;

            for (int i = 0; i < camino.Count; i++)
            {
                puntoActual = camino[i];
                vectorPuntoAnteriorActual = puntoActual - puntoAnterior;
                seccion = this.GetSeccion();
                dx = puntoActual.X - puntoAnterior.X;
                dy = puntoActual.Y - puntoAnterior.Y;
                angulo = Math.Atan(dy / dx);

                int cuadrante = vectorPuntoAnteriorActual.GetCuadrante();
                if (cuadrante.Equals(1) || cuadrante.Equals(4))
                    angulo -= Math.PI;

                if (this.HasEscalado()) seccion.Escalar(escaladoOriginal.X, escaladoOriginal.Y, escaladoOriginal.Z);
                if (this.HasTraslado()) seccion.Trasladar(trasladoOriginal.X, trasladoOriginal.Y, trasladoOriginal.Z);
                seccion.Rotar(angulo);
                seccion.Trasladar(puntoActual.X, puntoActual.Y, puntoActual.Z);
                secciones.Add(seccion);

                puntoAnterior = puntoActual;
            }

            // Con las secciones armadas se cargan los buffers de vertices, índices y normales.
            this.BuildSurfaceDataBuffers();
        }

        /// <summary>
        /// Carga la sección a ser utilizada para el barrido.
        /// </summary>
        public abstract Seccion GetSeccion();

        /// <summary>
        /// Carga la lista de puntos correspondientes al camino del barrido.
        /// </summary>
        public virtual void SetCamino(IList<Punto> camino)
        {
            this.camino = camino;
            this.LoadSecciones();
        }

        public virtual void Escalar(double x, double y, double z)
        {
            this.escaladoOriginal = new Punto(x, y, z);
        }

        public virtual void Trasladar(double x, double y, double z)
        {
            this.trasladoOriginal = new Punto(x, y, z);
        }

        protected virtual bool HasEscalado()
        {
            return this.escaladoOriginal != null;
        }

        protected virtual bool HasTraslado()
        {
            return this.trasladoOriginal != null;
        }

    }
}
