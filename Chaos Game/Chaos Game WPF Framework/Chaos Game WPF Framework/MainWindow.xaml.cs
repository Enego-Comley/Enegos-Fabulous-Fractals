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
using System.Diagnostics;
using Color = System.Drawing.Color;
using Microsoft.Win32;

namespace Chaos_Game_WPF_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int width = 1081;
        int height = 1081;
        float[] startingPoint = { 0, 0 };
        List<int> whichLastNodes;
        List<int> whichLastTwoNodes;
        List<int[]> jumpPositions;
        float factor = 0.5f;
        Color backgroudColor;
        Color mainColor;
        bool lastNodeCondition;
        bool lastTwoNodesCondition;
        bool baseOnImage;
        bool colorImage;
        string conditionImageLocation;
        Bitmap theFractal;
        Bitmap baseOffImage;
        Color[] rainbowColors;
        Color baseOffImageBackgroundColor;





        public MainWindow()
        {
            InitializeComponent();
            whichLastNodes = new List<int>();
            whichLastTwoNodes = new List<int>();
            jumpPositions = new List<int[]>();
            //whichLastNodes.Add(0);
            //whichLastTwoNodes.Add(0);
            UpdateConditions(new object(),new RoutedEventArgs());
            UpdatePositionText();
            rainbowColors = new Color[]{Color.Red, Color.Orange, Color.Green, Color.CadetBlue, Color.Indigo, Color.Violet};
        }

        public void SaveImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png file (*.png)|*.png|Jpg file (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                theFractal.Save(saveFileDialog.FileName);
            }
        }
        public void GenerateFractal(object sender, RoutedEventArgs e)
        {
            if(baseOnImage)
            {
                baseOffImageBackgroundColor = baseOffImage.GetPixel(0, 0);
            }
            if (jumpPositions.Count == 0)
            {
                DebugText.Text = "No positions";
                return;
            }
            CreateBitmap();
            List<float[]> pixelsToIterate = new List<float[]>();
            bool[,] pixelMap = new bool[width, height];
            pixelsToIterate.Add(new float[] { startingPoint[0], startingPoint[1], -5, -10 });
            while (pixelsToIterate.Count > 0)
            {
                List<float[]> newPixelsToIterate = new List<float[]>();
                foreach (float[] pixel in pixelsToIterate)
                {
                    for (int i = 0; i < jumpPositions.Count; i++)
                    {
                        int[] jumpPoint = jumpPositions[i];
                        float[] newPixel = new float[] { (pixel[0] + factor * (jumpPoint[0] - pixel[0])), (pixel[1] + factor * (jumpPoint[1] - pixel[1])), i, pixel[2] };
                        int roundedX = (int)Math.Round(newPixel[0]);
                        int roundedY = (int)Math.Round(newPixel[1]);
                        if (!pixelMap[roundedX, roundedY])
                        {
                            if(MeetsConditions(newPixel,pixel[3]))
                            {
                                newPixelsToIterate.Add(newPixel);
                                if(colorImage)
                                {
                                    theFractal.SetPixel(roundedX, roundedY, rainbowColors[i%rainbowColors.Length]);

                                }
                                else
                                {
                                    theFractal.SetPixel(roundedX, roundedY, mainColor);

                                }
                                pixelMap[roundedX, roundedY] = true;
                            }
                        }
                    }
                }
                pixelsToIterate = newPixelsToIterate;
            }
            DebugText.Text = "Generated Fractal";
            string tempFileName = System.IO.Path.GetTempFileName();
            theFractal.Save(tempFileName);
            TheImage.Source = new BitmapImage(new Uri(tempFileName));
        }

        bool MeetsConditions(float[] newPoint,float oldJumpPoint)
        {
            if(lastNodeCondition)
            {
                foreach(int whichLastNode in whichLastNodes)
                {
                    if(newPoint[2] == (newPoint[3] + whichLastNode)%jumpPositions.Count)
                    {
                        return false;
                    }
                }
            }
            if (lastTwoNodesCondition)
            {
                if (newPoint[3] == oldJumpPoint)
                {
                    foreach (int whichLastTwoNode in whichLastTwoNodes)
                    {
                        if (newPoint[2] == (newPoint[3] + whichLastTwoNode) % jumpPositions.Count)
                        {
                            return false;
                        }
                    }
                }
                
            }
            if (baseOnImage)
            {
                if(baseOffImage.GetPixel((int)newPoint[0], (int)newPoint[1]) != baseOffImageBackgroundColor)
                {
                    return false;
                }
            }
            return true;
        }

        void ChooseBaseOffImage(object sender, RoutedEventArgs e)
        {
            if((bool)checkBoxBaseOffImage.IsChecked)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Png file (*.png)|*.png|Jpg file (*.jpg)|*.jpg";
                if(openFileDialog.ShowDialog() == true)
                {
                    baseOffImage = new Bitmap(openFileDialog.FileName);
                    baseOnImage = true;
                } else
                {
                    checkBoxBaseOffImage.IsChecked = false;
                    baseOnImage = false;
                }
            } else
            {
                baseOnImage = false;
            }
        }

        void CreateBitmap()
        {
            theFractal = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    theFractal.SetPixel(i, j, backgroudColor);
                }
            }
        }

        public void AddPositionFromLengthAndAngle(object sender, RoutedEventArgs e)
        {
            float length;
            float angle;
            if (float.TryParse(PositionInputTextX.Text, out length) && float.TryParse(PositionInputTextY.Text, out angle))
            {
                int xPos = (int)(width * 0.5 + Math.Cos(angle * (Math.PI / 180.0f)) * length);
                int yPos = (int)(height * 0.5 + Math.Sin(angle * (Math.PI / 180.0f)) * length);
                if ((xPos >= 0) && (xPos < width))
                {
                    if ((yPos >= 0) && (yPos < height))
                    {
                        jumpPositions.Add(new int[] { (int)xPos, (int)yPos });
                        UpdatePositionText();
                        DebugText.Text = "Added new position";
                    }
                    else
                    {
                        DebugText.Text = "yPos out of range";
                    }
                }
                else
                {
                    DebugText.Text = "xPos out of range";
                }

            }
            else
            {
                DebugText.Text = "Bad position input";
            }

        }

        public void AddPositionFromDecimal(object sender, RoutedEventArgs e)
        {
            float xPos;
            float yPos;
            if (float.TryParse(PositionInputTextX.Text, out xPos) && float.TryParse(PositionInputTextY.Text, out yPos))
            {
                xPos *= width - 1;
                yPos *= height - 1;
                if ((xPos >= 0) && (xPos < width))
                {
                    if ((yPos >= 0) && (yPos < height))
                    {
                        jumpPositions.Add(new int[] { (int)xPos, (int)yPos });
                        UpdatePositionText();
                        DebugText.Text = "Added new position";
                    }
                    else
                    {
                        DebugText.Text = "yPos out of range";
                    }
                }
                else
                {
                    DebugText.Text = "xPos out of range";
                }

            }
            else
            {
                DebugText.Text = "Bad position input";
            }

        }

        public void RemovePosition(object sender, RoutedEventArgs e)
        {
            if(jumpPositions.Count > 0)
            {
                jumpPositions.RemoveAt(jumpPositions.Count - 1);
                UpdatePositionText();
                DebugText.Text = "Added Position";
            }
        }

        public void AddPositionFromCord(object sender, RoutedEventArgs e)
        {
            int xPos;
            int yPos;
            if (int.TryParse(PositionInputTextX.Text, out xPos) && int.TryParse(PositionInputTextY.Text, out yPos))
            {
                if((xPos >=0)&&(xPos < width))
                {
                    if ((yPos >= 0) && (yPos < height))
                    {
                        jumpPositions.Add(new int[] { xPos, yPos });
                        UpdatePositionText();
                        DebugText.Text = "Added new position";
                    }
                    else
                    {
                        DebugText.Text = "yPos out of range";
                    }
                } else
                {
                    DebugText.Text = "xPos out of range";
                }
                
            } else
            {
                DebugText.Text = "Bad position input";
            }

        }

        void UpdatePositionText()
        {
            string newPositionsText = "Positions:";
            foreach(int[] position in jumpPositions)
            {
                newPositionsText += " (" + position[0] + "," + position[1] + ")";
            }
            PositionListText.Text = newPositionsText;
        }

        public void UpdateConditions(object sender, RoutedEventArgs e)
        {
            colorImage = (bool)ColorImageCheckbox.IsChecked;
            string debugText = "";
            float newFactor;
            if(float.TryParse(factorText.Text,out newFactor))
            {
                if((newFactor<1)&&(newFactor>0))
                {
                    factor = newFactor;

                } else
                {
                    debugText += "Factor is not between 1 and 0 ";
                }
            } else
            {
                debugText += "Bad factor input. ";
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
                if((newStartPosX<0)||(newStartPosX>=width))
                {
                    debugText += "X start pos out of range ";
                }
                else if((newStartPosY < 0) || (newStartPosY >= height))
                {
                    debugText += "Y start pos out of range ";
                } else
                    startingPoint = new float[]{newStartPosX, newStartPosY };
            } else
            {
                debugText += "Bad starting position. ";
            }

            lastNodeCondition = (bool)checkBoxNotLastNode.IsChecked;
            if ((bool)checkBoxNotLastNode.IsChecked)
            {
                
                List<int> newWhichLastNodes;
                if (TryParseIntList(whichLastNodesText.Text, out newWhichLastNodes))
                {
                    whichLastNodes = newWhichLastNodes;
                } else
                {
                    debugText += "Bad which last node text. ";
                }
            }

            lastTwoNodesCondition = (bool)checkBoxNotLastTwoNodes.IsChecked;
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
            if(debugText == "")
            {
                debugText = "Set config.";
            }
            DebugText.Text = debugText;
        }
        
        bool TryParseIntList(string inputText, out List<int> intList)
        {
            intList = new List<int>();
            int newNumber;
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

            int a = 0;
            int r = 0;
            int g = 0;
            int b = 0;

            if (Int32.TryParse(hexCode.Substring(0,2),System.Globalization.NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out a))
            {

            } else
            {
                return false;
            }
            if (Int32.TryParse(hexCode.Substring(2,2),System.Globalization.NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out r))
            {

            } else
            {
                return false;
            }
            if (Int32.TryParse(hexCode.Substring(4,2),System.Globalization.NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out g))
            {

            } else
            {
                return false;
            }
            if (Int32.TryParse(hexCode.Substring(6,2),System.Globalization.NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out b))
            {

            } else
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
                width = tempWidth + 1;
                height = tempHeight + 1;
                DebugText.Text = "Updated image size. (" + (width - 1).ToString() + "," + (height - 1).ToString() + ")";
            } else
            {
                imageWidthText.Text = width.ToString();
                imageHeightText.Text = height.ToString();
                DebugText.Text = "Bad Size input";
            }
            jumpPositions = new List<int[]>();
            PositionListText.Text = "Positions:";
        }


    }
}
