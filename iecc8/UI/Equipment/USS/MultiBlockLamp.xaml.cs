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
    /// Interaction logic for MultiBlockLamp.xaml
    /// </summary>
    public partial class MultiBlockLamp : UserControl
    {
        #region Properties
        public string TrackCircuitStr
        {
            get
            {
                return (string)GetValue(TrackCircuitStrProperty);
            }
            set
            {
                SetValue(TrackCircuitStrProperty, value);
            }
        }
        [Category("USS"), Description("\"SUBAREA/CIRCUITNUM[,SUBAREA/CIRCUITNUM[...]]\"")]
        public static readonly DependencyProperty TrackCircuitStrProperty =
            DependencyProperty.Register(nameof(TrackCircuitStr), typeof(string), typeof(MultiBlockLamp), new PropertyMetadata(""));



        /// <summary>
        /// What color lamp to use.
        /// </summary>
        [Category("USS"), Description("Gets or sets the lamp color for this occupancy light.")]
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

        /// <summary>
        /// The lamp color property.
        /// </summary>
        public static readonly DependencyProperty LampColorProperty =
            DependencyProperty.Register(nameof(LampColor), typeof(string), typeof(MultiBlockLamp), new PropertyMetadata("amber"));



        #endregion

        public MultiBlockLamp()
        {
            InitializeComponent();
        }

        private void PopulateTrackCircuitsList()
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm == null) return;
            if (TrackCircuitStr.Length == 0) return;

            IndicatedTrackCircuits = new List<World.TrackCircuit>();

            foreach (string cstr in TrackCircuitStr.Split(','))
            {
                ushort subarea = ushort.Parse(cstr.Split('/')[0]);
                uint id = uint.Parse(cstr.Split('/')[1]);

                World.TrackCircuit circ =
                    vm.World.Region.GetTrackCircuit(id, subarea);

                if (circ != null)
                {
                    IndicatedTrackCircuits.Add(circ);
                    circ.PropertyChanged += OnCircuitPropChanged;
                }
            }
        }

        private void Update()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            string imgname = "uss-lamp-" + GetValue(LampColorProperty) as string;

            if (AnyCircuitsOccupied || AnyCircuitsReversed) imgname += "-on";
            else imgname += "-off";

            LampImage.Source = (ImageSource)FindResource(resourceKey: imgname);
        }

        private void OnCircuitPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            if (e.PropertyName == nameof(World.TrackCircuit.Occupied) 
                || e.PropertyName == nameof(World.TrackCircuit.ReversedHandPoints))
            {
                bool occ = false;
                bool rev = false;
                
                foreach(World.TrackCircuit circ in IndicatedTrackCircuits)
                {
                    if (circ.Occupied) occ = true;
                    if (circ.ReversedHandPoints) rev = true;
                }

                AnyCircuitsOccupied = occ;
                AnyCircuitsReversed = rev;

                Update();
            }
        }

        public List<World.TrackCircuit> IndicatedTrackCircuits 
        { get; private set;}

        public bool AnyCircuitsOccupied
        { get; private set; }

        public bool AnyCircuitsReversed
        { get; private set; }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            PopulateTrackCircuitsList();
            Update();
        }
    }
}
