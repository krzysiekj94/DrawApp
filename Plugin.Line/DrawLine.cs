using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Line
{
    public class DrawLine : DrawInterface.IPluginShapes
    {
        private Color m_colorShapeLine;
        private Canvas m_canvasCopy;
        private int m_styleLine;
        private int m_thicknessLine;
        bool m_bIsCanvasEventInitialized;

        public DrawLine()
        {
            m_colorShapeLine = Colors.Black;
            m_bIsCanvasEventInitialized = false;
            m_styleLine = 0;
            m_thicknessLine = 1;
        }

        public void Init(   Canvas canvas, 
                            Color colorShapesBorder, 
                            Color colorShapeFill, 
                            int thickness, 
                            int setStyleLine )
        {
            m_canvasCopy = canvas;
            m_colorShapeLine = colorShapesBorder;
            m_thicknessLine = thickness;
            m_styleLine = setStyleLine;

            if ( m_canvasCopy != null && !m_bIsCanvasEventInitialized)
            {
                m_canvasCopy.MouseLeftButtonDown += canvasCopy_MouseLeftButtonDown;
                m_canvasCopy.MouseLeftButtonUp += canvasCopy_MouseLeftButtonUp;
                m_canvasCopy.MouseMove += canvasCopy_MouseMove;
                m_bIsCanvasEventInitialized = true;
            }
        }

        public void Dispose()
        {
         if(m_canvasCopy != null && m_bIsCanvasEventInitialized)
         {
                m_canvasCopy.MouseLeftButtonDown -= canvasCopy_MouseLeftButtonDown;
                m_canvasCopy.MouseLeftButtonUp -= canvasCopy_MouseLeftButtonUp;
                m_canvasCopy.MouseMove -= canvasCopy_MouseMove;
                m_bIsCanvasEventInitialized = false;
          }           
        }

        public string GetName()
        {
            return "Linia";
        }

        private void canvasCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvasDraw = (Canvas)sender;

            if (canvasDraw.CaptureMouse())
            {
                var startPoint = e.GetPosition(canvasDraw);
                var line = new System.Windows.Shapes.Line
                {
                    Stroke = new SolidColorBrush(m_colorShapeLine),
                    StrokeThickness = m_thicknessLine,
                    X1 = startPoint.X,
                    Y1 = startPoint.Y,
                    X2 = startPoint.X,
                    Y2 = startPoint.Y,
                };

                switch(m_styleLine)
                {
                    case 0:
                        break;
                    case 1:
                        line.StrokeDashArray = new DoubleCollection() { 0,1 };
                        line.StrokeDashCap = PenLineCap.Round;
                        break;
                    case 2:
                        line.StrokeDashArray = new DoubleCollection() { 5,5 };
                        break;
                }

                canvasDraw.Children.Add(line);
            }
        }

        private void canvasCopy_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas)sender;

            if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var line = canvas.Children.OfType<System.Windows.Shapes.Line>().LastOrDefault();

                if (line != null)
                {
                    var endPoint = e.GetPosition(canvas);
                    line.X2 = endPoint.X;
                    line.Y2 = endPoint.Y;
                }
            }
        }
        private void canvasCopy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).ReleaseMouseCapture();
        }
    }
}
