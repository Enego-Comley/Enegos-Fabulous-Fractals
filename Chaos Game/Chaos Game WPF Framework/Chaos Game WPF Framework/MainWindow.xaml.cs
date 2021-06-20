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
        List<int> whichLastNodes;
        List<int> whichLastTwoNodes;
        float factor = 0.5f;
        Color backgroudColor;
        Color mainColor;
        bool lastNodeCondition;
        bool lastTwoNodesCondition;
        bool baseOnImage;
        string conditionImageLocation;
        char[] hexLetters = {'A','B','C','D','E','F'};

        public MainWindow()
        {
            InitializeComponent();
            whichLastNodes = new List<int>();
            whichLastTwoNodes = new List<int>();
            whichLastTwoNodes.Add(0);
            whichLastTwoNodes.Add(0);
        }

        public void UpdateConditions(object sender, RoutedEventArgs e)
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

            Color newBackgroundColor;
            if(TryParseColor(backgroundColorText.Text,out newBackgroundColor))
            {
                backgroudColor = newBackgroundColor;
            } else
            {
                debugText += "Bad background color. ";
            }

            Color newMainColor;
            if (TryParseColor(mainColorText.Text, out newMainColor))
            {
                mainColor = newMainColor;
            } else
            {
                debugText += "Bad main color. ";
            }

            int newStartPosX;
            int newStartPosY;
            if (int.TryParse(startPointXText.Text,out newStartPosX)&&int.TryParse(startPointYText.Text, out newStartPosY))
            {
                startingPoint = new int[]{newStartPosX, newStartPosY };
            } else
            {
                debugText += "Bad starting position. ";
            }
            if((bool)checkBoxNotLastNode.IsChecked)
            {
                List<int> newWhichLastNodes;
                if (TryParseIntList(whichLastTwoNodesText.Text, out newWhichLastNodes))
                {
                    whichLastTwoNodes = newWhichLastNodes;
                } else
                {
                    debugText += "Bad which last node text. ";
                }
            }
            if ((bool)checkBoxNotLastTwoNodes.IsChecked)
            {
                List<int> newWhichTwoLastNodes;
                if (TryParseIntList(whichLastTwoNodesText.Text, out newWhichTwoLastNodes))
                {
                    whichLastTwoNodes = newWhichTwoLastNodes;
                }
                else
                {
                    debugText += "Bad which last two nodes text. ";
                }
            }
            baseOnImage = (bool)checkBoxBaseOffImage.IsChecked;
            if(debugText == "")
            {
                debugText = "Set config.";
            }
            DebugText.Text = debugText;
        }
        
        bool TryParseIntList(string inputText, out List<int> intList)
        {
            intList = new List<int>();
            int newNumber = 0;
            foreach (string x in whichLastNodesText.Text.Split(','))
            {
                if (int.TryParse(x, out newNumber))
                {
                    intList.Add(newNumber);
                }
                else
                {
                    return false;
                }
            }
            if(intList.Count > 0)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        bool TryParseColor(string hexCode, out Color color)
        {
            color = new Color();

            if (hexCode.Length != 9)
            {
                return false;
            }
            hexCode = hexCode.Substring(1);

            int tempNum = 0;
            int a = 0;
            if (int.TryParse(hexCode[0].ToString(),out tempNum))
            {
                a += 16 * tempNum;
            } else if(hexLetters.Contains(hexLetters[0]))
            {
                a += 16 * (11 + Array.IndexOf(hexLetters, hexCode[0]));   
            } else
            {
                return false;
            }
            if (int.TryParse(hexCode[1].ToString(), out tempNum))
            {
                a +=  tempNum;
            }
            else if (hexLetters.Contains(hexLetters[1]))
            {
                a += (11 + Array.IndexOf(hexLetters, hexCode[1]));
            }
            else
            {
                return false;
            }

            int r = 0;
            if (int.TryParse(hexCode[2].ToString(), out tempNum))
            {
                r += 16 * tempNum;
            }
            else if (hexLetters.Contains(hexLetters[2]))
            {
                r += 16 * (11 + Array.IndexOf(hexLetters, hexCode[2]));
            }
            else
            {
                return false;
            }
            if (int.TryParse(hexCode[3].ToString(), out tempNum))
            {
                r += tempNum;
            }
            else if (hexLetters.Contains(hexLetters[3]))
            {
                r += (11 + Array.IndexOf(hexLetters, hexCode[3]));
            }
            else
            {
                return false;
            }

            int g = 0;
            if (int.TryParse(hexCode[4].ToString(), out tempNum))
            {
                g += 16 * tempNum;
            }
            else if (hexLetters.Contains(hexLetters[4]))
            {
                g += 16 * (11 + Array.IndexOf(hexLetters, hexCode[4]));
            }
            else
            {
                return false;
            }
            if (int.TryParse(hexCode[5].ToString(), out tempNum))
            {
                g += tempNum;
            }
            else if (hexLetters.Contains(hexLetters[5]))
            {
                g += (11 + Array.IndexOf(hexLetters, hexCode[5]));
            }
            else
            {
                return false;
            }

            int b = 0;
            if (int.TryParse(hexCode[6].ToString(), out tempNum))
            {
                b += 16 * tempNum;
            }
            else if (hexLetters.Contains(hexLetters[6]))
            {
                b += 16 * (11 + Array.IndexOf(hexLetters, hexCode[6]));
            }
            else
            {
                return false;
            }
            if (int.TryParse(hexCode[7].ToString(), out tempNum))
            {
                b += tempNum;
            }
            else if (hexLetters.Contains(hexLetters[7]))
            {
                b += (11 + Array.IndexOf(hexLetters, hexCode[7]));
            }
            else
            {
                return false;
            }

            color = Color.FromArgb(a,r,g,b);
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

        
    }
}
