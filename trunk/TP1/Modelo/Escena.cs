using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Escena
    {

        private float GRAVEDAD = 9.81f;

        // Flag para indicar si cambia la inclinación del terreno para realizar 
        // las correcciones para la simulación del movimiento físico.
        private bool cambioInclinacion = false;

        #region Propiedades

        public Rueda Rueda { set; get; }

        public Terreno Terreno { set; get; }

        #endregion

        public Escena()
        {
            this.Rueda = new Rueda();
            this.Terreno = new Terreno();
        }

        public void Simular(double t)
        {
            double tiempo = (t / 100d);

            double anguloTerreno = this.Terreno.GetAnguloInclinacion(this.Rueda.Centro.X);

            double aceleracion = GetAceleracion(anguloTerreno);
            double aceleracionX = 0;
            double aceleracionY = 0;

            aceleracionX = aceleracion * Math.Cos(anguloTerreno);
            aceleracionY = aceleracion * Math.Sin(anguloTerreno);

            double velocidadIniX = this.Rueda.VelocidadX;
            double velocidadIniY = this.Rueda.VelocidadY;

            // vfinal = vinicial + a.t
            this.Rueda.VelocidadX += (aceleracionX * tiempo);
            this.Rueda.VelocidadY += (aceleracionY * tiempo);

            // desplazamiento = vinicial.t + a.t^2 / 2
            this.Rueda.Centro.X += ((velocidadIniX) * tiempo) + (0.5 * aceleracionX * Math.Pow(tiempo, 2));
            this.Rueda.Centro.Y += ((velocidadIniY) * tiempo) + (0.5 * aceleracionY * Math.Pow(tiempo, 2));

            cambioInclinacion = (this.Terreno.GetAnguloInclinacion(this.Rueda.Centro.X) != anguloTerreno);

            // Actualiza los datos del cuerpo en caso de que se haya cambiado de pendiente
            ActualizarDatosCuerpo(this.Terreno.GetAnguloInclinacion(this.Rueda.Centro.X));
        }

        #region Métodos Privados

        private double GetAceleracion(double anguloTerreno)
        {
            return (Math.Sin(-anguloTerreno) * GRAVEDAD) / 2d;
        }

        private void ActualizarDatosCuerpo(double anguloTerreno)
        {
            if (this.cambioInclinacion)
            {
                // Calculo el modulo de la velocidad.
                double velocidad = Math.Sqrt(Math.Pow(this.Rueda.VelocidadX, 2) + Math.Pow(this.Rueda.VelocidadY, 2));

                // Direcciono la velocidad paralela al plano.
                if (this.Rueda.VelocidadX > 0)
                {
                    this.Rueda.VelocidadX = velocidad * Math.Cos(anguloTerreno);
                    this.Rueda.VelocidadY = velocidad * Math.Sin(anguloTerreno);
                }
                else
                {
                    this.Rueda.VelocidadX = -1 * velocidad * Math.Cos(anguloTerreno);
                    this.Rueda.VelocidadY = velocidad * Math.Sin(-anguloTerreno);
                }

                // Ubico a la rueda a la altura correcta.
                this.Rueda.Centro.Y = this.Terreno.GetAltura(this.Rueda.Centro.X) + this.Rueda.RadioExterno;
            }
        }

        #endregion
    }
}
