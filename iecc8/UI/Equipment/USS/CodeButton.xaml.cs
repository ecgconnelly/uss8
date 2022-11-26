using Iecc8.Messages;
using Iecc8.Schema;
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

                foreach(SwitchModule swm in switchModules)
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

        public World.Route pickRequestedRoute(SignalModule sigmod)
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

        private async void Press()
        {
            List<World.ControlledSignal> signalsToDrop= new List<World.ControlledSignal>();

            foreach (SignalModule sigmod in signalModules)
            {
                if (sigmod.LeverState == 1) //centered
                {
                    foreach (World.ControlledSignal dropsig in 
                        sigmod.LeftSignals.Concat(sigmod.RightSignals))
                    {
                        signalsToDrop.Add(dropsig);
                        
                    }
                    continue; //next signal module
                }
                
                World.Route requestedRoute = pickRequestedRoute(sigmod);
                if (requestedRoute == null) continue;
                Debug.Print("Code requested: {0} --> {1}", 
                    requestedRoute.Entrance.ID,
                    requestedRoute.Exit.ID);
                

                if (requestedRoute.Available)
                {
                    Debug.Print("Setting route");
                    foreach (World.RoutePointPosition rpp in requestedRoute.PointPositions)
                    {
                        await rpp.Points.SwingAsync(rpp.Reverse);
                    }
                    requestedRoute.Entrance.SetCurrentRoute(requestedRoute);
                }
                else
                {
                    Debug.Print("Requested route is not available");
                }
            }

            foreach (World.ControlledSignal dropsig in signalsToDrop)
            {
                await dropsig.CancelAsync();
            }

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
