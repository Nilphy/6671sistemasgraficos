using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Common.Configuration
{
    public class XMLParamsProvider  : IParamsProvider
    {
        XmlDocument docXML;
        XmlTextWriter xmlWriterFile;
        String fileName;

        /// <summary>
        /// Crea un archivo XML, si existe, lo carga
        /// </summary>
        /// <param name="fileName"></param>
        public XMLParamsProvider(string fileName)
        {
            this.fileName = fileName;
            if(File.Exists(fileName))
            {
                this.LoadXMLDocument(fileName);
            }
            else
            {
                this.CreateXMLDocument(fileName);
            }
        }

       

        /// <summary>
        /// Si el archivo XML no esta creado, lo crea.
        /// </summary>
        /// <param name="fileName"></param>
        private void CreateXMLDocument(string fileName)
        {
           xmlWriterFile = new XmlTextWriter(fileName, UTF8Encoding.UTF8);
           xmlWriterFile.Formatting = Formatting.Indented;
           xmlWriterFile.WriteStartDocument();
           xmlWriterFile.WriteStartElement("params");
           xmlWriterFile.WriteStartElement("param");
           xmlWriterFile.WriteEndElement();
           xmlWriterFile.WriteEndElement();
           xmlWriterFile.Flush();
           xmlWriterFile.Close();
           docXML = new XmlDocument();
           docXML.Load(fileName);
        }
        /// <summary>
        /// Si el archivo XML esta creado, lo carga.
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadXMLDocument(string fileName)
        {
            docXML = new XmlDocument();
            docXML.Load(fileName);
           
        }

        /// <summary>
        ///  Carga en una lista con aquellos elementos con el tag "param" 
        /// </summary>
        /// <returns>retorna la lista</returns>
        private XmlNodeList CargarListaNodos()
        {
            XmlNodeList listParameters =  docXML.GetElementsByTagName("params");
            XmlNodeList listKeyValue = ((XmlElement)listParameters[0]).GetElementsByTagName("param");
            return listKeyValue;
        }

        #region Miembros de IParamsProvider

        public string GetValue(string param)
        {
            XmlNodeList listKeyValue = this.CargarListaNodos();
            string   value = null;
            foreach (XmlElement nodo in listKeyValue)
	        {
        		   if(param == nodo.GetAttribute("name"))
                   {
                       value = nodo.GetAttribute("value");
                       break;
                   }
	        }
            return value;
        } 


            

        public void SetParam(string param, string value)
        {
            string oldValue = this.GetValue(param);
            if(oldValue == null)
            {
                XmlNode newNodo = docXML.CreateNode(XmlNodeType.Element, "param", "");
                ((XmlElement)newNodo).SetAttribute("name", param);
                ((XmlElement)newNodo).SetAttribute("value", value);
                docXML.DocumentElement.AppendChild(newNodo);
                docXML.Save(fileName);    
            }
            else
            {
                XmlNodeList listKeyValue = this.CargarListaNodos();
                foreach (XmlElement nodo in listKeyValue)
	            {
		           if(param == nodo.GetAttribute("name"))
                   {
                       nodo.SetAttribute("value",value);
                       docXML.Save(fileName);
                       break;
                   }
	            }  
            }

        }

        public void Refresh()
        {
           
        }

        #endregion
    }
}