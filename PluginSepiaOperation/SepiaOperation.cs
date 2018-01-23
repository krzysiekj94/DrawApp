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

namespace PluginSepiaOperation
{
    public class SepiaOperation : DrawInterface.IPluginOperations
    {
        bool bIsPolishLanguage;
        public SepiaOperation()
        {
            bIsPolishLanguage = false;
        }
        public void Dispose()
        {
        }

        public void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A)
        {
            byte B1 = B;
            byte G1 = G;
            byte R1 = R;

            double tr = 0.393 * R1 + 0.769 * G1 + 0.189 * B1;
            double tg = 0.349 * R1 + 0.686 * G1 + 0.168 * B1;
            double tb = 0.272 * R1 + 0.534 * G1 + 0.131 * B1;
            double ta = 255;

            A = (byte)ta;

            if (tr > 255)
            {
                R = 255;
            }
            else
            {
                R = (byte)tr;
            }

            if (tg > 255)
            {
                G = 255;
            }
            else
            {
                G = (byte)tg;
            }

            if (tb > 255)
            {
                B = 255;
            }
            else
            {
                B = (byte)tb;
            }
        }

        public string GetName()
        {
            string nameOperation = "";
            if (bIsPolishLanguage)
            {
                nameOperation = "Sepia";
            }
            else
            {
                nameOperation = "Sepia";
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
