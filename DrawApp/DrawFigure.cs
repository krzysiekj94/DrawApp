using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrawApp
{
    class DrawFigure : IOperation
    {
        Point _PreviousPoint;
        Point _CurrentPoint;
        FigureTypeEnum _FigureType;

        public DrawFigure( FigureTypeEnum figureType )
        {
            _FigureType = figureType;
        }

        public bool redo()
        {
            throw new NotImplementedException();
        }

        public bool undo()
        {
            throw new NotImplementedException();
        }
    }
}
