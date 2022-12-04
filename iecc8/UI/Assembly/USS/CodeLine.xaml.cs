using Iecc8.Messages;
using Iecc8.UI.Equipment.USS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for CodeLine.xaml
    /// </summary>
    public partial class CodeLine : UserControl
    {
        public CodeLine()
        {
            InitializeComponent();

            ActivateCodeLine += OnActivateCodeLine;
        }

     

        public class ControlTransmission
        {
            public List<World.ControlledSignal> signals;
            public List<World.Points> switches;
            public ESignalIndication requestedIndication;
            public bool requestedReverse;

            public ControlTransmission(SignalModule sigmod, SwitchModule swmod, bool callOn = false)
            {
                signals = new List<World.ControlledSignal>();
                switches = new List<World.Points>();

                if (sigmod != null)
                {
                    bool left = sigmod.LeverState == 0;
                    bool right = sigmod.LeverState == 2;
                    bool stop = sigmod.LeverState == 1;

                    if (left)
                    {
                        signals = sigmod.LeftSignals;
                        requestedIndication = sigmod.FleetSwitchOn ? ESignalIndication.Fleet : ESignalIndication.Proceed;
                    }
                    else if(right)
                    {
                        signals = sigmod.RightSignals;
                        requestedIndication = sigmod.FleetSwitchOn ? ESignalIndication.Fleet : ESignalIndication.Proceed;
                    }
                    else if (stop)
                    {
                        signals = Enumerable.Concat(sigmod.LeftSignals, sigmod.RightSignals).ToList();
                        requestedIndication = ESignalIndication.Stop;
                    }
                    else
                    {
                        throw new Exception("Missing signal lever state for signal module " + sigmod.PlateNumber.ToString());
                    }

                    if (callOn && (left || right))
                    {
                        requestedIndication = ESignalIndication.FlagBy;
                    }
                }

                if (swmod != null)
                {
                    switches.Add(swmod.PointsObject);
                    requestedReverse = swmod.LeverState == 1;
                }
            }
        }


        private void SetTransmitLamp(bool lampOn)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            string txKey = "uss-lamp-white-";
            txKey += lampOn ? "on" : "off";
            this.TransmitLampImage.Source = (ImageSource)FindResource(resourceKey: txKey);
        }


        private void SetReceiveLamp(bool lampOn)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            string txKey = "uss-lamp-white-";
            txKey += lampOn ? "on" : "off";
            this.ReceiveLampImage.Source = (ImageSource)FindResource(resourceKey: txKey);
        }


        private bool CodeLineActive = false;
        private Queue<ControlTransmission> ControlQueue = new Queue<ControlTransmission>();

        public event EventHandler ActivateCodeLine;

        

   

        private async Task Receive()
        {
            SetReceiveLamp(true);
            await Task.Delay(3500);
            SetReceiveLamp(false);
        }

        private async void OnActivateCodeLine(object sender, EventArgs e)
        {
            CodeLineActive = true;
            int count = 0;
            while (ControlQueue.Count > 0)
            {
                count++;
                ControlTransmission trans = ControlQueue.Dequeue();
                if (trans == null) continue;

                Debug.Print("calling Transmit for transmission {0}", count);
                //await Transmit(trans);
                Debug.Print("back from calling Transmit for transmission {0}", count);
                await Task.Delay(1000);
            }
            CodeLineActive = false;
        }

        private object TransmitLineHeldBy = null;
        private object ReceiveLineHeldBy = null;

        public bool RequestLineAccessTransmit(object sender)
        {
            if (TransmitLineHeldBy != null) { return false; }
            if (ReceiveLineHeldBy != null) { return false; }

            SetTransmitLamp(true);
            TransmitLineHeldBy = sender;
            return true;
        }

        public bool RequestLineAccessReceive(object sender)
        {
            if (TransmitLineHeldBy != null) { return false; }
            if (ReceiveLineHeldBy != null) { return false; }

            SetReceiveLamp(true);
            ReceiveLineHeldBy = sender;
            return true;
        }

        public bool ReleaseLineAccessTransmit(object sender)
        {
            if (TransmitLineHeldBy == sender)
            {
                TransmitLineHeldBy = null;
                SetTransmitLamp(false);
                return true;
            }
            
            return false; 
        }

        public bool ReleaseLineAccessReceive(object sender)
        {
            if (ReceiveLineHeldBy == sender)
            {
                ReceiveLineHeldBy = null;
                SetReceiveLamp(false);
                return true;
            }

            return false;
        }

        public void QueueControlTransmission(
            SignalModule sigmod,
            SwitchModule swmod)
        {
            // ControlTransmission: grab current lever settings
            // so that lever changes after Code is pressed do not have effect in the field
            ControlTransmission trans = new ControlTransmission(sigmod, swmod);
            ControlQueue.Enqueue(trans);

            if (!CodeLineActive)
            {
                Debug.Print("About to emit ActivateCodeLine");
                ActivateCodeLine(this, EventArgs.Empty);
                Debug.Print("Back from emitting ActivateCodeLine");
            }
            else
            {
                Debug.Print("Code line already active");
            }
        }


    }


}
