using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PluginInvertOperation
{
    public class InvertOperation : DrawInterface.IPluginOperations
    {
        Canvas m_canvasCopy;
        ProgressBar m_progressBar;
        public InvertOperation()
        {

        }
        public bool Init(Canvas canvas, ProgressBar progressBar)
        {
            bool bIsInitalized = false;

            m_canvasCopy = canvas;
            m_progressBar = progressBar;

            if( m_canvasCopy != null 
                && m_progressBar != null )
            {
                bIsInitalized = true;
            }

            return bIsInitalized;
        }
        public void Dispose()
        {
            //do nothing
        }

        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface)
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);


            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);

        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }


        public void DoOperation()
        {

            WriteableBitmap writeableBitmap = SaveAsWriteableBitmap(m_canvasCopy);
            BitmapImage image = ConvertWriteableBitmapToBitmapImage(writeableBitmap);
         
            int width = (int)image.Width;
            int height = (int)image.Height;
            WriteableBitmap _bitmap = new WriteableBitmap(image);
            int stride = width * ((_bitmap.Format.BitsPerPixel + 7) / 8);
            int arraySize = stride * height;
            byte[] pixels = new byte[arraySize];
            _bitmap.CopyPixels(pixels, stride, 0);
            int color = 0;
            int j = 0;
            for (int i = 0; i < pixels.Length / 4; ++i)
            {
                color = (pixels[j] + pixels[j + 1] + pixels[j + 2]) / 3;
                pixels[j] = (byte)( i % (pixels[j] + 1 ));//(byte)color; //B
                pixels[j + 1] = (byte)(i % (pixels[j + 1] +1) );// (byte)color; //G
                pixels[j + 2] = 255;//(byte)color; // R
                pixels[j + 3] = 255; // A
                j += 4;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            _bitmap.WritePixels(rect, pixels, stride, 0);

            Image image1 = new Image();
            image1.Source = _bitmap;
            m_canvasCopy.Children.Add(image1);
        }

        public string GetName()
        {
            return "Negatyw";
        }
    }
}
