using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Diagnostics;
using Microsoft.Win32;


namespace Pascal_s_triangle_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int height;
        public int heightPower = 2;
        public int factor = 105;
        public int border = 0;
        public int halfSquareSize = 1;
        public Bitmap triangleImage;
        public int[][] pascalModuloValues;
        int processorCount;
        bool varyColor = false;


        public MainWindow()
        {
            processorCount = Environment.ProcessorCount;

            InitializeComponent();
            height = (int)MathF.Pow(factor, heightPower);
            DisplayNewTriangle();


        }

        public void DisplayNewTriangle()
        {
            halfSquareSize = 1;
            if (height * 2 * halfSquareSize < 1000)
            {
                halfSquareSize = 500 / height;
            }
            CreateNewImage(height * 2 * halfSquareSize + 2 * border, height * 2 * halfSquareSize + 2 * border);
            //taskTest();
            //Task t = CalculateTriangleNumbersAsync();
            //t.Wait();
            //ParrallelTriangleNumbers();
            //CalculateTriangleNumbersMultiThread();
            DrawImage(TriangleNumbers());
            string tempFileName = System.IO.Path.GetTempFileName();
            triangleImage.Save(tempFileName);
            TheImage.Source = new BitmapImage(new Uri(tempFileName));
            UpdateText();
        }

        void CalculateTriangleNumbersMultiThread()
        {
            pascalModuloValues = new int[height][];

            int processorCount = Environment.ProcessorCount;


            for (int i = 0; i < (processorCount + 1); i++)
            {
                pascalModuloValues[i] = new int[i + 1];
                pascalModuloValues[i][0] = 1;
                for (int j = 1; j < i; j++)
                {
                    pascalModuloValues[i][j] = (pascalModuloValues[i - 1][j - 1] + pascalModuloValues[i - 1][j]) % factor;
                }
                pascalModuloValues[i][i] = 1;
            }


            for (int i = processorCount + 1; i < height; i++)
            {
                pascalModuloValues[i] = new int[i + 1];
                pascalModuloValues[i][0] = 1;
                List<Thread> threadList = new List<Thread>();
                //int numsPerProccessor = ;
                int fraction = 1 / (processorCount-4);
                for (int k = 0; k < processorCount - 1; k++)
                {
                    
                    threadList.Add(new Thread(() => CalculateNumbersFunction(i, 1 + k * (i + 1) * fraction, 1 + (k + 1) * (i + 1) * fraction)));
                    threadList[k].Start();
                }
                threadList.Add(new Thread(() => CalculateNumbersFunction(i, 1 + ((1/fraction) - 1) * (i + 1) / fraction, i)));
                threadList[processorCount - 1].Start();

                foreach (Thread thread in threadList)
                {
                    thread.Join();
                    

                }
                pascalModuloValues[i][i] = 1;
            }

        }



        int[][] TriangleNumbers()
        {
            int[][] numsArray = new int[height][];
            
            for (int i = 0; i < height; i++)
            {
                numsArray[i] = new int[i + 1];
                numsArray[i][0] = 1;
                for(int j = 1; j < i; j++)
                {
                    numsArray[i][j] = (numsArray[i - 1][j - 1] + numsArray[i - 1][j]) % factor;
                }
                numsArray[i][i] = 1;
            }
            return numsArray;
        }

        async Task CalculateTriangleNumbersAsync()
        { 
            pascalModuloValues = new int[height][];

            int processorCount = Environment.ProcessorCount;


            for (int i = 0; i < (processorCount + 2); i++)
            {
                pascalModuloValues[i] = new int[i + 1];
                pascalModuloValues[i][0] = 1;
                for (int j = 1; j < i; j++)
                {
                    pascalModuloValues[i][j] = (pascalModuloValues[i - 1][j - 1] + pascalModuloValues[i - 1][j]) % factor;
                }
                pascalModuloValues[i][i] = 1;
            }


            for (int i = processorCount + 2; i < height; i++)
            {
                pascalModuloValues[i] = new int[i + 1];
                pascalModuloValues[i][0] = 1;
                List<Task> taskList = new List<Task>();
                //int numsPerProccessor = ;
                for (int k = 0; k < processorCount - 1; k++)
                {
                    taskList.Add(CalculateNumbersTask(i, 1 + k * (i+1)/processorCount, 1 + (k+1) * (i+1)/processorCount));
                }
                taskList.Add(CalculateNumbersTask(i, 1 + (processorCount - 1) * (i + 1) / processorCount, i));
                
                foreach(Task task in taskList)
                {
                    await task;
                    
                }
                pascalModuloValues[i][i] = 1;
            }

        }

        void CalculateNumbersFunction(int i, int startJ, int endJ)
        {

            for (int j = startJ; j < endJ; j++)
            {
                pascalModuloValues[i][j] = ((pascalModuloValues[i - 1][j - 1] + pascalModuloValues[i - 1][j]) % factor);
            }
            
        }

        async Task CalculateNumbersTask(int i, int startJ, int endJ)
        {
            
            for(int j = startJ; j < endJ; j++)
            {
                pascalModuloValues[i][j] = ((pascalModuloValues[i - 1][j - 1] + pascalModuloValues[i - 1][j]) % factor);
            }
            
        }

        void CreateNewImage(int imageWidth, int imageHeight)
        {
            triangleImage = new Bitmap(imageWidth, imageHeight);
        }
        
        int GetColorValue(int number, int[] factors)
        {
            float color = 0;
            for(int i = 0; (i < factors[1]) && (number % factors[0] == 0); i++)
            {
                if (number%factors[0] == 0)
                {
                    color += 255.0f / (1 + factors[1]);
                    number /= factors[0];
                }
            }
            
            return (int)color;
        }

        void DrawImage(int[][] numberValues)
        {
            int imageSize = triangleImage.Width;
            if (varyColor)
            {
                List<int[]> valueFactors = FindFactors();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (numberValues[i][j] != 0)
                        {
                            System.Drawing.Color pixelColor = System.Drawing.Color.Black;
                            int r = 255;
                            int g = 255;
                            int b = 255;
                            if(valueFactors.Count > 0)
                            {
                                b = GetColorValue(numberValues[i][j],valueFactors[0]);
                                g = 0;
;                               r = 0;
                            }
                            if (valueFactors.Count > 1)
                            {
                                g = GetColorValue(numberValues[i][j], valueFactors[1]);
                            }
                            if (valueFactors.Count > 2)
                            {
                                r = GetColorValue(numberValues[i][j], valueFactors[2]);
                            }
                            pixelColor = System.Drawing.Color.FromArgb(255,r,g,b);
                            int imageXPoint = (imageSize / 2) - (i + 1) * halfSquareSize + j * 2 * halfSquareSize;

                            for (int x = imageXPoint; x < imageXPoint + 2 * halfSquareSize; x++)
                            {
                                int imageYPoint = border + i * 2 * halfSquareSize;
                                for (int y = imageYPoint; y < imageYPoint + 2 * halfSquareSize; y++)
                                {
                                    triangleImage.SetPixel(x, y, pixelColor);
                                }
                            }
                        }
                    }
                }
            } else
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (numberValues[i][j] != 0)
                        {

                            int imageXPoint = (imageSize / 2) - (i + 1) * halfSquareSize + j * 2 * halfSquareSize;
                            for (int x = imageXPoint; x < imageXPoint + 2 * halfSquareSize; x++)
                            {
                                int imageYPoint = border + i * 2 * halfSquareSize;
                                for (int y = imageYPoint; y < imageYPoint + 2 * halfSquareSize; y++)
                                {
                                    triangleImage.SetPixel(x, y, System.Drawing.Color.Black);
                                }
                            }
                        }
                    }
                }
            }
            
            
            
        }

        public void UpdateText()
        {
            InfoText.Text = "Factor:" + factor.ToString() + " " + "Height:" + height.ToString();
        }

        public void IncreaseHeight(object sender, RoutedEventArgs e)
        {
            heightPower++;
            height = (int)MathF.Pow(factor, heightPower);
            DisplayNewTriangle();
        }

        public void DecreaseHeight(object sender, RoutedEventArgs e)
        {
            if (heightPower > 0)
            {
                heightPower--;
            height = (int)MathF.Pow(factor, heightPower);
            DisplayNewTriangle();
            }
            
        }

        public void IncreaseFactor(object sender, RoutedEventArgs e)
        {
            factor++;
            height = factor * factor;
            heightPower = 2;
            DisplayNewTriangle();
        }

        public void DecreaseFactor(object sender, RoutedEventArgs e)
        {

            factor--;
            height = factor * factor;
            heightPower = 2;
            DisplayNewTriangle();
        }

        public void SaveImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png file (*.png)|*.png|Jpg file (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                triangleImage.Save(saveFileDialog.FileName);
            }
        }

        public void RevertColourCheckBox(object sender, RoutedEventArgs e)
        {
            varyColor = !varyColor;
            DisplayNewTriangle();
        }

        List<int[]> FindFactors()
        {
            List<int[]> factorsList = new List<int[]>();
            int number = factor;
            for (int b = 2; number > 1; b++)
            {
                if (number % b == 0)
                {
                    int x = 0;
                    while (number % b == 0)
                    {
                        number /= b;
                        x++;
                    }
                    factorsList.Add(new int[]{b,x});
                }
            }
            return factorsList;
        }
        void ParrallelTriangleNumbers()
        {
            pascalModuloValues = new int[height][];
            for (int i = 0; i < height; i++)
            {
                pascalModuloValues[i] = new int[i + 1];
                pascalModuloValues[i][0] = 1;
                Parallel.For(1, i, (j) =>
                {
                    pascalModuloValues[i][j] = (pascalModuloValues[i - 1][j - 1] + pascalModuloValues[i - 1][j]) % factor;
                });

                pascalModuloValues[i][i] = 1;
            }
        }
    }
}
