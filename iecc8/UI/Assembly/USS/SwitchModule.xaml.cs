using System;
using System.Collections.Generic;
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
                return (int) GetValue(LeverStateProperty);
            }

            set
            {
                SetValue(LeverStateProperty, value);
            }
        }
        public static readonly DependencyProperty LeverStateProperty = 
            DependencyProperty.Register(nameof(LeverState), typeof(int), typeof(SwitchModule), new PropertyMetadata(0));


        private void Lamp_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SetLampStates(bool left, bool right)
        {

        }

        private void Lamp_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void LeverImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
