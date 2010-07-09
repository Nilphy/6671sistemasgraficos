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
    public static class StringUtils
    {
        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}