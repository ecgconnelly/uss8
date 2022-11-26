using Iecc8.UI.Panels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
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
    /// Interaction logic for CodeButton.xaml
    /// </summary>
    public partial class CodeButton : UserControl
    {
        public string SignalModulesStr
        {
            get
            {
                return (string)GetValue(SignalModulesStrProperty);
            }
            set
            {
                SetValue(SignalModulesStrProperty, value);
            }
        }
        [Category("USS"), Description("Comma-separated list of module names this code button applies to")]
        public static readonly DependencyProperty SignalModulesStrProperty =
            DependencyProperty.Register(nameof(SignalModulesStr), typeof(string), typeof(SignalModule), new PropertyMetadata(""));

        public string SwitchModulesStr
        {
            get
            {
                return (string)GetValue(SwitchModulesStrProperty);
            }
            set
            {
                SetValue(SwitchModulesStrProperty, value);
            }
        }
        [Category("USS"), Description("Comma-separated list of module names this code button applies to")]
        public static readonly DependencyProperty SwitchModulesStrProperty =
            DependencyProperty.Register(nameof(SwitchModulesStr), typeof(string), typeof(SwitchModule), new PropertyMetadata(""));


        public CodeButton()
        {
            InitializeComponent();
           
        }

        private void Press()
        {

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Press(); //this can be expanded later
        }

        public List<SignalModule> signalModules { get; set; }
        public List<SwitchModule> switchModules { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }

            //populate lists of signal and switch modules we code for
            signalModules = new List<SignalModule>();
            switchModules = new List<SwitchModule>();
            foreach(string modname in SignalModulesStr.Split(','))
            {
                SignalModule mod = (SignalModule)LogicalTreeHelper.
                    FindLogicalNode(Parent, modname);

                if(mod != null)
                {
                    signalModules.Add(mod);
                }
            }

            foreach (string modname in SwitchModulesStr.Split(','))
            {
                if (modname.Length == 0) { continue; }

                SwitchModule mod = (SwitchModule)LogicalTreeHelper.
                    FindLogicalNode(Parent, modname);

                if (mod != null)
                {
                    switchModules.Add(mod);
                }
            }
        }
    }
}
