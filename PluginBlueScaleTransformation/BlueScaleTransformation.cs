using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PluginBlueScaleTransformation
{
    public class BlueScaleTransformation : DrawInterface.IPluginOperations
    {
        public void Dispose()
        {
        }

        public void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A)
        {
            //pixels[j] = 0; //B
            G = 0;   //pixels[j + 1] = 0; //G
            R = 0;   //R
            A = 255; //A
        }

        public string GetName()
        {
            return "Odcień niebieski";
        }
    }
}
