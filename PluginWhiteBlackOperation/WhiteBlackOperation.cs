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

namespace PluginWhiteBlackOperation
{
    public class WhiteBlackOperation : DrawInterface.IPluginOperations
    {
        bool bIsPolishLanguage;

        public WhiteBlackOperation()
        {
            bIsPolishLanguage = false;
        }

        public void setLanguage( string languageString )
        {
            if(languageString == "pl")
            {
                bIsPolishLanguage = true;
            }
        }

        public void Dispose()
        {
        }

        public void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A)
        {
            byte B1 = B;
            byte G1 = G;
            byte R1 = R;
            byte A1 = A;

            A = 255;

            if (((R1 + G1 + B1) / 3) >= 127)
            {
                R = G = B = 255;
            }
            else
            {
                R = G = B = 0;
            }
        }

        public string GetName()
        {
            string nameOperation = "";
            if( bIsPolishLanguage )
            {
                nameOperation = "Czarno-biały";
            }
            else
            {
                nameOperation = "Black-white";
            }

            return nameOperation;
        }
    }
}
