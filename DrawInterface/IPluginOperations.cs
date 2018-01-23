using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DrawInterface
{
    public interface IPluginOperations : IDisposable
    {
        void SetPixel(ref byte B, ref byte G, ref byte R, ref byte A);
        string GetName();
        void setLanguage(string languageString);
    }
}
