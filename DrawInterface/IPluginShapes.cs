using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DrawInterface
{
    public interface IPluginShapes : IDisposable
    {
        void Init(  Canvas canvas, 
                    Color colorShapesBorder, 
                    Color colorShapeFill, 
                    int thickness, 
                    int setStyleLine    );
        string GetName();
    }
}
