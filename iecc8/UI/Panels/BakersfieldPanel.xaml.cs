using Iecc8.UI.Equipment.USS;
using Iecc8.UI.TopLevel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Iecc8.UI.Panels
{
    /// <summary>
    /// Interaction logic for BakersfieldPanel.xaml
    /// </summary>
    public partial class BakersfieldPanel : UserControl
    {

     

        public void GetLogicalTree(Object obj)
        {
            if (obj is FrameworkElement)
            {
                FrameworkElement fe = (FrameworkElement)obj;
                if (fe.GetType() == typeof(Iecc8.UI.Equipment.USS.SignalModule))
                    Console.WriteLine("SIGNAL MODULE: " + fe.GetType() + ", Name: " + fe.Name);
                if (fe.GetType() == typeof(Iecc8.UI.Equipment.USS.SwitchModule))
                    Console.WriteLine("SWITCH MODULE: " + fe.GetType() + ", Name: " + fe.Name);
                // recurse through the children
                IEnumerable children = LogicalTreeHelper.GetChildren(fe);
                foreach (object child in children)
                {
                   GetLogicalTree(child);
                }
            }
            else
            {
                // stop recursing as we certainly can't have any more FrameworkElement children
                //Console.WriteLine("Logical Type: " + obj.GetType());
            }
        }


        public BakersfieldPanel()
        {
            InitializeComponent();

            SignalModule sig4 = null;
            sig4 = (SignalModule)LogicalTreeHelper.FindLogicalNode(this, "SignalModule4");
            Debug.Print(sig4.CenterLampImage.Source.ToString());
        }
    }
}
