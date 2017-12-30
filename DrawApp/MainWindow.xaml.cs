using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawApp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stack<Line> operationsStack;
        Point currentPoint;
        private DrawInterface.IMainInterface w;
        private int counter;

        public MainWindow()
        {
            InitializeComponent();
            LoadPlugins();
            operationsStack = new Stack<Line>();
            currentPoint = new Point();
            counter = 0;
        }

        public void LoadPlugins()
        {
            var a = Assembly.LoadFile(@"C:\PROJEKTY\drawapp\DrawPlugins\bin\Debug\DrawPlugins.dll");
            Type selectedType = null;
            foreach (var t in a.GetTypes())
            {
                if (t.IsPublic
                    && t.IsClass
                    && typeof(DrawInterface.IMainInterface).IsAssignableFrom(t))
                {
                    selectedType = t;
                }
            }
            if (selectedType == null)
            {
                throw new Exception();
            }

            var o = Activator.CreateInstance(selectedType);
            w = (DrawInterface.IMainInterface)o;
        }

        private void lineRadioButton_Checked(object sender, RoutedEventArgs e)
        {
             
        }

        private void drawCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (curveRadioButton.IsChecked.Value // rysowanie krzywej przy pomocy ołówka
                && e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(drawCanvas);
            }

            if( lineRadioButton.IsChecked.Value // rysowanie linii na podstawie zaznaczenia 2-óch punktów
                && e.ButtonState == MouseButtonState.Pressed && counter == 0)
            {
                currentPoint = e.GetPosition(drawCanvas);
                counter++;
            }
        }

        private void drawCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if ( curveRadioButton.IsChecked.Value 
                && e.LeftButton == MouseButtonState.Pressed )
            {
                Line line = new Line();
                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(drawCanvas).X;
                line.Y2 = e.GetPosition(drawCanvas).Y;

                currentPoint = e.GetPosition(drawCanvas);

                drawCanvas.Children.Add(line);
                operationsStack.Push(line);
            }
        }

        private void squareRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void elipseRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rectangleRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rectangleRightRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void diamondRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void pentagonRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void curveRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void drawCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lineRadioButton.IsChecked.Value
              && counter > 0 )
            {
                Line line = new Line();
                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(drawCanvas).X;
                line.Y2 = e.GetPosition(drawCanvas).Y;
                drawCanvas.Children.Add(line);
                operationsStack.Push(line);
                counter = 0;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int countElementCanvas = drawCanvas.Children.Count;
            
            if( countElementCanvas < 1 )
            {
                drawCanvas.Children.Clear();
            }
            else
            {
                drawCanvas.Children.Remove(operationsStack.Pop());
            }
        }
    }
}
