using KWDP.View;
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

namespace KWDP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PatientView PatientView { get; set; }
        public static Window MyWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MyWindow = this;
            PatientView = new PatientView();
            this.Content = PatientView;
        }
    }
}
