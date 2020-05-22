using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mandelbrot
{
    public static class Texture2dFactory
    {
        public static int Create(string filePath, bool flipImage = true)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found at " + filePath);
            }

            var textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            var bmp = CreateBitmap(filePath, flipImage);

            SendBitmapDataToOpenGLTexture(bmp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            return textureId;
        }

        private static Bitmap CreateBitmap(string filePath, bool flipImage)
        {
            var bmp = new Bitmap(filePath);

            if (flipImage)
            {
                bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            }

            return bmp;
        }

        private static void SendBitmapDataToOpenGLTexture(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            GL.TexImage2D(
                TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                bmpData.Width, bmpData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0
            );

            bmp.UnlockBits(bmpData);
        }
    }
}
