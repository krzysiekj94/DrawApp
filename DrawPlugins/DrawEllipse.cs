using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DrawPlugins
{
    public class DrawEllipse : DrawInterface.IPluginShapes
    {
        private Canvas m_canvasCopy;
        private Color m_colorShapeEllipse;
        private Color m_colorFillingShapeEllipse;
        private int m_styleLine;
        private int m_thicknessLine;
        Point startPointEllipse;
        bool m_bIsCanvasEventInitialized;

        public DrawEllipse()
        {
            InitDefaultValues();
        }

        private void InitDefaultValues()
        {
            m_colorShapeEllipse = Colors.Black;
            m_colorFillingShapeEllipse = Colors.Black;
            m_thicknessLine = 1;
            m_styleLine = 0;
            m_bIsCanvasEventInitialized = false;
        }

        public void Init(Canvas canvas, Color colorShapesBorder, Color colorShapeFill, int thickness, int setStyleLine)
        {
            m_canvasCopy = canvas;
            m_colorShapeEllipse = colorShapesBorder;
            m_colorFillingShapeEllipse = colorShapeFill;
            m_styleLine = setStyleLine;
            m_thicknessLine = thickness;

            if (m_canvasCopy != null && !m_bIsCanvasEventInitialized)
            {
                m_canvasCopy.MouseLeftButtonDown += canvasCopy_MouseLeftButtonDown;
                m_canvasCopy.MouseLeftButtonUp += canvasCopy_MouseLeftButtonUp;
                m_canvasCopy.MouseMove += canvasCopy_MouseMove;
                m_bIsCanvasEventInitialized = true;
            }
        }

        private void canvasCopy_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas)sender;

            if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var elipse = canvas.Children.OfType<System.Windows.Shapes.Ellipse>().LastOrDefault();

                if (elipse != null)
                {
                    var endPoint = e.GetPosition(canvas);
                    var x = Math.Min(endPoint.X, startPointEllipse.X);
                    var y = Math.Min(endPoint.Y, startPointEllipse.Y);

                    elipse.Width = Math.Max(endPoint.X, startPointEllipse.X) - x;
                    elipse.Height = Math.Max(endPoint.Y, startPointEllipse.Y) - y;

                    Canvas.SetLeft(elipse, x);
                    Canvas.SetTop(elipse, y);
                }
            }
        }

        private void canvasCopy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)sender).ReleaseMouseCapture();
        }

        private void canvasCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvasDraw = (Canvas)sender;

            if (canvasDraw.CaptureMouse())
            {
                startPointEllipse = e.GetPosition(canvasDraw);

                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString(m_colorFillingShapeEllipse.ToString());

                var elipse = new System.Windows.Shapes.Ellipse
                {
                    Stroke = new SolidColorBrush(m_colorShapeEllipse),
                    StrokeThickness = m_thicknessLine,
                    Fill = brush
                };

                Canvas.SetTop(elipse, startPointEllipse.Y);
                Canvas.SetLeft(elipse, startPointEllipse.X);

                switch (m_styleLine)
                {
                    case 0:
                        break;
                    case 1:
                        elipse.StrokeDashArray = new DoubleCollection() { 0, 1 };
                        elipse.StrokeDashCap = PenLineCap.Round;
                        break;
                    case 2:
                        elipse.StrokeDashArray = new DoubleCollection() { 5, 5 };
                        break;
                }

                canvasDraw.Children.Add(elipse);
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
            return "Elipsa";
        }
    }
}
