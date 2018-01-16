using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrawPlugins
{
    public class DrawEllipse : DrawInterface.IPluginShapes
    {

        public void Init(Canvas canvas, Color colorShapesBorder, Color colorShapeFill, int thickness, int setStyleLine)
        {
            throw new System.NotImplementedException();
        }

        public void backOperation()
        {
            MessageBox.Show("Usuwam: " + GetName());
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            return "Elipsa";
        }
    }
}
