using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trochita3D.Core;
using System.Windows.Forms;

namespace Trochita3D
{
    public class Controlador
    {
        
        public Escena Escena { get; set; }
        
        public bool view_grid = true;
        public bool view_axis = true;

        public Controlador()
        {
            this.Escena = new Escena();
            this.Escena.Camara = new CamaraAerea();
        }

        public void PonerCamaraAerea()
        {
            this.Escena.Camara = new CamaraAerea();
        }

        public void PonerCamaraTerreno()
        {
            this.Escena.Camara = new CamaraTerreno();
        }

        public void PonerCamaraLocomotora()
        {
            this.Escena.Camara = new CamaraLocomotora();
        }
    }
}
