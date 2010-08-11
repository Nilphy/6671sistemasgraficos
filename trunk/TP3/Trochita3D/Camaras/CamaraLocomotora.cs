using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Common.Utils;
using Tao.OpenGl;

namespace Trochita3D.Core
{
    public class CamaraLocomotora : Camara
    {
        public CamaraLocomotora()
        {
            this.Eye = new Punto(10, 10, 10);
            this.At = new Punto(11, 11, 11);
            this.Up = new Punto(0, 0, 1);
        }

        public override void UpdateCameraByMouse(bool mousing)
        {
        }

        public override void RotateCamera(double h, double v)
        {
        }

        public override void MoveCamera(float d)
        {
        }

        public override void SlideCamera(float h, float v)
        {
        }
    }
}
