using System;
using System.Collections.Generic;
using System.Text;


namespace Common
{
    /// <summary>
    /// Define un predicado según la condición que se indique. Por default la condición es 
    /// false.
    /// </summary>
    public class Predicate
    {
        protected bool condition;

        public Predicate()
        {
            this.condition = false;
        }

        /// <summary>
        /// Instancia un predicado dada una condición.
        /// </summary>
        /// <param name="condition">Condición del predicado.</param>
        public Predicate(bool condition)
        {
            this.condition = condition;
        }

        #region Métodos

        /// <summary>
        /// Evalua la condición asociada al predicado.
        /// </summary>
        /// <returns>El valor de la condición.</returns>
        public bool Evaluate()
        {
            return this.condition;
        }

        public Predicate And(Predicate predicate)
        {
            return new Predicate(this.condition && predicate.Evaluate());
        }

        public Predicate And(bool condition)
        {
            return new Predicate(this.condition && condition);
        }

        public Predicate Or(Predicate predicate)
        {
            return new Predicate(this.condition || predicate.Evaluate());
        }

        public Predicate Or(bool condition)
        {
            return new Predicate(this.condition || condition);
        }

        public bool Not()
        {
            return !this.condition;
        }

        #endregion

        #region Operadores

        public static bool operator ! (Predicate predicate)
        {
            return !predicate.Evaluate();
        }

        public static Predicate operator + (Predicate predicate1, Predicate predicate2)
        {
            return predicate1.Or(predicate2);
        }

        public static Predicate operator + (Predicate predicate, bool condition)
        {
            return predicate.Or(condition);
        }

        #endregion
    }
}
