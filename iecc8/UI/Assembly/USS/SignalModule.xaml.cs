﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Iecc8.UI.Equipment.USS
{
    /// <summary>
    /// Interaction logic for SignalModule.xaml
    /// </summary>
    public partial class SignalModule : UserControl
    {
        public SignalModule()
        {
            InitializeComponent();
        }

        public int LeverState
        {
            get
            {
                return (int)GetValue(LeverStateProperty);
            }

            set
            {
                if (value < 0 || value > 2) return; //only allow positions 0-2
                SetValue(LeverStateProperty, value);
            }
        }

        [Category("USS"), Description("Gets or sets the lever position (left 0)")]
        public static readonly DependencyProperty LeverStateProperty =
            DependencyProperty.Register(nameof(LeverState), typeof(int), typeof(SignalModule), new PropertyMetadata(0));

        public bool LeftLampState
        {
            get
            {
                return (bool)GetValue(LeftLampStateProperty);
            }
            set
            {
                SetValue(LeftLampStateProperty, value);
            }
        }

        [Category("USS"), Description("Gets or sets the state of the left lamp")]
        public static readonly DependencyProperty LeftLampStateProperty =
            DependencyProperty.Register(nameof(LeftLampState), typeof(bool), typeof(SignalModule), new PropertyMetadata(false));

        public bool CenterLampState
        {
            get
            {
                return (bool)GetValue(CenterLampStateProperty);
            }
            set
            {
                SetValue(CenterLampStateProperty, value);
            }
        }

        [Category("USS"), Description("Gets or sets the state of the Center lamp")]
        public static readonly DependencyProperty CenterLampStateProperty =
            DependencyProperty.Register(nameof(CenterLampState), typeof(bool), typeof(SignalModule), new PropertyMetadata(false));

        public bool RightLampState
        {
            get
            {
                return (bool)GetValue(RightLampStateProperty);
            }
            set
            {
                SetValue(RightLampStateProperty, value);
            }
        }

        [Category("USS"), Description("Gets or sets the state of the left lamp")]
        public static readonly DependencyProperty RightLampStateProperty =
            DependencyProperty.Register(nameof(RightLampState), typeof(bool), typeof(SignalModule), new PropertyMetadata(false));



        public int PlateNumber
        {
            get
            {
                return (int)GetValue(PlateNumberProperty);
            }
            set
            {
                SetValue(PlateNumberProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the number on the lever plate")]
        public static readonly DependencyProperty PlateNumberProperty =
            DependencyProperty.Register(nameof(PlateNumber), typeof(int), typeof(SignalModule), new PropertyMetadata(99));



        public string LeftLampColor
        {
            get
            {
                return (string)GetValue(LeftLampColorProperty);
            }
            set
            {
                SetValue(LeftLampColorProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the color of the left lamp")]
        public static readonly DependencyProperty LeftLampColorProperty =
            DependencyProperty.Register(nameof(LeftLampColor), typeof(string), typeof(SignalModule), new PropertyMetadata("green"));

        public string CenterLampColor
        {
            get
            {
                return (string)GetValue(CenterLampColorProperty);
            }
            set
            {
                SetValue(CenterLampColorProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the color of the Center lamp")]
        public static readonly DependencyProperty CenterLampColorProperty =
            DependencyProperty.Register(nameof(CenterLampColor), typeof(string), typeof(SignalModule), new PropertyMetadata("red"));

        public string RightLampColor
        {
            get
            {
                return (string)GetValue(RightLampColorProperty);
            }
            set
            {
                SetValue(RightLampColorProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the color of the right lamp")]
        public static readonly DependencyProperty RightLampColorProperty =
            DependencyProperty.Register(nameof(RightLampColor), typeof(string), typeof(SignalModule), new PropertyMetadata("green"));

        public string LeftSignalStr
        {
            get
            {
                return (string)GetValue(LeftSignalStrProperty);
            }
            set
            {
                SetValue(LeftSignalStrProperty, value);
            }
        }
        [Category("USS"), Description("\"SUBAREA/SIGNALNUM[,SUBAREA/SIGNALNUM]...\"")]
        public static readonly DependencyProperty LeftSignalStrProperty =
            DependencyProperty.Register(nameof(LeftSignalStr), typeof(string), typeof(SignalModule), new PropertyMetadata(""));

        public string RightSignalStr
        {
            get
            {
                return (string)GetValue(RightSignalStrProperty);
            }
            set
            {
                SetValue(RightSignalStrProperty, value);
            }
        }
        [Category("USS"), Description("\"SUBAREA/SIGNALNUM\"")]
        public static readonly DependencyProperty RightSignalStrProperty =
            DependencyProperty.Register(nameof(RightSignalStr), typeof(string), typeof(SignalModule), new PropertyMetadata(""));






        private void UpdateLamps()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            

            string lkey = "uss-lamp-";
            string ckey = "uss-lamp-";
            string rkey = "uss-lamp-";

            lkey += LeftLampColor + "-";
            ckey += CenterLampColor + "-";
            rkey += RightLampColor + "-";

            lkey += LeftLampState ? "on" : "off";
            ckey += CenterLampState ? "on" : "off";
            rkey += RightLampState ? "on" : "off";

            LeftLampImage.Source = (ImageSource)FindResource(resourceKey: lkey);
            CenterLampImage.Source = (ImageSource)FindResource(resourceKey: ckey);
            RightLampImage.Source = (ImageSource)FindResource(resourceKey: rkey);
        }


        private void LeverImage_MouseUp(object sender, MouseButtonEventArgs e)
        {

            int delta;

            if (e.GetPosition(LeverImage).X > LeverImage.ActualWidth / 2) delta = 1;
            else delta = -1;

            LeverState += delta; 


            string key = "uss-lever";
            if (LeverState == 0) key += "-left";
            else if (LeverState == 1) key += "-vertical";
            else if (LeverState == 2) key += "-right";
            else throw new IndexOutOfRangeException("Invalid lever state!");

            LeverImage.Source = (ImageSource)FindResource(resourceKey: key);
        }

        private void SignalNumberLabel_Loaded(object sender, RoutedEventArgs e)
        {
            this.SignalNumberLabel.Content = this.PlateNumber.ToString();
        }

        private void SignalModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            UpdateLamps();
        }

    }
}
