using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrawApp
{
    public partial class MainWindow : Window
    {
        Stack<UIElement> m_redoStack;
        Dictionary<string, DrawInterface.IPluginShapes> drawPlugins;
        List<DrawInterface.IPluginShapes> drawPluginsList;
        DrawInterface.IPluginShapes currentActivePlugin;

        public MainWindow()
        {
            InitializeComponent();
            drawPluginsList = PluginsManager.LoadPlugins<DrawInterface.IPluginShapes>().ToList();
            drawPlugins = new Dictionary<string, DrawInterface.IPluginShapes>();
            m_redoStack = new Stack<UIElement>();

            foreach ( var x in drawPluginsList )
            {
                drawPlugins.Add(x.GetName(), x);
            }

            foreach ( var drawPlugin in drawPlugins)
            {   
                Button pluginButton = new Button();
                pluginButton.Tag = drawPlugin.Key;
                pluginButton.Content = drawPlugin.Value.GetName();
                pluginButton.Click += pluginButton_Click;
                shapePlugins.Items.Add(pluginButton);
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

                if (drawPlugins.ContainsKey(key))
                {
                    pluginShape = drawPlugins[key];
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
    }
}
