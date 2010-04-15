using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SistemasGraficos.Entidades
{
    public class Vista
    {
        public IList PoligonosTerreno { set; get; }
        public IList PoligonosRueda { set; get; }

        public Vista()
        {
            this.PoligonosTerreno = new ArrayList();
            this.PoligonosRueda = new ArrayList();
        }


    }
}
