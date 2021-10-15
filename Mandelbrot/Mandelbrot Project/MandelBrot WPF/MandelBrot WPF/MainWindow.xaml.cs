using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;
using System.Drawing;
using Xceed;
using Xceed.Wpf;
using Xceed.Wpf.Toolkit;

using Color = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;


namespace MandelBrot_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        float[] corner1 = {-2, -1.5f};
        float[] corner2 = {1, 1.5f };
        int width;
        int height;
        int mandelbrotMaxIterations;
        Bitmap theImage;
        Color[] mandelbrotColors = {Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black};
        int[,] mandelbrotValues;

        public MainWindow()
        {
            InitializeComponent();
            DateTime startTime = DateTime.Now;
            GenerateMandelbrot();
            float unthreadedTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            startTime = DateTime.Now;
            GenerateMandelbrotMultithread();
            float threadedTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            DebugText.Text = "Multithreading is: " + (unthreadedTime / threadedTime).ToString() + " times faster.";


        }

        public void GenerateMandelbrotButton(object sender, RoutedEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            GenerateMandelbrot();
            float unthreadedTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            startTime = DateTime.Now;
            GenerateMandelbrotMultithread();
            float threadedTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            DebugText.Text = "Multithreading is: " + (unthreadedTime / threadedTime).ToString() + " times faster.";
        }

        

        void GenerateMandelbrot()
        {
            if(!CheckOverallSettings())
            {
                DebugText.Text = "Incorect overall settings input";
                return; 
            }
            if(!Int32.TryParse(mandelbrotIterationsInput.Text,out mandelbrotMaxIterations))
            {
                DebugText.Text = "Incorect mandelbrot settings input";
                return;
            }
            CreateBitmap();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int iterationsCounter = 0;
                    double realC = corner1[0] + i * ((corner2[0] - corner1[0]) / width);
                    double imaginaryC = corner1[1] + ((corner2[1] - corner1[1]) * j / height);
                    double real = realC;
                    double imaginary = imaginaryC;
                    for (; iterationsCounter < mandelbrotMaxIterations && (((real * real) + (imaginary * imaginary)) < 25);iterationsCounter++)
                    {
                        double temp = real;
                        real = real * real - (imaginary * imaginary) + realC;
                        imaginary = 2.0f * temp * imaginary + imaginaryC;
                    }
                    if(iterationsCounter == mandelbrotMaxIterations)
                    {
                        theImage.SetPixel(i, j, mandelbrotColors[mandelbrotColors.Length - 1]);
                    } else
                    {
                        theImage.SetPixel(i, j, mandelbrotColors[iterationsCounter%(mandelbrotColors.Length - 1)]);
                    }
                    



                }
            }
            string tempFileName = System.IO.Path.GetTempFileName();
            theImage.Save(tempFileName);
            FractalImage.Source = new BitmapImage(new Uri(tempFileName));
            DebugText.Text = "Generated Fractal";

        }

        void GenerateMandelbrotMultithread()
        {
            if (!CheckOverallSettings())
            {
                DebugText.Text = "Incorect overall settings input";
                return;
            }
            if (!Int32.TryParse(mandelbrotIterationsInput.Text, out mandelbrotMaxIterations))
            {
                DebugText.Text = "Incorect mandelbrot settings input";
                return;
            }
            CreateBitmap();
            int requiredFullTasks = (width - (width % 500))/500;
            mandelbrotValues = new int[width, height]; 
            Parallel.For(0,requiredFullTasks,(i) => { 
                GenerateMandelbrotRow(i * 500, 500); 
            });
            GenerateMandelbrotRow(requiredFullTasks * 500, width%500);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    if (mandelbrotValues[i,j] == mandelbrotMaxIterations)
                    {
                        theImage.SetPixel(i, j, mandelbrotColors[mandelbrotColors.Length - 1]);
                    }
                    else
                    {
                        theImage.SetPixel(i, j, mandelbrotColors[mandelbrotValues[i, j] % (mandelbrotColors.Length - 1)]);
                    }
                }
            }

            string tempFileName = System.IO.Path.GetTempFileName();
            theImage.Save(tempFileName);
            FractalImage.Source = new BitmapImage(new Uri(tempFileName));
            DebugText.Text = "Generated Fractal";
        }
        

        void GenerateMandelbrotRow(int startX, int rowWidth)
        {
            for(int i = startX; i < startX + rowWidth; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int iterationsCounter = 0;
                    double realC = corner1[0] + i * ((corner2[0] - corner1[0]) / width);
                    double imaginaryC = corner1[1] + ((corner2[1] - corner1[1]) * j / height);
                    double real = realC;
                    double imaginary = imaginaryC;
                    for (; iterationsCounter < mandelbrotMaxIterations && (((real * real) + (imaginary * imaginary)) < 25); iterationsCounter++)
                    {
                        double temp = real;
                        real = real * real - (imaginary * imaginary) + realC;
                        imaginary = 2.0f * temp * imaginary + imaginaryC;
                    }
                    mandelbrotValues[i, j] = iterationsCounter;




                }

            }
            
        }

        public void ColorInput(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            MediaColor newMediaColor = (MediaColor)(e.Source as ColorPicker).SelectedColor;
            int colorNumber = Int32.Parse((e.Source as ColorPicker).Name.Last().ToString());
            Color newColor = Color.FromArgb(newMediaColor.A, newMediaColor.R, newMediaColor.G, newMediaColor.B);
            Color test = mandelbrotColors[0];
            mandelbrotColors[colorNumber] = newColor;
        }

        void CreateBitmap()
        {
            theImage = new Bitmap(width,height);
        }

        bool CheckOverallSettings()
        {
            if(!Int32.TryParse(sizeInputX.Text,out width))
            {
                return false;
            }
            if (!Int32.TryParse(sizeInputY.Text, out height))
            {
                return false;
            }
            if(width * height > 536870912)
            {
                return false;
            }
            if(!float.TryParse(corner1InputR.Text,out corner1[0]))
            {
                return false;
            }
            if (!float.TryParse(corner1InputI.Text, out corner1[1]))
            {
                return false;
            }
            if (!float.TryParse(corner2InputR.Text, out corner2[0]))
            {
                return false;
            }
            if (!float.TryParse(corner2InputI.Text, out corner2[1]))
            {
                return false;
            }

            return true;
        }

        
    }
}
