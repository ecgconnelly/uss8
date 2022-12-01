using Iecc8.UI.Equipment.USS;
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

namespace Iecc8.UI.Assembly.USS
{
    /// <summary>
    /// Interaction logic for CTCColumn.xaml
    /// </summary>
    public partial class CTCColumn : UserControl
    {
        public CTCColumn()
        {
            InitializeComponent();
        }

        #region DependencyProperties
        public int SignalPlateNumber
        {
            get
            {
                return (int)GetValue(SignalPlateNumberProperty);
            }
            set
            {
                SetValue(SignalPlateNumberProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the number on the lever plate")]
        public static readonly DependencyProperty SignalPlateNumberProperty =
            DependencyProperty.Register(nameof(SignalPlateNumber), typeof(int), typeof(CTCColumn));


        public int SwitchPlateNumber
        {
            get
            {
                return (int)GetValue(SwitchPlateNumberProperty);
            }
            set
            {
                SetValue(SwitchPlateNumberProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the number on the lever plate")]
        public static readonly DependencyProperty SwitchPlateNumberProperty =
            DependencyProperty.Register(nameof(SwitchPlateNumber), typeof(int), typeof(CTCColumn));


        public bool HasSwitchPlate
        {
            get
            {
                return (bool)GetValue(HasSwitchPlateProperty);
            }
            set
            {
                SetValue(HasSwitchPlateProperty, value);
            }
        }

        [Category("USS"), Description("Whether this column has a switch plate")]
        public static readonly DependencyProperty HasSwitchPlateProperty =
            DependencyProperty.Register(nameof(HasSwitchPlate), typeof(bool), typeof(CTCColumn));

        public bool HasSignalPlate
        {
            get
            {
                return (bool)GetValue(HasSignalPlateProperty);
            }
            set
            {
                SetValue(HasSignalPlateProperty, value);
            }
        }

        [Category("USS"), Description("Whether this column has a signal plate")]
        public static readonly DependencyProperty HasSignalPlateProperty =
            DependencyProperty.Register(nameof(HasSignalPlate), typeof(bool), typeof(CTCColumn));


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
            DependencyProperty.Register(nameof(LeftSignalStr), typeof(string), typeof(CTCColumn), new PropertyMetadata(""));

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
            DependencyProperty.Register(nameof(RightSignalStr), typeof(string), typeof(CTCColumn), new PropertyMetadata(""));


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
            DependencyProperty.Register(nameof(SwitchStr), typeof(string), typeof(CTCColumn), new PropertyMetadata(""));


        #endregion
    }
}
