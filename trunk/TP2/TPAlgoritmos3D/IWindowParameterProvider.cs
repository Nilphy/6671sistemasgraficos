using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPAlgoritmos3D
{
    public interface IWindowParameterProvider
    {

        int TOP_VIEW_POSX { get; set; }
        int TOP_VIEW_POSY { get; set; }
        int TOP_VIEW_W { get; set; }
        int TOP_VIEW_H { get; set; }
        int HEIGHT_VIEW_POSX { get; set; }
        int HEIGHT_VIEW_POSY { get; set; }
        int HEIGHT_VIEW_W { get; set; }
        int HEIGHT_VIEW_H { get; set; }
        int DL_AXIS { get; set; }
        int DL_GRID { get; set; }
        int DL_AXIS2D_TOP { get; set; }
        int DL_AXIS2D_HEIGHT { get; set; }
        int W_WIDTH { get; set; }
        int W_HEIGHT { get; set; }

    }
}
