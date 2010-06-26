using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trochita3D.Core
{
    public class BMPUtils
    {
        private static int VALOR_MAXIMO_BYTE = 255;
        private static int CANTIDAD_BYTES_POR_PIXEL = 3;
        private static int PRIMER_BYTE_CON_INFO_DE_COLOR = 55;

        public static byte[] ObtenerBytesDeArchivo(string archivo)
        {
            System.IO.Stream str = new System.IO.FileStream(archivo, System.IO.FileMode.Open);
            System.IO.BinaryReader br = new System.IO.BinaryReader(str);

            byte[] img_bytes = br.ReadBytes((int)br.BaseStream.Length);

            str.Close();

            return img_bytes;
        }

        /// <summary>
        /// Escala el valor de los bytes del rango 0 -> valor_maximo_byte a 0 -> altura maxima
        /// </summary>
        /// <param name="alturaMaxima"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double[] EscalarBytes(int alturaMaxima, byte[] bytes)
        {
            double[] bytesEscalados = new double[bytes.Length];
            
            for (int i = 0; i < bytes.Length; i++)
            {
                bytesEscalados[i] = bytes[i] * ((double)alturaMaxima / (double)VALOR_MAXIMO_BYTE);
            }

            return bytesEscalados;
        }

        /// <summary>
        /// Recibe el array de bytes del archivo bmp y devuelve un array con un byte con un color por pixel
        /// ya que es en escala de grises, sacando los bytes de preambulo del archivo
        /// </summary>
        /// <param name="bytesArchivo"></param>
        /// <returns></returns>
        public static byte[] ObtenerBytesUtilesDelArchivo(byte[] bytesArchivo)
        { 
            byte[] bytesUtiles = new byte[ObtenerCantidadBytesUtiles(bytesArchivo.Length)];

            int k = 0;
            for (int i = PRIMER_BYTE_CON_INFO_DE_COLOR - 1; i < bytesArchivo.Length; i += CANTIDAD_BYTES_POR_PIXEL)
            {
                bytesUtiles[k++] = bytesArchivo[i];
            }

            return bytesUtiles;
        }

        /// <summary>
        /// Devuelve la cantidad de bytes utiles de un archivo dada la cantidad total de bytes
        /// </summary>
        /// <param name="cantidadTotalBytes"></param>
        /// <returns></returns>
        public static int ObtenerCantidadBytesUtiles(int cantidadTotalBytes)
        {
            int cantidadTotalBytesColor = cantidadTotalBytes - (PRIMER_BYTE_CON_INFO_DE_COLOR - 1);
            return cantidadTotalBytesColor / CANTIDAD_BYTES_POR_PIXEL;
        }
    }
}
