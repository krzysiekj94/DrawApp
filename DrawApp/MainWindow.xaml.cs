using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawApp
{
    public partial class MainWindow : Window
    {
        Stack<UIElement> m_redoStack;

        Dictionary<string, DrawInterface.IPluginShapes> drawPluginsDictionary;
        Dictionary<string, DrawInterface.IPluginOperations> operationsPluginsDictionary;

        List<DrawInterface.IPluginShapes> drawPluginsList;
        List<DrawInterface.IPluginOperations> operationPluginsList;

        DrawInterface.IPluginShapes currentActivePlugin;
        DrawInterface.IPluginOperations currentOperationPlugin;

        public MainWindow()
        {
            InitializeComponent();

            drawPluginsList = PluginsManager.LoadPlugins<DrawInterface.IPluginShapes>().ToList();
            operationPluginsList = PluginsManager.LoadPlugins<DrawInterface.IPluginOperations>().ToList();

            drawPluginsDictionary = new Dictionary<string, DrawInterface.IPluginShapes>();
            operationsPluginsDictionary = new Dictionary<string, DrawInterface.IPluginOperations>();
 
            m_redoStack = new Stack<UIElement>();

            //insert list draw plugins into dictionary
            foreach ( var plugin in drawPluginsList )
            {
                drawPluginsDictionary.Add(plugin.GetName(), plugin);
            }

            //insert list operation plugins into dictionary
            foreach( var plugin in operationPluginsList )
            {
                operationsPluginsDictionary.Add(plugin.GetName(), plugin);
            }

            //add draw plugins into toolbar
            foreach( var plugin in drawPluginsDictionary)
            {   
                Button pluginButton = new Button();
                pluginButton.Tag = plugin.Key;
                pluginButton.Content = plugin.Value.GetName();
                pluginButton.Click += pluginButton_Click;
                shapePlugins.Items.Add(pluginButton);
            }

            //add operation plugins into menu
            foreach( var plugin in operationsPluginsDictionary )
            {
                MenuItem operationDrawMenuItem = new MenuItem();
                operationDrawMenuItem.Header = plugin.Key;
                operationDrawMenuItem.Click += operationsMenuItem_Click;
                operationsMenu.Items.Add(operationDrawMenuItem);
            }
        }

        private void operationsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItemPlugin = sender as MenuItem;
            string key = String.Empty;
            DrawInterface.IPluginOperations pluginOperation = null;

            if( currentOperationPlugin != null )
            {
                currentOperationPlugin.Dispose();
            }

            if( menuItemPlugin != null )
            {
                key = menuItemPlugin.Header.ToString();

                if (operationsPluginsDictionary.ContainsKey(key))
                {
                    pluginOperation = operationsPluginsDictionary[key];
                }

                if (pluginOperation != null)
                {
                    currentOperationPlugin = pluginOperation;
                    if( currentOperationPlugin.Init(drawCanvas, drawProgressBar) )
                    {
                        currentOperationPlugin.DoOperation();
                    }
                }
            }

        }

        public void pluginButton_Click(object sender, EventArgs e)
        {
            Button buttonPlugin = sender as Button;
            string key = String.Empty;
            DrawInterface.IPluginShapes pluginShape = null;

            if( currentActivePlugin != null )
            {
                currentActivePlugin.Dispose();
            }

            if ( buttonPlugin != null )
            {
               key = buttonPlugin.Tag.ToString();

                if (drawPluginsDictionary.ContainsKey(key))
                {
                    pluginShape = drawPluginsDictionary[key];
                }

                if( pluginShape != null )
                {
                    currentActivePlugin = pluginShape;
                    currentActivePlugin.Init( drawCanvas, 
                                              getCurrentColorBorder(),
                                              getCurrentColorShapeFill(),
                                              getCurrentThickness(),
                                              getStyleLine() );
                }
            }
        }

        Color getCurrentColorBorder()
        {
            Color currentColorBorder = Colors.Black;
            currentColorBorder = borderShapeLineColorPicker.SelectedColor.Value;

            return currentColorBorder;
        }

        int getStyleLine()
        {
            int selectedIndexStyleCombobox = 0;

            if( styleLineComboBox.SelectedIndex != -1)
            {
                selectedIndexStyleCombobox = styleLineComboBox.SelectedIndex;
            }

            return selectedIndexStyleCombobox;
        }

        Color getCurrentColorShapeFill()
        {
            Color currentColorShapeFill = Colors.Black;
            currentColorShapeFill = fillShapeColorPicker.SelectedColor.Value;

            return currentColorShapeFill;
        }

        int getCurrentThickness()
        {
            int thickness = 1;

            if( !int.TryParse(thicknessCombobox.Text, out thickness) || thickness < 1 )
            {
                thickness = 1;
            }
            else if( thickness > 10)
            {
                thickness = 10;
            }

            return thickness;
        }

        private void ReInitializeShapeParams()
        {
            if( currentActivePlugin != null )
            {
                currentActivePlugin.Init(drawCanvas,
                                        getCurrentColorBorder(),
                                        getCurrentColorShapeFill(),
                                        getCurrentThickness(),
                                        getStyleLine());
            }
        }

        private void thicknessCombobox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReInitializeShapeParams();
        }
        private void styleLineCombobox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReInitializeShapeParams();
        }

        private void borderShapeLineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ReInitializeShapeParams();
        }

        private void fillShapeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ReInitializeShapeParams();
        }

        private void redoOperation_Click(object sender, RoutedEventArgs e)
        {
            if (currentActivePlugin != null)
            {
                if( m_redoStack.Count > 0 )
                {
                    drawCanvas.Children.Add( m_redoStack.Pop() );
                }
            }
        }
        private void undoOperation_Click(object sender, RoutedEventArgs e)
        {
            if (currentActivePlugin != null)
            {
                int indexLastElement = drawCanvas.Children.Count - 1;

                if (indexLastElement >= 0)
                {
                    m_redoStack.Push(drawCanvas.Children[indexLastElement]);
                    drawCanvas.Children.RemoveAt(indexLastElement);
                }
            }
        }

        private void createNewDraw_Click(object sender, RoutedEventArgs e)
        {
            if( MessageBox.Show("Czy chcesz utworzyć nowy rysunek? Niezapisane zmiany zostaną utracone.", "Tworzenie nowego rysunku", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Question, 
                                MessageBoxResult.No) 
                == MessageBoxResult.Yes )
            {
                drawCanvas.Children.Clear();
            }
        }

        private void openDraw_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "PNG (*.png)|*.png|" +
              "(*.*)|*.*";

            if (op.ShowDialog() == true)
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(op.FileName, UriKind.Relative));
                drawCanvas.Background = brush;
            }
        }

        private void saveDraw_Click(object sender, RoutedEventArgs e)
        {
            /*
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                
                int width = Convert.ToInt32(drawCanvas.Width);
                int height = Convert.ToInt32(drawCanvas.Height);
                BitmapImage bmp = new BitmapImage(width, height);
                drawImage.DrawToBitmap(bmp, new Rectangle(0, 0, width, height);
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                
            }
            */
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
