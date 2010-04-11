using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;


namespace Common.Configuration
{
    /// <summary>
    /// Clase proveedora de parametros de configuracion
    /// Permite crear y obtener de el parametros de la forma clave/valor.
    /// </summary>
    public class PropertiesParamsProvider : IParamsProvider
    {
        private string fileName;

        private Hashtable mapaKeyValue = new Hashtable();

        /// <summary>
        /// Constuctor. Abre o creo el archivo.
        /// </summary>
        /// <param name="fileName"></param>
        public PropertiesParamsProvider(string fileName)
        {
            this.CreatePropertiesFile(fileName);
        }

        /// <summary>
        ///  Crea o abre un archivo. Si existe, guarda sus elementos clave/valor en un hashtable
        /// </summary>
        /// <param name="fileName"></param>
        public void CreatePropertiesFile(string fileName)
        {
            this.fileName = fileName;
            this.Refresh();
        }

          
        /// <summary>
        /// escribe una nueva linea en el archivo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>         
        private void WriteNewLine(string param, string value)
        {
            FileStream fileProperties = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter writer = new StreamWriter(fileProperties);
            writer.WriteLine(param + "=" + value);
            this.CloseWriterFile(fileProperties, writer);
        }

        /// <summary>
        /// rescribe la linea de un archivo en otro archivo temporal
        /// </summary>
        /// <param name="line"></param>
        private void ReWriteLine(string line)
        {
            FileStream fileProperties = new FileStream(fileName + ".tmp", FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter writer = new StreamWriter(fileProperties);
            writer.WriteLine(line);
            this.CloseWriterFile(fileProperties, writer);
        }

        /// <summary>
        /// edita el valor de un parametro y guarda la linea en un archivo temporal
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        private void EditLine(string param, string value)
        {
            FileStream fileProperties = new FileStream(fileName + ".tmp", FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter writer = new StreamWriter(fileProperties);
            writer.WriteLine(param + "=" + value);
            this.CloseWriterFile(fileProperties, writer);
        }

        /// <summary>
        /// cierra un archivo abierto para lectura
        /// </summary>
        /// <param name="file"></param>
        /// <param name="reader"></param>
        private void CloseReaderFile(FileStream file, StreamReader reader)
        {
            reader.Close();
            file.Close(); 
        }

        /// <summary>
        /// cierra una archivo abierto para escritura
        /// </summary>
        /// <param name="file"></param>
        /// <param name="writer"></param>
        private void CloseWriterFile(FileStream file, StreamWriter writer)
        {
            writer.Close();
            file.Close();
        }

        #region Miembros de IParamsProvider


        public string GetValue(string param)
        {
            if(this.mapaKeyValue.ContainsKey(param))
            {
                return this.mapaKeyValue[param].ToString();
            }
            else
            {
                return null;
            }
        }

        public void SetParam(string param, string value)
        {
            string oldValue = this.GetValue(param);
            if(oldValue == null)
            {
                this.WriteNewLine(param, value);
                this.mapaKeyValue.Add(param, value);
            }
            else
            {
                this.mapaKeyValue[param] = value;
                string oldLine = param + "=" + oldValue; 
                string line;

                FileStream fileProperties = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(fileProperties);                
                while(reader.Peek() > -1)
                {
                    line = reader.ReadLine();
                    if(line == oldLine)
                    {
                        this.EditLine(param, value);
                    }
                    else
                    {
                        this.ReWriteLine(line);
                    } 
                }
                this.CloseReaderFile(fileProperties, reader); 
                File.Delete(fileName);
                File.Move(fileName + ".tmp", fileName);
            }
            
        }

        public void Refresh()
        {
            FileStream fileProperties = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite); 

            string  lineKeyValue;
            char[] splitter  = {'='};
            string[] lineInfo = new string[3];
            StreamReader reader = new StreamReader(fileProperties);
            while(reader.Peek() > -1)
            {
                lineKeyValue = reader.ReadLine();
                if(lineKeyValue != "")
                {
                  lineInfo = lineKeyValue.Split(splitter);
                  this.mapaKeyValue.Add(lineInfo[0], lineInfo[1]);  
                }                
            }

            this.CloseReaderFile(fileProperties, reader);           
        } 

        #endregion
    }
}
