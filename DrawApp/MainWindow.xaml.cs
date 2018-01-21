using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawApp
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker backgroundWorker;

        private Stack<UIElement> m_redoStack;

        private Dictionary<string, DrawInterface.IPluginShapes> drawPluginsDictionary;
        private Dictionary<string, DrawInterface.IPluginOperations> operationsPluginsDictionary;

        private List<DrawInterface.IPluginShapes> drawPluginsList;
        private List<DrawInterface.IPluginOperations> operationPluginsList;

        private DrawInterface.IPluginShapes currentActivePlugin;
        private DrawInterface.IPluginOperations currentOperationPlugin;

        WriteableBitmap m_writeableBitmap;
        BitmapImage m_image;

        public MainWindow()
        {
            CultureResources.ChangeCulture(new CultureInfo("pl"));
            InitializeComponent();

            InitializeBackGroundWorker();

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

        private void InitializeBackGroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += background_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += background_RunWorkerCompleted;
            backgroundWorker.DoWork += background_DoWork;
        }

        private void background_DoWork(object sender, DoWorkEventArgs e)
        { 
            if ( currentOperationPlugin != null )
            {
                DoOperation(e);
            }
        }

        private delegate void writeableBitmapDelegate( BitmapImage bitmapImage, Int32Rect rect, byte[] pixels, int stride);

        private void addOperationDelegateToCanvas(BitmapImage bitmapImage, Int32Rect rect, byte[] pixels, int stride)
        {
            WriteableBitmap wb = new WriteableBitmap(bitmapImage);
            wb.WritePixels(rect, pixels, stride, 0);

            Image image1 = new Image();
            image1.Source = wb;
            drawCanvas.Children.Add(image1);
        }

        public void DoOperation(DoWorkEventArgs e)
        {
            int width = (int)m_image.Width;
            int height = (int)m_image.Height;
            WriteableBitmap _bitmap123 = new WriteableBitmap(m_image);
            int stride = width * ((_bitmap123.Format.BitsPerPixel + 7) / 8);
            int arraySize = stride * height;
            byte[] pixels = new byte[arraySize];
            _bitmap123.CopyPixels(pixels, stride, 0);
            int color = 0;
            int j = 0;

            int amountPixels = pixels.Length / 4;

            for (int i = 0; i < amountPixels; ++i)
            {
                color = (pixels[j] + pixels[j + 1] + pixels[j + 2]) / 3;

                currentOperationPlugin.SetPixel(ref pixels[j], ref pixels[j + 1], ref pixels[j + 2], ref pixels[j + 3]);

                var progressProcent = (1000 * i) / amountPixels;
                backgroundWorker.ReportProgress(Convert.ToInt32(progressProcent));
                
                j += 4;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            _bitmap123.WritePixels(rect, pixels, stride, 0);

            writeableBitmapDelegate delegatBitmap = new writeableBitmapDelegate(addOperationDelegateToCanvas);
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, delegatBitmap, m_image, rect, pixels, stride);
        }

        private RenderTargetBitmap GetRenderedTargetBitmap()
        {
            Rect rect = new Rect(drawCanvas.RenderSize);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
              (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(drawCanvas);
            return rtb;
        }

        private void background_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //statusLabel.Content = "Waiting...";
            UpdateButtonState(true);

        }

        private void UpdateButtonState( bool enabled ) 
        {
            //menuItemFile.IsEnabled = enabled; 
            //toolChooseGrid.IsEnabled = enabled;
            //drawCanvas.IsEnabled = enabled;
        }

        private void background_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //statusLabel.Content = "Count " + e.ProgressPercentage;
            drawProgressBar.Value = e.ProgressPercentage;
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
                    UpdateButtonState( false );
                    m_writeableBitmap = SaveAsWriteableBitmap(drawCanvas);
                    m_image = ConvertWriteableBitmapToBitmapImage(m_writeableBitmap);
                    if (backgroundWorker.IsBusy == false)
                    {
                        backgroundWorker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Program wykonuje operację. Proszę poczekać!");
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
            if (currentActivePlugin != null
                || currentOperationPlugin != null)
            {
                if( m_redoStack.Count > 0 )
                {
                    drawCanvas.Children.Add( m_redoStack.Pop() );
                }
            }
        }
        private void undoOperation_Click(object sender, RoutedEventArgs e)
        {
            if (currentActivePlugin != null 
                || currentOperationPlugin != null )
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a picture";
            openFileDialog.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "PNG (*.png)|*.png|" +
              "(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                LoadBitmapToCanvas(openFileDialog.FileName);
            }
        }

        private void LoadBitmapToCanvas( string fileNameString )
        {
            BitmapImage image = new BitmapImage(new Uri(fileNameString, UriKind.Relative));

            int width = (int)image.Width;
            int height = (int)image.Height;
            WriteableBitmap _bitmap = new WriteableBitmap(image);
            int stride = width * ((_bitmap.Format.BitsPerPixel + 7) / 8);
            int arraySize = stride * height;
            byte[] pixels = new byte[arraySize];
            _bitmap.CopyPixels(pixels, stride, 0);

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            _bitmap.WritePixels(rect, pixels, stride, 0);

            Image image1 = new Image();
            image1.Source = _bitmap;
            drawCanvas.Children.Add(image1);
        }

        private void saveDraw_Click( object sender, RoutedEventArgs e )
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select a picture";
            saveFileDialog.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "PNG (*.png)|*.png|" +
              "(*.*)|*.*";

            if ( saveFileDialog.ShowDialog() == true )
            {
                WriteImageFromCanvasToFile(saveFileDialog.FileName ); 
            }     
        }

        private void WriteImageFromCanvasToFile( string filePathString )
        {
            WriteableBitmap writeableBitmap = SaveAsWriteableBitmap(drawCanvas);
            BitmapImage image = ConvertWriteableBitmapToBitmapImage(writeableBitmap);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePathString, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface)
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            //surface.Measure(size);
            //surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);

        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage( WriteableBitmap wbm )
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void setPolishLang_Click(object sender, RoutedEventArgs e)
        {
            CultureResources.ChangeCulture(new CultureInfo("pl"));
        }

        private void setEnglishLang_Click(object sender, RoutedEventArgs e)
        {
            CultureResources.ChangeCulture(new CultureInfo("en"));
        }
    }
}
