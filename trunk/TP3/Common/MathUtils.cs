using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils
{
    public class MathUtils
    {

        public static Decimal ValueBetween(Decimal min, Decimal max)
        {
            return (min + ((max - min)/2));
        }

        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

    }
}
