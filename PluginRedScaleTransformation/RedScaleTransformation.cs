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

namespace PluginRedScaleTransformation
{
    public class RedScaleTransformation : DrawInterface.IPluginOperations
    {
        bool bIsPolishLanguage;
        public RedScaleTransformation()
        {
            bIsPolishLanguage = false;
        }
        public void Dispose()
        {
        }

        public void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A)
        {
            B = 0; //B
            G = 0; //G
            //pixels[j + 2] = 0; //R
            A = 255; //A
        }

        public string GetName()
        {
            string nameOperation = "";
            if (bIsPolishLanguage)
            {
                nameOperation = "Odcień czerwieni";
            }
            else
            {
                nameOperation = "Red filter";
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
