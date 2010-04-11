using System;
using System.Collections;
using System.Text;


namespace Common.Utils
{
    /// <summary>
    /// Provee funcionalidades básicas sobre administración y operaciones entre fechas.
    /// </summary>
    public class DateUtils
    {

        /// <summary>
        /// Obtiene la hora y fecha actual.
        /// </summary>
        /// <returns>La hora y fecha actual</returns>
        public static DateTime Now()
        {
            return DateTime.Now;
        }

        /// <summary>
        ///  Convierte la fecha a string y la devuelve en formato corto: dd/MM/yyyy
        /// </summary>a
        public static string DateShortString(DateTime? date)
        {
            return date.GetValueOrDefault().ToShortDateString();
        }

        /// <summary>
        /// Crea una lista con ListElements que representan los meses del calendario.
        /// </summary>
        /// <remarks>Los elementos de la lista son ListElement para permitir "bindiear" dicha
        /// lista con una DropDownList por ejemplo.</remarks>
        /// <seealso cref="Common.ListElement"/>
        /// <returns>Lista con los meses del año.</returns>
        public static IList Months()
        {
            ArrayList mesesList = new ArrayList();

            String[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            for (int i = 0; i <= 11; i++)
            {
                mesesList.Add(new ListElement(i + 1, meses[i]));
            }

            return mesesList;
        }

        /// <summary>
        /// Obtiene la fecha intermedia entre las dos indicadas.
        /// </summary>
        /// <remarks>
        /// En caso de que uno de los parámetros sea nulo, se devuelve aquel que contenga un valor. En caso
        /// de que ambos parámetros sean nulos devuelve nulo.
        /// </remarks>
        /// <see cref="DateBetween"/>
        /// <param name="initDate">Fecha inicial o desde</param>
        /// <param name="finalDate">Fecha final o hasta</param>
        /// <exception cref="ArgumentException">
        /// En caso de que la fecha inicial sea mayor a la final, lanza una ArgumentException.
        /// </exception>
        /// <returns>
        /// Devuelve la fecha intermedia entre la fecha inicial y final.
        /// </returns>
        public static Nullable<DateTime> DateBetween(Nullable<DateTime> initDate, Nullable<DateTime> finalDate)
        {
            if (initDate.HasValue && finalDate.HasValue)
            {
                return DateBetween(initDate.Value, finalDate.Value);
            }

            return (initDate.HasValue) ? initDate : finalDate;
        }

        /// <summary>
        /// Obtiene la fecha intermedia entre las dos indicadas.
        /// </summary>
        /// <param name="initDate">Fecha inicial o desde</param>
        /// <param name="finalDate">Fecha final o hasta</param>
        /// <exception cref="ArgumentException">
        /// En caso de que la fecha inicial sea mayor a la final, lanza una ArgumentException.
        /// </exception>
        /// <returns>
        /// Devuelve la fecha intermedia entre la fecha inicial y final.
        /// </returns>
        public static DateTime DateBetween(DateTime initDate, DateTime finalDate)
        {
            if (initDate > finalDate)
            {
                throw new ArgumentException("Fecha inicial mayor a la fecha final", "initDate");
            }
            TimeSpan dias = (finalDate.Subtract(initDate));

            return initDate.AddDays((dias.Days) / 2);
        }

        public static Boolean IsBetween(DateTime fechaMitad, DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaMitad.CompareTo(fechaInicio) >= 0 && fechaMitad.CompareTo(fechaFin) <= 0)
                return true;

            return false;
        }

        public static Nullable<DateTime> GetDateValueFrom(string text)
        {
            Nullable<DateTime> date = null;

            if (text != null)
            {
                if (!text.Trim().Equals(""))
                {
                    date = Convert.ToDateTime(text);
                }                
            }
            return date;
        }

        public static DateTime? ConvertToDateTime(String date)
        {            
            try
            {
                DateTime? dateTime = (DateTime?)GetDateValueFrom(date);
                return dateTime;
            }
            catch (Exception) 
            {
                return null;
            }            
        }

    }

    /// <summary>
    /// Representa un elemeto de una lista cualquiera.
    /// </summary>
    public class ListElement
    {
        private int id;
        private string descripcion;

        public ListElement(int id, string descripcion)
        {
            this.id = id;
            this.descripcion = descripcion;
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Descripcion
        {
            get { return this.descripcion; }
            set { this.descripcion = value; }
        }
    }
}
