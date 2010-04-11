using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelo
{
    public class Escena
    {
        public Rueda Rueda { set; get; }
        public Terreno Terreno { set; get; }

        public Escena()
        {
            this.Rueda = new Rueda();
            this.Terreno = new Terreno();
        }
    }
}
