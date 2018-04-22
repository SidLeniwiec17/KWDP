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
using System.Windows.Shapes;

namespace KWDP.Objects
{
    /// <summary>
    /// Interaction logic for Wywiad.xaml
    /// </summary>
    public partial class Wywiad : Window
    {
        public Wywiad()
        {
            InitializeComponent();
        }

        private void WrocButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
