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

namespace RuleBasedFractalsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int relevantSquares = 3;
        Dictionary<int[],int> rules;
        int height = 1024;
        private int width;
        Bitmap currentBitmap;

        public MainWindow()
        {
            rules = new Dictionary<int[], int>();
            rules.Add(new int[] { 0, 0, 0 }, 0);
            rules.Add(new int[] { 0, 0, 1 }, 1);
            rules.Add(new int[] { 0, 1, 0 }, 1);
            rules.Add(new int[] { 0, 1, 1 }, 1);
            rules.Add(new int[] { 1, 0, 0 }, 1);
            rules.Add(new int[] { 1, 0, 1 }, 0);
            rules.Add(new int[] { 1, 1, 0 }, 0);
            rules.Add(new int[] { 1, 1, 1 }, 0);
            /*
             rules.Add(new int[] { 0, 0, 0 }, 0);
            rules.Add(new int[] { 0, 0, 1 }, 1);
            rules.Add(new int[] { 0, 1, 0 }, 1);
            rules.Add(new int[] { 0, 1, 1 }, 0);
            rules.Add(new int[] { 1, 0, 0 }, 1);
            rules.Add(new int[] { 1, 0, 1 }, 1);
            rules.Add(new int[] { 1, 1, 0 }, 0);
            rules.Add(new int[] { 1, 1, 1 }, 1);
             */

            width = (height - relevantSquares % 2) * 2 + relevantSquares % 2;
            InitializeComponent();
            int[,] array = GenerateFractal();
            TheFractal.Source = GetAsImage(array);
            
        }

        BitmapImage GetAsImage(int[,] input)
        {
            Bitmap myBitmap;
            if(relevantSquares%2 == 1)
            {
                myBitmap = new Bitmap(width, height);
                for (int i = 0; i < height; i++)
                {
                    for(int j = 0; j < width; j++)
                    {
                        if(input[j,i] != 0)
                        {

                            myBitmap.SetPixel(j, i, System.Drawing.Color.Black);

                        }
                        
                    }
                }
            } else
            {
                //Add code for double squares here.
                myBitmap = new Bitmap(width, height);
            }
            currentBitmap = myBitmap;
            string tempFileName = System.IO.Path.GetTempFileName();
            myBitmap.Save(tempFileName);
            Console.WriteLine(tempFileName);
            return new BitmapImage(new Uri(tempFileName));
        }

        int[,] GenerateFractal()
        {
            int[,] output = new int[width,height];
            output[height, 0] = 1;
            for(int i = 1; i < height; i++)
            {
                int maxPad = (relevantSquares - relevantSquares % 2) / 2;
                List<int> padding = new int[maxPad].ToList<int>();
                
                for (int j = 0; j < maxPad; j++)
                {
                    List<int> nonPad = new List<int>();
                    for (int k = 0; k < relevantSquares - padding.Count(); k++)
                    {
                        nonPad.Add(output[j + k - maxPad + padding.Count(),i-1]);
                    }
                    List<int> aboveValues = padding.Concat(nonPad).ToList();
                    
                    output[j, i] = GetTileValue(aboveValues);
                    padding.Remove(0);
                }
                for (int j = maxPad; j < width - maxPad; j++)
                {
                    List<int> aboveValues = new List<int>();
                    for (int k = 0; k < relevantSquares; k++)
                    {
                        aboveValues.Add(output[j + k - maxPad, i - 1]);
                    }
                    output[j, i] = GetTileValue(aboveValues);
                }
                for (int j = width - maxPad; j < width; j++)
                {
                    padding.Add(0);
                    List<int> nonPad = new List<int>();
                    for (int k = 0; k < relevantSquares - padding.Count(); k++)
                    {
                        nonPad.Add(output[j + k - maxPad, i - 1]);
                    }
                    List<int> aboveValues = nonPad.Concat(padding).ToList();
                    output[j, i] = GetTileValue(aboveValues);
                }
            }
            return output;
        }

        int GetTileValue(List<int> aboveValues)
        {
            foreach (int[] key in rules.Keys) 
            {
                bool flag = true; ;
                for (int i = 0; i < aboveValues.Count() && flag; i++)
                {
                    if (key[i] != aboveValues[i])
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    return rules[key];
                }
            }
            throw new NullReferenceException("Above values not in rules");
        }
        
        void DebugArray(int[,] array)
        {
            string outputString = "";
            for (int i = 0; i < height; i++)
            {
                String rowString = "";
                for (int j = 0; j < width; j++)
                {
                    rowString += array[j, i];
                    rowString += " ";
                }
                outputString += rowString;
                outputString += "\n";
            }
            Debug.Text = outputString;
        }




    }
}
