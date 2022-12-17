using Iecc8.Messages;
using Iecc8.Schema;
using Iecc8.UI.Assembly.USS;
using Iecc8.UI.Panels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.ServiceModel.Description;
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
using static Iecc8.UI.Assembly.USS.CodeLine;

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

        private bool RouteMatchesLeverSettings(World.Route route)
        {
            IReadOnlyList<World.RoutePointPosition>
                requiredSwitches = route.PointPositions;

            foreach(World.RoutePointPosition rpp in requiredSwitches)
            {
                bool leverPresent = false;
                bool requestedReverse = false;

                foreach(SwitchModule swm in SwitchModules)
                {
                    if (swm.PointsObject == rpp.Points)
                    {
                        leverPresent = true;
                        requestedReverse = swm.LeverState == 1;
                        break;
                    }
                }
                if (!leverPresent) return false;
                if (requestedReverse != rpp.Reverse) return false;
            }

            return true;
        }

        public World.Route PickRequestedRoute(SignalModule sigmod)
        {
            bool left = sigmod.LeverState == 0;
            bool right = sigmod.LeverState == 2;

            List<World.ControlledSignal> siglist = new List<World.ControlledSignal>();
            if (left) siglist = sigmod.LeftSignals;
            if (right) siglist = sigmod.RightSignals;

            if (siglist.Count == 0) return null;

            World.Route req = null;
            foreach (World.ControlledSignal sig in siglist)
            {
                
                foreach (var candidate in sig.RoutesFrom)
                {
                    if (RouteMatchesLeverSettings(candidate.Value))
                    {
                        if (req == null) req = candidate.Value;
                        else throw new Exception(
                            "Multiple candidate routes from one signal" +
                            " this should never happen");
                    }
                }
            }
            return req;
        }

        public SignalModule ColumnSignalModule;
        public SwitchModule ColumnSwitchModule;
        public CodeLine ColumnCodeLine;
        public CTCColumn Column;




        private async void Press(bool callOn)
        {
            
            Column.AwaitingIndicationCode = true;
            while (ColumnCodeLine.RequestLineAccessTransmit(this) == false)
            {
                Debug.Print("Line is busy, waiting");
                await Task.Delay(500);
            }
            Debug.Print("Got the code line");
            
            ControlTransmission trans = 
                new ControlTransmission(ColumnSignalModule, ColumnSwitchModule, callOn);

            Column.TransmitSound.Source = new Uri("Sounds/Code-send.wav", UriKind.Relative);
            Column.TransmitSound.Position = new System.TimeSpan(0);
            Column.TransmitSound.Play();
            await Task.Delay(4000);
            Column.TransmitSound.Stop();

            Column.FieldController.SendControlCode(trans);
            Debug.Assert(ColumnCodeLine.ReleaseLineAccessTransmit(this));
            Debug.Print("Released the code line");
           


            //ColumnCodeLine.QueueControlTransmission(ColumnSignalModule, ColumnSwitchModule);
            

        }

        private List<SwitchModule> FindSwitchModulesInPanel()
        {
            List<SwitchModule> mods = new List<SwitchModule>();

            var siblings = LogicalTreeHelper.
                    GetChildren(Parent);
            
            foreach (var child in siblings)
            {
                if (child.GetType() == typeof(SwitchModule) && child != null)
                {
                    mods.Add((SwitchModule)child);
                }
            }

            return mods;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ButtonPressSound.Source = new Uri("Sounds/Button-press.wav", UriKind.Relative);
            ButtonPressSound.Position = new System.TimeSpan(0);
            ButtonPressSound.Play();

            
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            bool callOn = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
          
            ButtonReleaseSound.Source = new Uri("Sounds/Button-release.wav", UriKind.Relative);
            ButtonReleaseSound.Position = new System.TimeSpan(0);
            ButtonReleaseSound.Play();

            Press(callOn); //this can be expanded later
        }

        public List<SignalModule> SignalModules { get; set; }
        public List<SwitchModule> SwitchModules { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }
            if (SignalModulesStr == "") { return; }

            //populate lists of signal and switch modules we code for
            SignalModules = new List<SignalModule>();
            SwitchModules = new List<SwitchModule>();
            foreach(string modname in SignalModulesStr.Split(','))
            {
                SignalModule mod = (SignalModule)LogicalTreeHelper.
                    FindLogicalNode(Parent, modname);

                if(mod != null)
                {
                    SignalModules.Add(mod);
                }
            }

            SwitchModules = FindSwitchModulesInPanel();

        }
    }
}
