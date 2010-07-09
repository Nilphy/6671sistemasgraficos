using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Configuration
{
    interface IParamsProvider
    {
        /// <summary>
        /// Retorna el valor de la parametro ingresado
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        String GetValue(string param);

        /// <summary>
        /// Setea el parametro ingresado con su respectivo valor
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        void SetParam(string param, string value);
        
        /// <summary>
        /// Actualiza el estado los parametros ingresados
        /// </summary>
        void Refresh();

    }
}
