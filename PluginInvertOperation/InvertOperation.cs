using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PluginInvertOperation
{
    public class InvertOperation : DrawInterface.IPluginOperations
    {
        bool bIsPolishLanguage;
        public InvertOperation()
        {
            bIsPolishLanguage = false;
        }

        public void Dispose()
        {
        }

        public void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A)
        {
            B = (byte)(255 - B);//(byte)color; //B
            G = (byte)(255 - G);// (byte)color; //G
            R = (byte)(255 - R);//(byte)color; // R
            A = 255; // A
        }

        public string GetName()
        {
            string nameOperation = "";
            if (bIsPolishLanguage)
            {
                nameOperation = "Negatyw";
            }
            else
            {
                nameOperation = "Invert";
            }

            return nameOperation;
        }

        public void setLanguage(string languageString)
        {
            if (languageString == "pl")
            {
                bIsPolishLanguage = true;
            }
        }
    }
}
