using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Plugin.Rectangle
{
    public class DrawRectangle : DrawInterface.IPluginShapes
    {
        private Canvas m_canvasCopy;
        private Color m_colorShapeLine;
        private Color m_colorFillingShapeRect;
        private Point startPointRect;
        private int m_thicknessLine;
        private int m_styleLine;
        bool m_bIsCanvasEventInitialized;

        public DrawRectangle()
        {
            InitDefaultValues();
        }

        private void InitDefaultValues()
        {
            m_colorShapeLine = Colors.Black;
            m_colorFillingShapeRect = Colors.Black;
            m_thicknessLine = 1;
            m_styleLine = 0;
            m_bIsCanvasEventInitialized = false;
        }

        public void Init( Canvas canvas, 
                          Color colorShapesBorder, 
                          Color colorShapeFill, 
                          int thickness, 
                          int setStyleLine)
        {
            m_canvasCopy = canvas;
            m_colorShapeLine = colorShapesBorder;
            m_colorFillingShapeRect = colorShapeFill;
            m_thicknessLine = thickness;
            m_styleLine = setStyleLine;

            if (m_canvasCopy != null && !m_bIsCanvasEventInitialized)
            {
                m_canvasCopy.MouseLeftButtonDown += canvasCopy_MouseLeftButtonDown;
                m_canvasCopy.MouseLeftButtonUp += canvasCopy_MouseLeftButtonUp;
                m_canvasCopy.MouseMove += canvasCopy_MouseMove;
                m_bIsCanvasEventInitialized = true;
            }
        }
        private void canvasCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvasDraw = (Canvas)sender;

            if (canvasDraw.CaptureMouse())
            {
                startPointRect = e.GetPosition(canvasDraw);

                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString(m_colorFillingShapeRect.ToString());

                var rectangle = new System.Windows.Shapes.Rectangle
                {
                    Stroke = new SolidColorBrush(m_colorShapeLine),
                    StrokeThickness = m_thicknessLine,
                    Fill = brush
                };

                Canvas.SetTop(rectangle, startPointRect.Y);
                Canvas.SetLeft(rectangle, startPointRect.X);

                switch (m_styleLine)
                {
                    case 0:
                        break;
                    case 1:
                        rectangle.StrokeDashArray = new DoubleCollection() { 0, 1 };
                        rectangle.StrokeDashCap = PenLineCap.Round;
                        break;
                    case 2:
                        rectangle.StrokeDashArray = new DoubleCollection() { 5, 5 };
                        break;
                }

                canvasDraw.Children.Add(rectangle);
            }
        }
        private void canvasCopy_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas)sender;

            if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var rect = canvas.Children.OfType<System.Windows.Shapes.Rectangle>().LastOrDefault();

                if (rect != null)
                {
                    var endPoint = e.GetPosition(canvas);
                    var x = Math.Min(endPoint.X, startPointRect.X);
                    var y = Math.Min(endPoint.Y, startPointRect.Y);

                    rect.Width = Math.Max(endPoint.X, startPointRect.X) - x;
                    rect.Height = Math.Max(endPoint.Y, startPointRect.Y) - y;

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                }
            }
        }
        private void canvasCopy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).ReleaseMouseCapture();
        }
        public void backOperation()
        {
            int indexLastElement = m_canvasCopy.Children.Count - 1;

            if (indexLastElement >= 0)
            {
                m_canvasCopy.Children.RemoveAt(indexLastElement);
            }
        }

        public void Dispose()
        {
            if (m_canvasCopy != null && m_bIsCanvasEventInitialized)
            {
                m_canvasCopy.MouseLeftButtonDown -= canvasCopy_MouseLeftButtonDown;
                m_canvasCopy.MouseLeftButtonUp -= canvasCopy_MouseLeftButtonUp;
                m_canvasCopy.MouseMove -= canvasCopy_MouseMove;
                m_bIsCanvasEventInitialized = false;
            }
        }

        public string GetName()
        {
            return "Prostokąt";
        }
    }
}
