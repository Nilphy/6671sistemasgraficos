using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class IndexStrategy
    {

        public IList<int> GetIndexForSecciones(int renderType, IList<Seccion> secciones)
        {
            switch (renderType)
            {
                case Gl.GL_QUADS:
                        return this.GetIndexByQuadStrategy(secciones);
                default:
                        return this.GetIndexByStripStrategy(secciones);
            }
        }

        private IList<int> GetIndexByStripStrategy(IList<Seccion> secciones)
        {
            int indexCount = 0;
            IList<int> indices = new List<int>();
            Seccion seccion;

            for (int i = 0; i < secciones.Count; i++)
            {
                seccion = secciones[i];

                for (int j = 0; j < seccion.Vertices.Count; j++)
                {
                    indices.Add(indexCount);
                    indices.Add((indexCount + seccion.Vertices.Count) % (secciones.Count * seccion.Vertices.Count));
                    indexCount++;
                }
            }

            return indices;
        }

        private IList<int> GetIndexByQuadStrategy(IList<Seccion> secciones)
        {
            int indexCount = 0;
            IList<int> indices = new List<int>();
            Seccion seccion;

            for (int i = 0; i < secciones.Count - 1; i++)
            {
                seccion = secciones[i];

                for (int j = 0; j < seccion.Vertices.Count - 1; j++)
                {
                    indices.Add(indexCount);
                    indices.Add(indexCount + seccion.Vertices.Count);
                    indices.Add(indexCount + 1 + seccion.Vertices.Count);
                    indices.Add(++indexCount);
                }

                indexCount++;
            }

            return indices;
        }

    }
}
