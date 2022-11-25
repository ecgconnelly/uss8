using Iecc8.UI.Common;
using System;
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
    /// Interaction logic for SwitchModule.xaml
    /// </summary>
    public partial class SwitchModule : UserControl
    {
        public SwitchModule()
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
                SetValue(LeverStateProperty, value);
            }
        }

        [Category("USS"), Description("Gets or sets the lever position (left 0)")]
        public static readonly DependencyProperty LeverStateProperty =
            DependencyProperty.Register(nameof(LeverState), typeof(int), typeof(SwitchModule), new PropertyMetadata(0));

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
            DependencyProperty.Register(nameof(LeftLampState), typeof(bool), typeof(SwitchModule), new PropertyMetadata(true));

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
            DependencyProperty.Register(nameof(RightLampState), typeof(bool), typeof(SwitchModule), new PropertyMetadata(false));



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
            DependencyProperty.Register(nameof(PlateNumber), typeof(int), typeof(SwitchModule), new PropertyMetadata(99));


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
            DependencyProperty.Register(nameof(LeftLampColor), typeof(string), typeof(SwitchModule), new PropertyMetadata("green"));

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
            DependencyProperty.Register(nameof(RightLampColor), typeof(string), typeof(SwitchModule), new PropertyMetadata("amber"));

        public string SwitchStr
        {
            get
            {
                return (string)GetValue(SwitchStrProperty);
            }
            set
            {
                SetValue(SwitchStrProperty, value);
            }
        }
        [Category("USS"), Description("\"SUBAREA/SWITCHNUM\"")]
        public static readonly DependencyProperty SwitchStrProperty =
            DependencyProperty.Register(nameof(SwitchStr), typeof(string), typeof(SwitchModule), new PropertyMetadata(""));





        private void DrawLamps()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            string lkey = "uss-lamp-";
            string rkey = "uss-lamp-";

            lkey += LeftLampColor + "-";
            rkey += RightLampColor + "-";

            lkey += LeftLampState ? "on" : "off";
            rkey += RightLampState ? "on" : "off";

            LeftLampImage.Source = (ImageSource)FindResource(resourceKey: lkey);
            RightLampImage.Source = (ImageSource)FindResource(resourceKey: rkey);
        }


        private void LeverImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LeverState = 1 - LeverState; //toggle

            string key = "uss-lever-" + (LeverState == 1 ? "right" : "left");
            LeverImage.Source = (ImageSource)FindResource(resourceKey: key);
        }

        private void SwitchNumberLabel_Loaded(object sender, RoutedEventArgs e)
        {
            this.SwitchNumberLabel.Content = this.PlateNumber.ToString();
        }

        public World.Points PointsObject;

        private void OnPointsPropChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(World.Points.Reversed) || e.PropertyName == nameof(World.Points.HandCrankable))
            {
                if (PointsObject.Inconsistent || !PointsObject.Proved)
                {
                    LeftLampState = false;
                    RightLampState = false;
                }
                else if (PointsObject.Reversed)
                {
                    LeftLampState = false;
                    RightLampState = true;
                }
                else
                {
                    LeftLampState = true;
                    RightLampState = false;
                }

                DrawLamps();
            }
        }



        private void SwitchModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            ushort SubAreaID = ushort.Parse((SwitchStr.Split('/')[0]));
            ushort SwitchID  = ushort.Parse((SwitchStr.Split('/')[1]));

            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                PointsObject = vm.World.Region.SubAreas[SubAreaID].PowerPoints[SwitchID];
                PointsObject.PropertyChanged += OnPointsPropChanged;
                
            }


            DrawLamps();
        }
    }
}
