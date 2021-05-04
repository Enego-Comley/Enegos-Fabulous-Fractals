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


        public MainWindow()
        {
            InitializeComponent();
            height = (int)MathF.Pow(factor, heightPower);
            UpdateText();

        }

        public void UpdateText()
        {
            InfoText.Text = "Factor:" + factor.ToString() + " " + "Height:" + height.ToString();
        }

        public void IncreaseHeight(object sender, RoutedEventArgs e)
        {
            heightPower++;
            height = (int)MathF.Pow(factor, heightPower);
            UpdateText();
        }

        public void DecreaseHeight(object sender, RoutedEventArgs e)
        {
            if (heightPower > 0)
            {
                heightPower--;
            height = (int)MathF.Pow(factor, heightPower);
            UpdateText();
            }
            
        }

        public void IncreaseFactor(object sender, RoutedEventArgs e)
        {
            factor++;
            height = factor;
            UpdateText();
        }

        public void DecreaseFactor(object sender, RoutedEventArgs e)
        {

            factor--;
            height = factor;
            UpdateText();
        }
    }
}
