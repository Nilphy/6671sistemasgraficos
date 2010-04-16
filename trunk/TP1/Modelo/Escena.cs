using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Escena
    {

        private float GRAVEDAD = 9.81f;

        public const double X_MIN = 0;
        public const double Y_MIN = 0;
        public const double X_MAX = 800;
        public const double Y_MAX = 600;

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

            // vfinal = vinicial + a.t
            this.Rueda.VelocidadX += (aceleracionX * tiempo);
            this.Rueda.VelocidadY += (aceleracionY * tiempo);

            // desplazamiento = vinicial.t + a.t^2 / 2
            this.Rueda.Centro.X += ((this.Rueda.VelocidadX) * tiempo) + (0.5 * aceleracionX * Math.Pow(tiempo, 2));
            this.Rueda.Centro.Y += ((this.Rueda.VelocidadY) * tiempo) + (0.5 * aceleracionY * Math.Pow(tiempo, 2));

            cambioInclinacion = (this.Terreno.GetAnguloInclinacion(this.Rueda.Centro.X) != anguloTerreno);

            // Actualiza los datos del cuerpo en caso de que se haya cambiado de pendiente
            ActualizarDatosCuerpo(tiempo, this.Terreno.GetAnguloInclinacion(this.Rueda.Centro.X));

            // TODO: Probar esto!!! No guarda el signo del angulo en caso de ser negativo!!!
            this.Rueda.AnguloRotacion += this.Rueda.SentidoX * this.Rueda.VelocidadAngular * tiempo;
            this.Rueda.AnguloRotacion %= (2 * Math.PI);
        }

        #region Métodos Privados

        private double GetAceleracion(double anguloTerreno)
        {
            return (Math.Sin(-anguloTerreno) * GRAVEDAD) / (1 + (Math.Pow(Rueda.RadioExterno, 2) / 4d));
        }

        private void ActualizarDatosCuerpo(double tiempo, double anguloTerreno)
        {
            if (this.cambioInclinacion)
            {
                int sentido = this.Rueda.SentidoX;

                // Calculo el modulo de la velocidad.
                double velocidad = Math.Sqrt(Math.Pow(this.Rueda.VelocidadX, 2) + Math.Pow(this.Rueda.VelocidadY, 2));

                // Direcciono la velocidad paralela al plano.
                this.Rueda.VelocidadX = sentido * velocidad * Math.Cos(anguloTerreno);
                this.Rueda.VelocidadY = velocidad * Math.Sin(sentido * anguloTerreno);

                // Ubico a la rueda a la altura correcta.
                this.Rueda.Centro.Y = this.Terreno.GetAltura(this.Rueda.Centro.X) + this.Rueda.RadioExterno;
            }
        }

        #endregion
    }
}
