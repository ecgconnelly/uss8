using Iecc8.Messages;
using Iecc8.UI.Equipment.USS;
using Iecc8.World;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        public class CTCFieldController
        {
            public CTCColumn Column = null;
            public bool WaitingForSwitch
            { get; private set; }
            
            public bool WaitingForSignal
            { get; private set; }

            public bool Waiting
            {
                get { return WaitingForSignal || WaitingForSwitch; }
            }

            public async void SendControlCode(CodeLine.ControlTransmission trans)
            {
                WaitingForSwitch = true;
                foreach (World.Points sw in trans.switches)
                {
                    if (!sw.Movable)
                    {
                        Debug.Print("This switch cannot be moved right now");
                        continue;
                    }
                    Debug.Print("Throwing switch");
                    WaitingForSwitch = true;
                    await sw.SwingAsync(trans.requestedReverse);

                    Debug.Print("Waiting for switch to prove");
                    for (int loopcount = 0; loopcount < 30; loopcount++)
                    {
                        if (sw.Reversed == trans.requestedReverse && sw.Proved)
                        {
                            break;
                        }

                        Debug.Print("{0}", loopcount);
                        await Task.Delay(1000);
                    }

                    Debug.Print(sw.Reversed.ToString());
                }
                WaitingForSwitch = false;


                World.Route route = null;
                if (trans.requestedIndication == ESignalIndication.Stop)
                {
                    foreach (World.ControlledSignal sig in trans.signals)
                    {
                        await sig.CancelAsync();
                    }
                    foreach (World.ControlledSignal sig in trans.signals)
                    {
                        Debug.Print("Waiting for signal to drop");
                        for (int loopcount = 0; loopcount < 30; loopcount++)
                        {
                            if (sig.Indication == ESignalIndication.Stop)
                            {
                                break;
                            }

                            Debug.Print("{0}", loopcount);
                            await Task.Delay(1000);
                        }
                    }
                    
                }
                if (trans.requestedIndication != ESignalIndication.Stop) // requested indication was not Stop
                {
                    foreach (World.ControlledSignal sig in trans.signals)
                    {
                        route = sig.ReadyRoute;
                        if (route != null) break;
                    }

                    if (route != null)
                    {
                        await route.CallAsync(fleet: trans.requestedIndication == ESignalIndication.Fleet);
                        foreach (World.ControlledSignal sig in trans.signals)
                        {
                            Debug.Print("Waiting for signal {0}to display", sig.ID);
                            for (int loopcount = 0; loopcount < 30; loopcount++)
                            {
                                if (sig.Indication == ESignalIndication.Stop)
                                {
                                    break;
                                }

                                Debug.Print("{0}", loopcount);
                                await Task.Delay(1000);
                            }
                        }
                    }
                    else if (trans.signals != null && trans.requestedIndication != ESignalIndication.Stop)
                    {
                        Debug.Print("Not displaying a signal because no routes were ready");
                    }
                }
                WaitingForSignal = false;

                while (!Column.ColumnCodeLine.RequestLineAccessReceive(this))
                {
                    await Task.Delay(1000);
                }
                Debug.Print("Got the indication line");

                Column.TransmitSound.Source = new Uri("Sounds/Code-receive.wav", UriKind.Relative);
                Column.TransmitSound.Position = new System.TimeSpan(0);
                Column.TransmitSound.Play();
                await Task.Delay(4000);
                Column.TransmitSound.Stop();
                Debug.Assert(Column.ColumnCodeLine.ReleaseLineAccessReceive(this));
                Debug.Print("Released the indication line");

                Column.AwaitingIndicationCode = false;
                
            }


            public CTCFieldController(CTCColumn col)
            {
                this.Column = col;
            }
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


        public string CodeLineName
        {
            get
            {
                return (string)GetValue(CodeLineNameProperty);
            }
            set
            {
                SetValue(CodeLineNameProperty, value);
            }
        }
        [Category("USS"), Description("Gets or sets the name of the codeline this CTCColumn is connected to")]
        public static readonly DependencyProperty CodeLineNameProperty =
            DependencyProperty.Register(nameof(CodeLineName), typeof(string), typeof(CTCColumn));


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

        private bool AwaitingIndicationCodeImpl = false;
        public bool AwaitingIndicationCode
        {
            get { return AwaitingIndicationCodeImpl; }
            set
            {
                AwaitingIndicationCodeImpl = value;
                sig.UpdateLampStates();
                sw.UpdateLampStates();
            }
        }


        private CodeLine FindCodeLine()
        {
            if (this.CodeLineName == "") return null;

            var siblings = LogicalTreeHelper.GetChildren(Parent);
            foreach ( var child in siblings )
            {
                if (child.GetType() == typeof(CodeLine))
                {
                    CodeLine cl = (CodeLine)child;
                    if ((cl.Name) == CodeLineName) return cl;

                }
            }
            return null;
        }

        public CTCFieldController FieldController = null;
        CodeLine ColumnCodeLine = null;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SignalModule sigmod = this.HasSignalPlate ? this.sig : null;
            SwitchModule swmod = this.HasSwitchPlate ? this.sw : null;
            CodeButton code = this.code;
            FieldController = new CTCFieldController(this);
            ColumnCodeLine = FindCodeLine();
            AwaitingIndicationCode = false;


            code.Column = this;
            code.ColumnSignalModule = sigmod;
            code.ColumnSwitchModule = swmod;
            code.ColumnCodeLine = ColumnCodeLine;

            sw.Column = this;
            sig.Column = this;

        }
    }
}
