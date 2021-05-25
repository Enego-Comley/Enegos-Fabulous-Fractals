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
using System.Drawing;
using Color = System.Drawing.Color;

namespace Chaos_Game_WPF_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int width = 1080;
        int height = 1080;
        int numberOfPoints = 20000;
        int[] startingPoint = { 0, 0 };
        int[] whichLastNodes = {0};
        int[] whichLastTwoNodes = { 0 };
        float factor = 0.5f;
        Color backgroudColor;
        Color mainColor;
        bool lastNodeCondition;
        bool lastTwoNodesCondition;
        bool baseOnImage;
        string conditionImageLocation;



        public void UpdateConditions()
        {
            string debugText = "";
            float newFactor;
            if(float.TryParse(factorText.Text,out newFactor))
            {
                factor = newFactor;
            } else
            {
                debugText += "Bad factor input. ";
            }

            int newNumberOfPoints;
            if(int.TryParse(numberOfPointsText.Text,out newNumberOfPoints))
            {
                numberOfPoints = newNumberOfPoints;
            } else
            {
                debugText += "Bad Number of points input. ";
            }
        }

        bool TryParseColor(string hexCode, out Color color)
        {
            color = new Color();

            if (hexCode.Length != 9)
            {
                return false;
            }
            int testHex = 0;
            if (!int.TryParse(hexCode,out testHex ,System.Globalization.NumberStyles.HexNumber))
            {

            } 


            hexCode = hexCode.Substring(1);
            return true;
        }


        public void SetSize(object sender, RoutedEventArgs e)
        {
            string widthAsString = imageWidthText.Text;
            string heightAsString = imageHeightText.Text;
            int tempWidth;
            int tempHeight;
            if(Int32.TryParse(widthAsString,out tempWidth) && Int32.TryParse(heightAsString, out tempHeight))
            {
                width = tempWidth;
                height = tempHeight;
                DebugText.Text = "Updated image size. (" + width.ToString() + "," + height.ToString() + ")";
            } else
            {
                imageWidthText.Text = width.ToString();
                imageHeightText.Text = height.ToString();
                DebugText.Text = "Bad Size input";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
