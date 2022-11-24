using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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

namespace Iecc8.UI.Equipment.USS
{
    /// <summary>
    /// Interaction logic for Lamp.xaml
    /// </summary>
    public partial class Lamp : UserControl
    {
        public Lamp()
        {
            InitializeComponent();
        }

        
        private bool LampOn = true;

        public void SetLampState(bool active)
        {
            this.LampOn = active;        
        }

        public void Update()
        {
            string source = LampColorProperty.ToString();
            if (this.LampOn)
            {
                source += "-on";
            }
            else
            {
                source += "-off";
            }

            this.LampImage.Source = (ImageSource)FindResource(resourceKey: source);
        }



        public static DependencyProperty LampColorProperty = 
            DependencyProperty.Register("LampColor", typeof(string), typeof(Lamp),
                new PropertyMetadata("purple"));

        [Category("Lamp"), Description("TEST TEST TEST")]
        public string LampColor
        {
            get
            {
                return (string)GetValue(LampColorProperty);
            }
            set
            {
                SetValue(LampColorProperty, value);
            }
        }



        public static DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(Lamp),
                new PropertyMetadata("purple-on"));

        [Category("Lamp"), Description("The default image source for design time use")]
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(LampColorProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
    }
}
