using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Web;

namespace Common.Utils
{
    /// <summary>
    /// Define métodos para realizar busquedas sobre colecciones.
    /// </summary>
    public static class XlsUtils
    {
        public static void Exportar(string nombreArchivo, IList titulos, IList propiedades, IList elementos, HttpResponse response, string tituloPrincipal)
        {
            string pathArchivo = ConfigurationManager.AppSettings["PATH_TEMP"] + nombreArchivo;

            FileInfo archivo = new FileInfo(pathArchivo);
            StreamWriter handlerArchivo = archivo.CreateText();

            handlerArchivo.WriteLine(tituloPrincipal);

            // Escribir encabezado
            string linea = "";

            foreach (string titulo in titulos)
            {
                linea += "=CONCATENAR(\"\";\"" + titulo + "\";\"\")\t";
            }

            handlerArchivo.WriteLine(linea);



            // Escribir contenido
            foreach (object elemento in elementos)
            {
                linea = "";

                foreach (string propiedad in propiedades)
                {
                    linea += "=CONCATENAR(\"\";\"" + ReflectionUtils.GetProperty(elemento, propiedad) + "\";\"\")\t";
                }

                handlerArchivo.WriteLine(linea);
            }


            // Genero la salida
            handlerArchivo.Close();
            response.AppendHeader("content-disposition", "attachment; filename=" + nombreArchivo);
            response.ContentType = "application / msexcel";
            response.WriteFile(pathArchivo);
            response.End();
        }
    }
}