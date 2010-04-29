using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Common;

namespace Common
{
    public class ConvertUtil
    {
        public static string[] ConvertToStringArray(IList objectList, string propertyName)
        {
            string[] result = new string[objectList.Count];
            IEnumerator it = objectList.GetEnumerator();
            for (int i = 0; it.MoveNext(); i++)
            {
                object entity = it.Current;
                result[i] = ReflectionUtils.GetProperty(entity, propertyName).ToString();
            }

            return result;
        }
    }
}
