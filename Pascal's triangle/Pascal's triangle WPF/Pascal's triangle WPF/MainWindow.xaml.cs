using System;
using System.IO;
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
using System.Drawing;

namespace Pascal_s_triangle_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int height;
        public int heightPower = 1;
        public int factor = 2;
        public int border = 5;
        public int halfSquareSize = 1;
        public Bitmap triangleImage;
        public int[][] pascalModuloValues;


        public MainWindow()
        {
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
            pascalModuloValues = TriangleNumbers();
            DrawImage(pascalModuloValues);
            string tempFileName = System.IO.Path.GetTempFileName();
            triangleImage.Save(tempFileName);
            TheImage.Source = new BitmapImage(new Uri(tempFileName));
            UpdateText();
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

        void CreateNewImage(int imageWidth, int imageHeight)
        {
            triangleImage = new Bitmap(imageWidth, imageHeight);
        }

        void DrawImage(int[][] numberValues)
        {
            int imageSize = triangleImage.Width;
            
            for(int i = 0;i < height; i++)
            {
                for(int j = 0; j <= i; j++)
                {
                    if (numberValues[i][j] != 0)
                    {
                        int imageXPoint = (imageSize/2) - (i + 1) * halfSquareSize + j * 2 * halfSquareSize;
                        for(int x = imageXPoint; x < imageXPoint + 2 * halfSquareSize; x++)
                        {
                            int imageYPoint = border + i * 2 * halfSquareSize;
                            for(int y = imageYPoint; y < imageYPoint + 2 * halfSquareSize; y++)
                            {
                                triangleImage.SetPixel(x, y, System.Drawing.Color.Black);
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
    }
}
