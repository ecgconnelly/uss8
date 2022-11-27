using Iecc8.UI.Common;
using System;
using System.ComponentModel;
using System.Resources;
using System.Windows;
using System.Windows.Media;

namespace Iecc8.UI.Equipment.USS
{
    /// <summary>
    /// Interaction logic for HSection.xaml
    /// </summary>
    public partial class BlockLamp : TrackSection
    {
        /// <summary>
        /// Whether there is a TCB at the left end of this section.
        /// </summary>
        [Category("Track"), Description("Gets or sets whether there is a track circuit break at the left side of this section.")]
        public bool TCBLeft
        {
            get
            {
                return (bool)GetValue(TCBLeftProperty);
            }
            set
            {
                SetValue(TCBLeftProperty, value);
            }
        }

        /// <summary>
        /// The TCB at left end property.
        /// </summary>
        public static readonly DependencyProperty TCBLeftProperty = DependencyProperty.Register(nameof(TCBLeft), typeof(bool), typeof(BlockLamp), new PropertyMetadata(false, OnTCBPropertyChanged));

        /// <summary>
        /// Whether there is a TCB at the right end of this section.
        /// </summary>
        [Category("Track"), Description("Gets or sets whether there is a track circuit break at the right side of this section.")]
        public bool TCBRight
        {
            get
            {
                return (bool)GetValue(TCBRightProperty);
            }
            set
            {
                SetValue(TCBRightProperty, value);
            }
        }

        /// <summary>
        /// The TCB at right end property.
        /// </summary>
        public static readonly DependencyProperty TCBRightProperty = DependencyProperty.Register(nameof(TCBRight), typeof(bool), typeof(BlockLamp), new PropertyMetadata(false, OnTCBPropertyChanged));

        /// <summary>
        /// Constructs a new HSection.
        /// </summary>
        public BlockLamp()
        {
            InitializeComponent();
        }

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
            DependencyProperty.Register(nameof(LampColor), typeof(string), typeof(BlockLamp), new PropertyMetadata("amber"));



        
        /// <summary>
        /// Refreshes the appearance of this object based on changes to the equipment.
        /// </summary>
        protected override void Update()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            
            this.LampImage.Source = (ImageSource)FindResource(resourceKey: PickLampImageName());
        }

        private string PickLampImageName()
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm == null)           return "uss-lamp-s-inconsistent";
            if (TrackCircuit == null) return "uss-lamp-s-unknown";

            string imgname = "uss-lamp-" + GetValue(LampColorProperty) as string;
            if (TrackCircuit.Occupied) imgname += "-on";
            else imgname += "-off";

            return imgname;

         }
            
    



        /// <summary>
        /// Invoked when the TCB property changes on an object.
        /// </summary>
        /// <param name="d">The object whose value changed.</param>
        /// <param name="e">Details about the change.</param>
        private static void OnTCBPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlockLamp)d).Update();
        }
    }
}
