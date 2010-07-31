using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using System.Drawing.Imaging;
using System.Drawing;

namespace Trochita3D.Core
{
    public class Textura
    {
        private int id;

        public Textura(string fileName, bool repeat)
        {
            Gl.glGenTextures(1, out id);
            Bitmap bmp = new Bitmap(fileName);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, (repeat) ? Gl.GL_REPEAT : Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, (repeat) ? Gl.GL_REPEAT : Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, bmp.Width, bmp.Height, 0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bmpData.Scan0);

            //Unlock the bits.
            bmp.UnlockBits(bmpData);
            //Get rid of the image.
            bmp.Dispose();
        }

        public void Activate()
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id);
        }

    }
}
