using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace Common.Utils
{
    /// <summary>
    /// Define métodos para realizar busquedas sobre colecciones.
    /// </summary>
    public static class CollectionUtils
    {

        #region ----------------------------------------------------------------------------------- Public 

        #region ----------------------------------------------------------------------------------- Find's 

        /// <summary>
        /// Busca el elemento indicado en la lista.
        /// </summary>
        /// <param name="lista">Lista sobre la cual se realizará la busqueda.</param>
        /// <param name="value">Valor que se desea buscar en la lista.</param>
        /// <returns>
        ///     Posición en la que se encuentra el valor buscado. En caso de no existir 
        ///     devuelve -1.
        /// </returns>
        public static int Find(IList lista, object value)
        {
            return Find(lista, value, null);
        }

        /// <summary>
        /// Busca el primer elemento en la lista que contenga el valor para la propiedad 
        /// indicada.
        /// </summary>
        /// <param name="lista">Lista sobre la que se buscará el elemento.</param>
        /// <param name="value">Valor de la propiedad.</param>
        /// <param name="property">Propiedad por la que se desea buscar.</param>
        /// <returns>
        ///     La posición del primer elemento que contiene el valor para la propiedad 
        ///     indicada.
        ///     En caso de no encontrar ningún elemento se devuelve -1.
        /// </returns>
        public static int FindFirstByProperty(IList lista, object value, string property)
        {
            return FindFirstByPropertyFromPosition(lista,value,property,0);
        }
        
        /// <summary>
        /// Busca el primer elemento a partir de la posición en la lista que contenga el 
        /// valor para la propiedad indicada.
        /// </summary>
        /// <param name="lista">Lista sobre la que se buscará el elemento.</param>
        /// <param name="value">Valor de la propiedad.</param>
        /// <param name="property">Propiedad por la que se desea buscar.</param>
        /// <param name="pos">Posición a partir de la cual se debe comenzar a buscar en la lista.</param>
        /// <returns>
        ///     La posición del primer elemento que contiene el valor para la propiedad 
        ///     indicada.
        ///     En caso de no encontrar ningún elemento se devuelve -1.
        /// </returns>
        /// <example>Se tiene la clase Auto. Se quiere obtener todos los auto cuya marca
        /// tenga la descripción "Ford".
        /// 
        /// CollectionUtils.FindFirstByPropertyFromPosition(listaAutos,"Ford","Marca.Descripcion",15);</example>
        public static int FindFirstByPropertyFromPosition(IList lista, object value, string property, int pos)
        {
            bool encontrado = false;

            int i;

            if (lista == null || lista.Count == 0) return -1;

            for (i = pos; i < lista.Count; i++)
            {
                object element = lista[i];

                if (GetProperty(element, property).Equals(value))
                {
                    encontrado = true;
                    break;
                }
            }

            return (encontrado) ? i : -1;
        }

        public static bool Contains(string[] array, Object obj)
        {
            bool contains = false;
            int i = 0;

            while(!contains && i < array.Length)
            {
                if (array[i].Equals(obj))
                    contains = true;

                i++;
            }

            return contains;
        }

        /// <summary>
        /// Clona la lista pasada por parametro
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList CloneIList(IList list)
        {
            IList listClone = new ArrayList();

            foreach (Object o in list)
            {
                listClone.Add(o);
            }
            return listClone;
        }

        /// <summary>
        /// Realiza la misma operación que FindFirstByProperty pero devuelve el objeto 
        /// encontrado y no la posición en la lista del mismo.
        /// </summary>
        /// <seealso cref="CollectionUtils.FindFirstByProperty"/>
        /// <returns>
        ///     El primer elemento encontrado para las condiciones indicadas. En caso de 
        ///     no encontrar ninguno, se devuelve su default.
        /// </returns>
        public static T FindFirstByProperty<T>(IList lista, object value, string property)
        {
            int i = CollectionUtils.FindFirstByProperty(lista, value, property);

            return (i >= 0) ? (T)lista[i] : default(T);
        }

        /// <summary>
        /// Realiza la misma operación que FindFirstByPropertyFromPosition pero devuelve
        /// el objeto encontrado y no la posición en la lista del mismo.
        /// </summary>
        /// <seealso cref="CollectionUtils.FindFirstByPropertyFromPosition"/>
        /// <returns>
        ///     El primer elemento encontrado para las condiciones indicadas. En
        ///     caso de no encontrar ninguno, se devuelve su default.
        /// </returns>
        public static T FindFirstByPropertyFromPosition< T >(IList lista, object value, string property, int pos)
        {
            int i = CollectionUtils.FindFirstByPropertyFromPosition(lista,value,property,pos);

            return (i >= 0) ? (T)lista[i] : default(T);
        }

        /// <summary>
        /// Busca el elemento indicado en la lista.
        /// </summary>
        /// <param name="lista">Lista sobre la cual se realizará la busqueda.</param>
        /// <param name="value">Valor que se desea buscar en la lista.</param>
        /// <param name="finder">Función a ser utilizada para buscar sobre la lista.</param>
        /// <returns>
        ///     Posición en la que se encuentra el valor buscado. En caso de no existir
        ///     devuelve -1.
        /// </returns>
        public static int Find(IList lista, object value, IFinder finder)
        {
            int pos = 0;

            foreach (object elemento in lista)
            {
                if (finder != null)
                {
                    if (finder.Find(elemento,value))
                        return pos;
                }
                else if (elemento.Equals(value))
                    return pos;
                pos++;
            }

            return -1;
        }
              

        #endregion

        #region ---------------------------------------------------------------------------------- Order's 

        /// <summary>
        /// Ordena los elementos de la lista devolviendo los mismo de forma ordenada en una nueva lista.
        /// </summary>
        /// <param name="list">Lista a ser ordenada.</param>
        /// <param name="comparator">Comparador de elementos de la lista.</param>
        /// <returns>
        ///     Un <c>ArrayList</c> con los elementos de la lista ordenados.
        /// </returns>
        public static ArrayList Sort(IList list, IComparer comparator)
        {
            ArrayList array = ArrayList.Adapter(list);

            array.Sort(comparator);

            return array;
        }


        public static ArrayList Sort<T>(IList lista, string property) where T : IComparable
        {
            ArrayList array = ArrayList.Adapter(lista);

            array.Sort(new SortByProperty<T>(property));

            return array;
        }

        #endregion

        /// <summary>
        /// Devuelve una lista con la unión de los objetos de ambas, filtrando los repetidos,
        /// estos se agregan sólo una vez.
        /// </summary>
        /// <param name="a">La primer lista, no debe ser null</param>
        /// <param name="b">La segunda lista, no debe ser null</param>
        public static IList Union(IList a, IList b)
        {
            IList listUnion = new ArrayList();

            listUnion = (a == null) ? b : a;

            if (b != null)
            {
                foreach (object obj in b)
                {
                    if (!listUnion.Contains(obj))
                    {
                        listUnion.Add(obj);
                    }
                }
            }
            return listUnion;
        }

        /// <summary>
        /// Filtra los objetos de la lista pasada como parametro, devolviendo una nueva lista con únicamente aquellos 
        /// objetos cuya propiedad especificada sea igual al valor especificado.
        /// </summary>
        /// <param name="list">Lista de objetos sobre los cuales se van a comparar los valores de las propiedades</param>
        /// <param name="propertyList">Nombre de Propiedad o Propiedades separadas por punto (Ejemplo: Prop1.Prop2)</param>
        /// <param name="value">Valor con el cual se quiere comparar la propiedad</param>
        /// <exception cref="ArgumentException">Se lanza cuando propertyList no es válida</exception>
        /// <returns>
        ///     ArrayList con los objetos de la lista original que contengan la propiedad que coincida con el 
        ///     valor especificado
        /// </returns>
        public static IList Select(IList list, string propertyList, object value)
        {
            IList newList = new ArrayList();
            object propertyValue = null;

            try
            {
                foreach (object obj in list)
                {
                    propertyValue = GetProperty(obj, propertyList);

                    // Si el valor de la propiedad coincide, agregarlo a la lista que se va a devolver
                    if (value == null || propertyValue.Equals(value))
                    {
                        newList.Add(obj);
                    }
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException();
            }

            return newList;
        }

        /// <summary>
        /// Filtra los objetos de la lista origen, devolviendo una nueva lista con únicamente aquellos 
        /// objetos que no se encuentren en la lista diferencia.
        /// </summary>
        /// <param name="listOrigin"> Lista de objetos a la cual se le van a restar los objetos de la lista listDif</param>
        /// <param name="listDif">Lista de objetos que van a ser restados de listOrigin</param>
        /// <returns></returns>
        public static IList Subtract(IList listOrigin, IList listDif) 
        {
            if (listDif == null) return listOrigin;

            foreach (object o in listDif) 
            {
                listOrigin.Remove(o);
            }
            return listOrigin;
        }


        /// <summary>
        /// Transforma todos los elementos de una lista según un transformador dado.
        /// </summary>
        /// <param name="list">Lista con los elementos a transformar.</param>
        /// <param name="transformer">Transformador a ser aplicado a cada elemento de la colección.</param>
        /// <returns>
        ///     Lista con los elementos transformados por el transformer indicado.
        /// </returns>
        public static IList Transform(IList list, ITransformer transformer)
        {
            ArrayList transformedList = new ArrayList();

            foreach (object element in list)
            {
                transformedList.Add(transformer.Transform(element));
            }

            return transformedList;
        }
        
        /// <summary>
        /// Devuelve una nueva lista con todos los objetos que son propiedad de la lista y que se especifica en
        /// <code>propertyList</code>
        /// </summary>
        /// <param name="list">Lista de objetos sobre los cuales se van a comparar los valores de las propiedades</param>
        /// <param name="propertyList">Nombre de Propiedad o Propiedades separadas por punto (Ejemplo: Prop1.Prop2)</param>
        /// <exception cref="ArgumentException">Se lanza cuando propertyList no es válida</exception>
        /// <returns>
        ///     ArrayList con los objetos de la lista original de la propiedad <code>propertyList</code>
        /// </returns>
        public static IList Collect(IList list, string propertyList)
        {
            IList newList = new ArrayList();
            object propertyValue = null;

            try
            {
                foreach (object obj in list)
                {
                    propertyValue = GetProperty(obj, propertyList);

                    newList.Add(propertyValue);
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException();
            }

            return newList;   
        }

        /// <summary>
        /// Agrega todos los elementos de la lista origen a la lista destino.
        /// </summary>
        /// <param name="listaOrigen"></param>
        /// <param name="listaDestino"></param>
        public static void AddAllElements(IList listaOrigen, IList listaDestino)
        {
            AddElementsFromIndex(listaOrigen,listaDestino,0);
        }

        /// <summary>
        /// Agrega los elementos de la lista origen a la lista destino. Se debe indicar
        /// el indice a partir del cual se agregarán los elemntos de la lista origen.
        /// </summary>
        /// <remarks>
        /// En caso de indicar un indice mayor al tamaño de la lista origen, no se 
        /// realiza ninguna operación.
        /// 
        /// Los elementos se van agregando al final de la lista destino.
        /// 
        /// Se agragan todos los elementos a partir del indice indicado hasta el final de la
        /// lista origen.
        /// </remarks>
        /// <param name="listaOrigen">Lista que posee los elementos a agragar.</param>
        /// <param name="listaDestino">Lista en la que se insertarán los elementos a copiar.</param>
        /// <param name="index">Indice que indica la posición en la lista origen a partir de
        /// la cual se debe comenzar a copiar los elementos.</param>
        public static void AddElementsFromIndex(IList listaOrigen, IList listaDestino, int index)
        {
            for (int i = index; i < listaOrigen.Count; i++)
            {
                listaDestino.Add(listaOrigen[i]);
            }
        }

        public static IList GetRemoveElements(IList originList, IList list, string propertyName)
         {
            IList removeElements = new ArrayList();
            object propertyValue = null;

            if (originList != null)
            {
                foreach (object obj in originList)
                {
                     // Obtengo el valor de la propertie
                    propertyValue = ReflectionUtils.GetProperty(obj, propertyName);

                    // Si no existia en la lista original lo agrego a la lista de nvos elementos
                    if (FindFirstByProperty(list, propertyValue, propertyName) == -1)
                        removeElements.Add(obj);
                }
            }

            return removeElements;
        }

        public static IList GetNewElements(IList originList, IList list, string propertyName)
        {
            IList newElements = new ArrayList();
            
            object propertyValue = null;

            if (list != null)
            {
                foreach (object obj in list)
                {
                    // Obtengo el valor de la propertie
                    propertyValue = ReflectionUtils.GetProperty(obj, propertyName);

                    // Si no existia en la lista original lo agrego a la lista de nvos elementos
                    if (FindFirstByProperty(originList, propertyValue, propertyName) == -1)
                        newElements.Add(obj);
                }
            }

            return newElements;
        }

        #endregion

        #region --------------------------------------------------------------------------------- Privados 

        /// <summary>
        /// Obtiene el valor de la propiedad indicada.
        /// </summary>
        /// <param name="element">Elemento del cual se desea obtener el valor de una propiedad.</param>
        /// <param name="propertyList">Nombre de Propiedad o Propiedades separadas por punto (Ejemplo: Prop1.Prop2)</param>
        /// <exception cref="ArgumentException">Se lanza cuando propertyList no es válida</exception>
        /// <returns>
        ///     Devuelve el valor que contiene la propiedad indicada.
        /// </returns>
        internal static object GetProperty(object element, string propertyList)
        {
            object value = element;
            object propertyValue = null;

            try
            {
                // Partir el nombre de la propiedad es sus componentes (Ejemplo: property1.property2.property3)
                char[] separator = { '.' };
                string[] properties = propertyList.Split(separator);

                foreach (string property in properties)
                {
                    // Obtener la propiedad del objeto                    
                    propertyValue = GetPropertyInfo(value, property).GetValue(value, null);

                    // Ahora el nuevo elemento es la propiedad
                    value = propertyValue;
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException();
            }

            return value;
        }

        private static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            Type t = obj.GetType();

            // Need to perform reflection in isolation on the object and its supertype due to a problem with Castle Dynamic
            // Proxy and lazy loading
            foreach (PropertyInfo info in t.BaseType.GetProperties())
            {
                if (info.Name == propertyName)
                {
                    return info;
                }
            }

            return t.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        #endregion
    }

    /// <summary>
    /// Interfaz utilizada por CollectionUtils para realizar las busquedas sobre colecciones.
    /// </summary>
    public interface IFinder
    {

        /// <summary>
        /// Compara el elemento buscado (elementToFind) con el elemento a comparar.
        /// </summary>
        /// <remarks>Este método es utilizado para buscar sobre elementos de una colección.
        /// El elemento buscado no es directamente uno sobre la colección sino uno contenido
        /// dentro de cada objeto de la colección (Busqueda profunda).</remarks>
        /// <example>Tengo una colección de caramelos. Busco el caramelo cuya marca es 
        /// "Caramelon".
        /// El método se invocaría de la siguiente manera:
        /// IFinder.Find(caramelo_de_la_coleccion,marca_buscada);</example>
        /// <param name="elementToCompare">Elemento a comparar.</param>
        /// <param name="elementToFind">Elemento buscado.</param>
        /// <returns>
        ///     True si el elemento a comparar contiene el elemento buscado. False en caso contrario.
        /// </returns>
        bool Find(object elementToCompare, object elementToFind);
    }

    /// <summary>
    /// Interfaz utilizada para transformar un objeto en otro.
    /// </summary>
    public interface ITransformer
    {
        /// <summary>
        /// Transforma el elemento indicado.
        /// </summary>
        /// <param name="toTransform">Elemento a transformar.</param>
        /// <returns>
        ///     Un nuevo elemento, producto de la transformación del indicado por parámetro.
        /// </returns>
        object Transform(object toTransform);
    }

    internal class SortByProperty<T> : IComparer where T : IComparable
    {
        private string property;

        public SortByProperty(string property)
        {
            this.property = property;
        }

        public int Compare(object x, object y)
        {
            T objectX = (T)CollectionUtils.GetProperty(x, this.property);
            T objectY = (T)CollectionUtils.GetProperty(y, this.property);
            return objectX.CompareTo(objectY);
        }       
    }    
}
